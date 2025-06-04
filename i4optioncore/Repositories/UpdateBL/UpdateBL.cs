using static StackExchange.Redis.Role;
using System.Collections.Generic;
using System;
using i4optioncore.DBModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using i4optioncore.DBModelsMaster;
using Newtonsoft.Json;
using static i4optioncore.Models.CommonModel;
using i4optioncore.Repositories.GlobalMarket;
using Azure.Core;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Builder;

namespace i4optioncore.Repositories.UpdateRedis
{
    public class UpdateBL : IUpdateBL
    {
        private readonly i4option_dbContext db;
        private readonly MasterdataDbContext dbMaster;
        private readonly IRedisBL redisBL;
        private readonly ICommonBL commonBL;
        private readonly IGlobalMarketBL globalMarketBL;
        public UpdateBL(i4option_dbContext db, IRedisBL redisBL, MasterdataDbContext dbMaster, ICommonBL commonBL, IGlobalMarketBL globalMarketBL)
        {
            this.db = db;
            this.redisBL = redisBL;
            this.dbMaster = dbMaster;
            this.commonBL = commonBL;
            this.globalMarketBL = globalMarketBL;
        }

        public bool UpdateMarketHoliday(bool IsMarketHoliday, int Days)
        {
            db.Configurations.FirstOrDefault(x => x.Key == "IsMarketOpen").Value = IsMarketHoliday ? "True" : "False";
            db.Configurations.FirstOrDefault(x => x.Key == "HistoryData").Value = Days.ToString();
            db.SaveChanges();
            return true;
        }

        public async Task UpdateTouchlineRedis(List<string> symbols)
        {
            bool afterMarket = false;
            if ((DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 15 || DateTime.Now.Hour < 9 || (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 15))
                afterMarket = true;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                afterMarket = true;
            var holidays = await GetHolidays();
            if (holidays.Any(h => h.Date == DateTime.Now.Date)) afterMarket = true;

            await Task.WhenAll(db.TouchlineSubscriptions.Where(t => symbols.Contains(t.Symbol))
                .Select(t => afterMarket ? redisBL.SetValue($"TOUCHLINE_{t.Symbol}", JsonConvert.SerializeObject(t)) : redisBL.SetValueWithExpiry($"TOUCHLINE_{t.Symbol}", JsonConvert.SerializeObject(t), 1)));
        }
        public async Task UpdateSegmentTouchlineRedis(List<DateTime> dates, string segment)
        {
            await Task.WhenAll(dbMaster.SegmentTouchlines.Where(t => t.Segment == segment && dates.Contains(t.LastUpdatedTime.Date))
                .Select(t => redisBL.SetValue($"SEGMENTTOUCHLINE_{segment}_{t.LastUpdatedTime:dd-MM-yyyy}", JsonConvert.SerializeObject(t))));
        }
        public async Task UpdateSymbolTouchlineRedis(List<DateTime> dates, string symbol)
        {
            await Task.WhenAll(dbMaster.TouchlineSubscriptions.Where(t => t.Symbol == symbol && dates.Contains(t.LastUpdatedTime.Date))
                .Select(t => redisBL.SetValue($"SYMBOLTOUCHLINE_{symbol}_{t.LastUpdatedTime:dd-MM-yyyy}", JsonConvert.SerializeObject(t))));
        }
        public async Task UpdateFullTouchlineRedis()
        {
            await Task.WhenAll(db.TouchlineSubscriptions.Select(t => redisBL.SetValue($"TOUCHLINE_{t.Symbol}", JsonConvert.SerializeObject(t))));
        }
        public async Task UpdateSymbolList()
        {
            await Task.WhenAll(db.Symbols.Select(t => redisBL.SetValue($"{t.Symbol1}", t.SymbolId.ToString())));
        }

        public async Task UpdateSnapshotRedis()
        {
            await Task.WhenAll(dbMaster.HistorySubscriptions.Where(h => h.Symbol == "NIFTY 50").Select(t =>
                redisBL.SetValue($"Snapshot_{t.Symbol}_{t.LastTradeTime:dd-MM-yyyy HH:mm}", JsonConvert.SerializeObject(t))
            ));

            await Task.WhenAll(dbMaster.HistorySubscriptions.Where(h => h.Symbol == "NIFTY BANK").Select(t =>
                redisBL.SetValue($"Snapshot_{t.Symbol}_{t.LastTradeTime:dd-MM-yyyy HH:mm}", JsonConvert.SerializeObject(t))
            ));

            var expiries = await GetAllExpiry();
            foreach (var expiry in expiries)
            {
                string expiryString = expiry.ToString("yyMMdd");

                await Task.WhenAll(dbMaster.HistorySubscriptions.Where(h => h.Symbol.Contains(expiryString) && h.Symbol.StartsWith("NIFTY") && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(t =>
                               redisBL.SetValue($"OptionSnapshot_{t.Symbol}_{expiry:dd-MM-yyyy}_{t.LastTradeTime:dd-MM-yyyy HH:mm}", JsonConvert.SerializeObject(t))
                           ));

                await Task.WhenAll(dbMaster.HistorySubscriptions.Where(h => h.Symbol.Contains(expiryString) && h.Symbol.StartsWith("BANKNIFTY") && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(t =>
                    redisBL.SetValue($"OptionSnapshot_{t.Symbol}_{expiry:dd-MM-yyyy}_{t.LastTradeTime:dd-MM-yyyy HH:mm}", JsonConvert.SerializeObject(t))
                ));
            }
        }
        public async Task<bool> UpdateIV(List<IVRequest> request)
        {

            var data = request.Select(x => new Ivdatum
            {
                Ceiv = x.CEIV,
                Peiv = x.PEIV,
                Symbol = x.Symbol,
                Expiry = Convert.ToDateTime(x.Expiry),
                UpdatedOn =DateTime.Now
            });

            db.Ivdata.AddRange(data);

            //request.ForEach(x =>
            //{
            //    db.Ivdata.Add(new Ivdatum
            //    {
            //        Ceiv = x.CEIV,
            //        Peiv = x.PEIV,
            //        Symbol = x.Symbol,
            //        Expiry = Convert.ToDateTime(x.Expiry),
            //        UpdatedOn = Convert.ToDateTime(x.Date)
            //    });
            //});
            await db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateIVPIVR(DateTime date)
        {

            var _allIVData = db.Ivdata.Where((iv) => iv.UpdatedOn.Date >= date.AddDays(-252).Date
             && iv.UpdatedOn.Date <= date.Date
             ).ToList();

            _allIVData.Select(x => new { x.Symbol, x.Expiry }).Distinct().ToList().ForEach(symbol =>
            {
                var allIVData = _allIVData.Where(x => x.Symbol.Equals(symbol.Symbol) && x.Expiry == symbol.Expiry).ToList();
                var _todayIV = allIVData.Where(x => x.Expiry.Value.Date >= date.Date).OrderBy(x => x.Expiry.Value.Date).FirstOrDefault();
                if (_todayIV != null)
                {
                    var todayIV = (_todayIV.Ceiv + _todayIV.Peiv) * 100 / 2;

                    var count = allIVData.Count(x => ((x.Ceiv + x.Peiv) * 100 / 2) < todayIV);
                    var minIV = allIVData.Min(x => (x.Ceiv + x.Peiv) * 100 / 2);
                    var maxIV = allIVData.Max(x => (x.Ceiv + x.Peiv) * 100 / 2);
                    var minCEIV = allIVData.Min(x => (x.Ceiv) * 100 / 2);
                    var maxCEIV = allIVData.Max(x => (x.Ceiv) * 100 / 2);
                    var minPEIV = allIVData.Min(x => (x.Peiv) * 100 / 2);
                    var maxPEIV = allIVData.Max(x => (x.Peiv) * 100 / 2);


                    var ce_count = allIVData.Count(x => (x.Ceiv * 100) < (_todayIV.Ceiv * 100));
                    var pe_count = allIVData.Count(x => (x.Peiv * 100) < (_todayIV.Peiv * 100));


                    var ivp = count * 100 / 252;
                    var ivr = (maxIV - minIV) == 0 ? 0 : (todayIV - minIV) * 100 / (maxIV - minIV);
                    var Ceivr = (maxCEIV - minCEIV) == 0 ? 0 : (_todayIV.Ceiv - minCEIV) * 100 / (maxCEIV - minCEIV);
                    var Peivr = (maxPEIV - minPEIV) == 0 ? 0 : (_todayIV.Peiv - minPEIV) * 100 / (maxPEIV - minPEIV);



                    if (ivr > 99.9m) ivr = 99.9m;
                    db.Ivdata.Where((iv) => iv.Symbol == symbol.Symbol && iv.Expiry == symbol.Expiry && iv.UpdatedOn.Date == date.Date).ToList().ForEach(x =>
                {
                    x.Ivp = ivp;
                    x.Ivr = ivr;
                    x.Ceivp = ce_count / 252;
                    x.Peivp = pe_count / 252;
                    x.Ceivr = Ceivr;
                    x.Peivr = Peivr;
                });

                }
            });
            await db.SaveChangesAsync();

            await Task.WhenAll(db.Ivdata.Where(x => x.UpdatedOn.Date == date.Date)
                .Select(x => redisBL.SetValue($"IV_{x.Symbol}_{date:dd-MM-yyyy}", JsonConvert.SerializeObject(x))));
            return true;

        }

        private async Task<List<DateTime>> GetHolidays()
        {
            var key = "holidays";
            var value = await redisBL.GetValue(key);
            if (value != null)
                return JsonConvert.DeserializeObject<List<Holiday>>(value).Select(x => x.Date).ToList();

            var holidays = await db.Holidays.ToListAsync();
            await redisBL.SetValue(key, JsonConvert.SerializeObject(holidays));
            return holidays.Select(x => x.Date).ToList();

        }
        private async Task<List<DateTime>> GetAllExpiry()
        {
            return await db.Symbols.Where(x => x.Symbol1.StartsWith("NIFTY") && x.Symbol1.EndsWith("PE")).OrderBy(x => x.Expiry).Select(x => x.Expiry.Value).Distinct().OrderBy(x => x).ToListAsync();

        }

        public async Task UpdateMaxPain()
        {
            var allpains = new List<DBModels.MaxPain>();
            var _stocks = await commonBL.GetStocks();
            foreach (var _stock in _stocks)
            {

                var stocks = new List<string> { _stock.DisplayName };
                var touchline = await commonBL.GetTouchline(stocks);

                var symbols = new List<string>();

                var expiries = await commonBL.GetExpiryDates(_stock.CalendarId.Value);
                var expCount = 0;
                foreach (var exp in expiries.ToList())
                {
                    var strikes = await commonBL.GetStrikes(_stock.Name, exp);

                    expCount += 1;
                    foreach (var _strike in strikes)
                    {
                        decimal strike = decimal.Parse(_strike);
                        symbols.Add(_stock.Name + exp.Year.ToString("00").Remove(0, 2) + exp.Month.ToString("00") + exp.Day.ToString("00") + Math.Round(strike) + "CE");
                        symbols.Add(_stock.Name + exp.Year.ToString("00").Remove(0, 2) + exp.Month.ToString("00") + exp.Day.ToString("00") + Math.Round(strike) + "PE");
                    }

                    var pains = new List<MaxPain>();
                    var data = await commonBL.GetTouchline(symbols);

                    strikes.ForEach(_strike =>
                    {
                        decimal strike = decimal.Parse(_strike);
                        var _cedata = data.FirstOrDefault(x => x.Symbol.EndsWith(Math.Round(strike) + "CE"));
                        var _pedata = data.FirstOrDefault(x => x.Symbol.EndsWith(Math.Round(strike) + "PE"));
                        if ((_cedata != null && _cedata.TodayOi != 0) && (_pedata != null && _pedata.TodayOi != 0))
                            pains.Add(new MaxPain
                            {
                                Strike = strike,
                                PEOI = _pedata.TodayOi,
                                CEOI = _cedata.TodayOi,

                            });

                    });

                    var count = 0;
                    pains.ForEach(pain =>
                    {
                        List<decimal> strikes = new();
                        List<decimal> ceois = new();
                        for (int i = 0; i <= count; i++)
                        {
                            strikes.Add(pains[i].Strike);
                            ceois.Add(pains[i].CEOI);
                        }
                        var sp = SumProduct(strikes, ceois);
                        pain.CEValue = (pain.Strike * ceois.Sum()) - sp;
                        count += 1;
                    });
                    pains.Reverse();

                    count = 0;
                    pains.ForEach(pain =>
                    {
                        List<decimal> strikes = new();
                        List<decimal> peois = new();
                        for (int i = 0; i <= count; i++)
                        {
                            strikes.Add(pains[i].Strike);
                            peois.Add(pains[i].PEOI);
                        }
                        var sp = SumProduct(strikes, peois);
                        pain.PEValue = sp - (pain.Strike * peois.Sum());
                        count += 1;
                    });
                    pains.Reverse();

                    List<MaxpainResults> sumois = new();

                    pains.ForEach(x =>
                    {
                        sumois.Add(new MaxpainResults { Value = (x.CEValue + x.PEValue), Strike = x.Strike });
                    });

                    if (!sumois.Any(s => s.Value > 0)) break;
                    var maxpain = sumois.FirstOrDefault(x => x.Value == sumois.Where(s => s.Value != 0).Min(m => m.Value))?.Strike;
                    if (maxpain != null)
                    {
                        if (expCount == 1)
                        {
                            _stock.MaxPain = maxpain;
                            _stock.MaxPainLastUpdatedUtc = DateTime.Now;
                        }
                        allpains.Add(new DBModels.MaxPain
                        {
                            Expiry = exp,
                            MaxPain1 = maxpain ?? 0,
                            Stock = _stock.DisplayName,
                            UpdatedOn = DateTime.Now
                        });
                    }

                }

            };
            db.MaxPains.AddRange(allpains);
            await db.SaveChangesAsync();
        }
        private decimal SumProduct(List<decimal> arrayA, List<decimal> arrayB)
        {
            decimal result = 0;
            for (int i = 0; i < arrayA.Count(); i++)
                result += arrayA[i] * arrayB[i];
            return result;
        }
        public async Task UpdateRedisFOREODDATA()
        {
            List<string> symbols = new()
            {
                "DJI", "NDX", "IXIC", "GSPC", "NYA", "RUT", "VIX", "GSPTSE", "BVSP", "MXX", "FTSE", "FCHI", "IMOEX", "GDAXI", "BEL20", "1", "HSI", "N225", "STI", "KLSE", "KOSPI", "TWII", "CASE30", "TA35", "GNRI", "ADI", "TASI", "AXJO", "NZ50",
                "DXY","USD/INR","GBP/INR","EUR/INR","JPY/INR","CNY/INR","USD/JPY","EUR/USD","USD/CNY",
                "BTC/USD","ETH/USD","BNB/USD","XRP","XRP/usd","DOJE/USD","ADA/usd","SOLD/USD","DOT/USD","LTC/usd","AVAX/usd","matic/usd","trx/usd","shib/usd"

            };
            foreach (var item in symbols)
            {
                await globalMarketBL.GetQuote();
            }
            await commonBL.GetFutureRollOver("CURRENT");
            await commonBL.GetFutureRollOver("NEXT");
            await commonBL.GetFutureRollOver("FUTURE");
        }
        public async Task UpdateVolumeCommentary()
        {
            db.Database.SetCommandTimeout(60 * 10);
            await db.Database.ExecuteSqlInterpolatedAsync($"EXEC sProc_UpdateVolumeCommentary");

            await redisBL.DeleteKey($"GetCommentary");
        }
        public async Task UpdateSpotVolumeCommentary()
        {
            db.Database.SetCommandTimeout(60 * 10);
            await db.Database.ExecuteSqlInterpolatedAsync($"EXEC sProc_UpdateSpotVolumeCommentary");

            await redisBL.DeleteKey($"SpotCommentary");
        }
        public async Task UpdateBreadth()
        {
            dbMaster.Database.SetCommandTimeout(60 * 10);
            await db.Database.ExecuteSqlInterpolatedAsync($"EXEC sProc_UpdateBreadth");

            await redisBL.DeleteKey($"Breadth");
        }


        public async Task UpdateEODSegment(DateTime date)
        {
            dbMaster.Database.SetCommandTimeout(60 * 10);
            await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_Segment_DATE @Date={date}");
            var Symbols = db.Segments.Select(x => x.Name).ToArray();
            await Task.WhenAll(Symbols.Select(symbol => redisBL.DeleteKey($"EOD_{symbol}_{date:dd-MM-yyyy}")));
            await Task.WhenAll(Symbols.Select(symbol => redisBL.DeleteKey($"TOUCHLINE_STOCK_{symbol}_{date:dd-MM-yyyy}")));
            await Task.WhenAll(Symbols.Select(symbol => redisBL.DeleteKey($"TOUCHLINE_{symbol}_{date:dd-MM-yyyy}")));
        }
        public async Task UpdateEOD(DateTime date)
        {
            dbMaster.Database.SetCommandTimeout(60 * 10);

            switch (date.Year)
            {
                case 2018:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE2018 @Date={date.Date}");
                        break;
                    }
                case 2019:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE2019 @Date={date.Date}");
                        break;
                    }
                case 2020:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE2020 @Date={date.Date}");
                        break;
                    }
                case 2021:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE2021 @Date={date.Date}");
                        break;
                    }
                case 2022:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE2022 @Date={date.Date}");
                        break;
                    }
                case 2023:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE2023 @Date={date.Date}");
                        break;
                    }
                case 2024:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE2024 @Date={date.Date}");
                        break;
                    }
                case 2025:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE2025 @Date={date.Date}");
                        break;
                    }
                default:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE @Date={date.Date}");
                        break;
                    }
            }

            // await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_UPDATE_EOD_DATE @Date={date.Date}");
            var Symbols = db.Stocks.Select(x => x.Name).ToArray();
            await Task.WhenAll(Symbols.Select(symbol => redisBL.DeleteKey($"EOD_{symbol}_{date:dd-MM-yyyy}")));
            await Task.WhenAll(Symbols.Select(symbol => redisBL.DeleteKey($"TOUCHLINE_STOCK_{symbol}_{date:dd-MM-yyyy}")));
            await Task.WhenAll(Symbols.Select(symbol => redisBL.DeleteKey($"TOUCHLINE_{symbol}_{date:dd-MM-yyyy}")));
        }
        public async Task UpdateSegmentTouchline(DateTime date)
        {
            dbMaster.Database.SetCommandTimeout(60 * 10);
            switch (date.Year)
            {
                case 2018:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_SYNC_SEGMENT_TOUCHLINE_BY_DATE2018 @Date={date.Date}");
                        break;
                    }
                case 2019:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_SYNC_SEGMENT_TOUCHLINE_BY_DATE2019 @Date={date.Date}");
                        break;
                    }
                case 2020:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_SYNC_SEGMENT_TOUCHLINE_BY_DATE2020 @Date={date.Date}");
                        break;
                    }
                case 2021:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_SYNC_SEGMENT_TOUCHLINE_BY_DATE2021 @Date={date.Date}");
                        break;
                    }
                case 2022:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_SYNC_SEGMENT_TOUCHLINE_BY_DATE2022 @Date={date.Date}");
                        break;
                    }
                case 2023:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_SYNC_SEGMENT_TOUCHLINE_BY_DATE2023 @Date={date.Date}");
                        break;
                    }
                case 2024:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_SYNC_SEGMENT_TOUCHLINE_BY_DATE2024 @Date={date.Date}");
                        break;
                    }
                case 2025:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_SYNC_SEGMENT_TOUCHLINE_BY_DATE2025 @Date={date.Date}");
                        break;
                    }
                default:
                    {
                        await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC sproc_SYNC_SEGMENT_TOUCHLINE_BY_DATE @Date={date.Date}");
                        break;
                    }
            }

            var Symbols = db.Stocks.Select(x => x.Name).ToArray();
            await Task.WhenAll(Symbols.Select(symbol => redisBL.DeleteKey($"TOUCHLINE_{symbol}_{date:dd-MM-yyyy}")));
        }
        public async Task UpdateVolatility(DateTime date)
        {
            dbMaster.Database.SetCommandTimeout(60 * 10);
            await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC SPROC_UPDATE_VOLATILITY_BY_DATE @Date={date.Date}");
            var __ = dbMaster.Volatilities.Where(x => x.Date.Date == date.Date)
                .Select(x => $"VOLATILITY_{x.Symbol}_{date:dd-MM-yyyy}").ToList();
            await Task.WhenAll(dbMaster.Volatilities.Where(x => x.Date.Date == date.Date)
                .Select(x => redisBL.SetValue($"VOLATILITY_{x.Symbol}_{date:dd-MM-yyyy}", JsonConvert.SerializeObject(x))));
        }
        public async Task UpdateTouchline(DateTime date)
        {
            dbMaster.Database.SetCommandTimeout(60 * 10);
            await dbMaster.Database.ExecuteSqlInterpolatedAsync($"EXEC SPROC_UPDATE_TOUCHLINEBYDATE @Date={date.Date}");
            var Symbols = db.Stocks.Select(x => x.DisplayName).ToArray();
            await Task.WhenAll(Symbols.Select(symbol => redisBL.DeleteKey($"TOUCHLINE_STOCK_{symbol}_{date:dd-MM-yyyy}")));
            await Task.WhenAll(Symbols.Select(symbol => redisBL.DeleteKey($"TOUCHLINE_{symbol}_{date:dd-MM-yyyy}")));
        }
        public async Task UpdatePCR()
        {
            db.Database.SetCommandTimeout(60 * 10);
            await db.Database.ExecuteSqlInterpolatedAsync($"EXEC SPROC_UPDATE_PCR");
            await db.Database.ExecuteSqlInterpolatedAsync($"EXEC SPROC_UPDATE_PCR_INDEX");
        }




        public void CopyMasterSync()
        {
            using MasterdataDbContext dbMaster = new();
            dbMaster.Database.SetCommandTimeout(900);
            dbMaster.Database.ExecuteSqlInterpolated($"EXEC SPROC_MASTERSYNCDATA");
        }

        public async Task UpdateFutureRollover()
        {

            var current = await commonBL.GetFutureRollOver("CURRENT");
            var future = await commonBL.GetFutureRollOver("FUTURE");
            var next = await commonBL.GetFutureRollOver("NEXT");
            var alreadyRollOvers = await dbMaster.FutureRollovers.ToListAsync();
            var RollOvers = new List<FutureRollover>();
            foreach (var item in current.Select(x => new { x.Symbol, x.Date }))
            {

                var _symbol = item.Symbol.Remove(item.Symbol.LastIndexOf('-'));


                var _current = current.Where(x => x.Symbol == $"{_symbol}-I" && x.Date.Month == item.Date.Month && x.Date.Year == item.Date.Year).FirstOrDefault();
                var _next = next.Where(x => x.Symbol == $"{_symbol}-II" && x.Date.Month == item.Date.Month && x.Date.Year == item.Date.Year).FirstOrDefault();
                var _future = future.Where(x => x.Symbol == $"{_symbol}-III" && x.Date.Month == item.Date.Month && x.Date.Year == item.Date.Year).FirstOrDefault();
                if ((_current.TodayOI + _future.TodayOI + _next.TodayOI) != 0)
                {
                    var rollover = (_next.TodayOI + _future.TodayOI) * 100 / (_current.TodayOI + _future.TodayOI + _next.TodayOI);
                    if (!(RollOvers.Any(x => x.Month == item.Date.Month && x.Year == item.Date.Year && x.Symbol == _symbol) || alreadyRollOvers.Any(x => x.Month == item.Date.Month && x.Year == item.Date.Year && x.Symbol == _symbol)))
                        RollOvers.Add(new FutureRollover
                        {
                            Year = item.Date.Year,
                            Month = item.Date.Month,
                            Rollover = rollover,
                            Symbol = _symbol,
                        });
                }

            }
            dbMaster.FutureRollovers.AddRange(RollOvers);
            await dbMaster.SaveChangesAsync();
        }

        private class MaxpainResults
        {
            public decimal Strike { get; set; }
            public decimal Value { get; set; }
        }
        private class MaxPain
        {
            public decimal CEOI { get; set; }
            public decimal CEValue { get; set; }
            public decimal PEOI { get; set; }
            public decimal PEValue { get; set; }
            public decimal Total { get; set; }
            public decimal Strike { get; set; }
        }
    }
}
