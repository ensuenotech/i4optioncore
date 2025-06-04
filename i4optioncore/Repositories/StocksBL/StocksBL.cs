using i4optioncore.DBModelsMaster;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using i4optioncore.DBModels;
using i4optioncore.Models;
using Microsoft.Data.SqlClient;
using System.Data;
//using Twilio.Http;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Connections.Features;
using System.Runtime.InteropServices;
using Azure.Core;
using System.Text.Json;
using static i4optioncore.Models.CommonModel;

namespace i4optioncore.Repositories
{
    public class StocksBL : IStocksBL
    {
        readonly MasterdataDbContext dbMaster;
        readonly i4option_dbContext db;
        readonly IRedisBL redisBL;
        readonly ICommonBL commonBL;
        private readonly ICacheBL cacheBL;
        public StocksBL(MasterdataDbContext dbMaster, IRedisBL redisBL, i4option_dbContext _db, ICommonBL commonBL, ICacheBL cacheBL)
        {
            db = _db;
            this.dbMaster = dbMaster;
            this.redisBL = redisBL;
            this.commonBL = commonBL;
            this.cacheBL = cacheBL;
        }
        public async Task<List<Eod>> GetEods(DateTime date, List<string> Symbols)
        {
            var key = $"EOD_{date:dd-MM-yyyy}_{string.Join("_", Symbols)}";
            if (!string.IsNullOrEmpty(cacheBL.GetValue(key)))
            {
                return JsonConvert.DeserializeObject<List<Eod>>(cacheBL.GetValue(key));
            }
            var result = await dbMaster.Eods.Where(x => x.Date.Date.Equals(date.Date) && Symbols.Contains(x.Stock)).ToListAsync();
            if (result.Count > 0)
                cacheBL.SetValue(key, JsonConvert.SerializeObject(result));

            return result;

            //var eods = new List<Eod>();
            //var _eods = await Task.WhenAll(Symbols.Select(symbol => redisBL.GetValue($"EOD_{symbol}_{date:dd-MM-yyyy}")));
            //foreach (var eod in _eods.Where(t => t != null))
            //{
            //    try
            //    {
            //        if (eod != null)
            //        {
            //            var _t = JsonConvert.DeserializeObject<Eod>(eod);
            //            eods.Add(_t);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}

            //var newSymbols = Symbols.Where(x => !eods.Where(t => t.Stock == x).Select(t => t.Stock).Contains(x)).ToList();
            //if (newSymbols.Any())
            //{
            //    await UpdateEODByDate(date, newSymbols);
            //    var _newEods = await Task.WhenAll(newSymbols.Select(symbol => redisBL.GetValue($"EOD_{symbol}_{date:dd-MM-yyyy}")));
            //    foreach (var eod in _newEods.Where(t => t != null))
            //    {
            //        try
            //        {
            //            if (eod != null)
            //            {
            //                var _t = JsonConvert.DeserializeObject<Eod>(eod);
            //                eods.Add(_t);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //        }
            //    }
            //}
            //return eods.ToList();
        }
        public List<FOIG> GetFOIG(FOIGRequest request)
        {
            var results = new List<FOIG>();
            var param = new SqlParameter[]
            {
                new SqlParameter()
                {
                    ParameterName = "Interval",
                    SqlDbType=SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = request.Interval
                },
                new SqlParameter()
                {
                    ParameterName = "Type",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = request.Type
                }
            };
            using var connection = new SqlConnection(db.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sPROC_GET_FOIG]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new FOIG
                    {
                        OIChangePercentage = reader.IsDBNull("OIChangePercentage") ? 0 : reader.GetDecimal("OIChangePercentage"),
                        PriceChangePercentage = reader.IsDBNull("PriceChangePercentage") ? 0 : reader.GetDecimal("PriceChangePercentage"),
                        Symbol = reader.GetString("Symbol"),
                    });
                }
            }
            connection.Close();
            return results;
        }
        public ICollection<OptionDashboard> GetOptionDashboards()
        {
            var results = new List<OptionDashboard>();

            using var connection = new SqlConnection(db.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[SPROC_GETOPTION_DASHBOARD]";
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new OptionDashboard
                    {
                        ATM = reader.IsDBNull("ATM") ? 0 : reader.GetInt32("ATM"),
                        FutPriceChangePercentage = reader.IsDBNull("FUTPriceChangePercentage") ? 0 : reader.GetDecimal("FUTPriceChangePercentage"),
                        FutPriceChange = reader.IsDBNull("FUTPriceChange") ? 0 : reader.GetDecimal("FUTPriceChange"),
                        FutLTP = reader.IsDBNull("FUTLTP") ? 0 : reader.GetDecimal("FUTLTP"),
                        SpotPrice = reader.IsDBNull("SpotPrice") ? 0 : reader.GetDecimal("SpotPrice"),
                        PreviousClose = reader.IsDBNull("PreviousClose") ? 0 : reader.GetDecimal("PreviousClose"),
                        FutPreviousClose = reader.IsDBNull("FUTPreviousClose") ? 0 : reader.GetDecimal("FUTPreviousClose"),
                        CEStrikeSymbol = reader.IsDBNull("CEStrikeSymbol") ? string.Empty : reader.GetString("CEStrikeSymbol"),
                        PEStrikeSymbol = reader.IsDBNull("PEStrikeSymbol") ? string.Empty : reader.GetString("PEStrikeSymbol"),
                        FutSymbolId = reader.IsDBNull("FUTSymbolId") ? 0 : reader.GetInt32("FUTSymbolId"),
                        SymbolId = reader.IsDBNull("SymbolId") ? 0 : reader.GetInt32("SymbolId"),
                        LatestExpiry = reader.GetDateTime("LatestExpiry"),
                        Symbol = reader.IsDBNull("Symbol") ? string.Empty : reader.GetString("Symbol"),
                        FUTSymbol = reader.IsDBNull("FUTSymbol") ? string.Empty : reader.GetString("FUTSymbol"),
                        MaxPain = reader.IsDBNull("MaxPain") ? 0 : reader.GetDecimal("MaxPain"),
                        MPD = reader.IsDBNull("MPD") ? 0 : reader.GetDecimal("MPD"),
                        MinIV = reader.IsDBNull("MINIV") ? 0 : reader.GetDecimal("MINIV"),
                        MaxIV = reader.IsDBNull("MAXIV") ? 0 : reader.GetDecimal("MAXIV"),
                        OIPCR = reader.IsDBNull("OIPCR") ? 0 : reader.GetDecimal("OIPCR"),
                    });
                }
            }
            connection.Close();
            return results;
        }

        public async Task<ICollection<IIntradayPCR>> GetPcrByInterval(GetPCRRequest request)
        {
            var key = $"PCRInterval_{request.Symbol}_{request.Expiry:dd-MM-yyyy}_{request.Date:dd-MM-yyyy}";

            var value = await redisBL.GetValue(key);

            if (value != null)
            {
                return JsonConvert.DeserializeObject<ICollection<IIntradayPCR>>(value);
            }
            var results = dbMaster.Pcrs.Where(x => x.Stock.ToLower().Equals(request.Symbol.ToLower())
            && x.Date.Date == (request.Date ?? DateTime.Now).Date
            && x.Expiry.Date == request.Expiry.Date).Select(x => new IIntradayPCR
            {
                CEOI = x.Ceoi ?? 0,
                CEVolume = x.Cevolume ?? 0,
                LastTradeTime = x.Date,
                OIPCR = x.Oipcr ?? 0,
                OISum = x.Oisum ?? 0,
                PEOI = x.Peoi ?? 0,
                PEVolume = x.Pevolume ?? 0,
                TotalVolumeSum = x.TotalVolumeSum ?? 0,
                VolumePCR = x.VolumePcr ?? 0
            }).ToList();


            bool afterMarket = false;
            if ((DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 15 || DateTime.Now.Hour < 9 || (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 15))
                afterMarket = true;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                afterMarket = true;
            var holidays = await commonBL.GetHolidays();
            if (holidays.Any(h => h.Date == DateTime.Now.Date)) afterMarket = true;
            if (results.Count == 0)
                return results;
            if (afterMarket)
            {
                var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Hour < 9 ? DateTime.Now.Day : DateTime.Now.Day + 1, 9, 15, 0);
                await redisBL.SetValueWithExpiry(key, JsonConvert.SerializeObject(results), (startTime - DateTime.Now).TotalMinutes);
            }
            else
                await redisBL.SetValueWithExpiry(key, JsonConvert.SerializeObject(results), 1);
            return results;

        }

        public List<IIntradayPCR> GetPcrByDate(IPCRByDateRequest request)
        {
            var results = new List<IIntradayPCR>();

            var param = new SqlParameter[]
          {
                new SqlParameter()
                {
                    ParameterName = "Symbol",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = request.Symbol
                },
                new SqlParameter()
                {
                    ParameterName = "Expiry",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = request.Expiry
                },
                new SqlParameter()
                {
                    ParameterName = "FromDate",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = request.FromDate
                } ,
              new SqlParameter()
                {
                    ParameterName = "ToDate",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = request.ToDate
                }
          };
            using var connection = new SqlConnection(dbMaster.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sproc_GET_PCR]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    if (!reader.IsDBNull("CEOI"))
                    {
                        try
                        {
                            results.Add(new IIntradayPCR
                            {
                                LastTradeTime = Convert.ToDateTime(reader["Date"]),
                                CEOI = Convert.ToInt64(reader["CEOI"]),
                                PEOI = Convert.ToInt64(reader["PEOI"]),
                                CEVolume = Convert.ToInt64(reader["CEVolume"]),
                                PEVolume = Convert.ToInt64(reader["PEVolume"]),
                                VolumePCR = Convert.ToDecimal(reader["VolumePCR"]),
                                OIPCR = Convert.ToDecimal(reader["OIPCR"]),
                            });
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            connection.Close();
            //if (results.Count == 0) return results;
            //results.ForEach(pcr => dbMaster.DayPcrs.Add(new DayPcr
            //{
            //    Ceoi = pcr.CEOI,
            //    Cevolume = pcr.CEVolume,
            //    Date = date,
            //    Expiry = expiry,
            //    Stock = symbol,
            //    Oipcr = pcr.OIPCR,
            //    Oisum = pcr.OISum,
            //    Peoi = pcr.PEOI,
            //    Pevolume = pcr.PEVolume,
            //    TotalVolumeSum = pcr.TotalVolumeSum,
            //    VolumePcr = pcr.VolumePCR
            //}));
            //await dbMaster.SaveChangesAsync();
            // await redisBL.SetValue(key, JsonConvert.SerializeObject(results));

            return results;

        }
        public async Task<List<IIntradayPCR>> GetIntradayPCR(string symbol, DateTime expiry)
        {
            var results = new List<IIntradayPCR>();
            var key = $"PCRIntraday_{symbol}_{expiry:dd-MM-yyyy}";

            var value = await redisBL.GetValue(key);

            if (value != null)
            {
                return JsonConvert.DeserializeObject<List<IIntradayPCR>>(value);
            }

            var param = new SqlParameter[]
        {
                new SqlParameter()
                {
                    ParameterName = "Symbol",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = symbol
                },
                new SqlParameter()
                {
                    ParameterName = "Expiry",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = expiry
                }
        };
            using var connection = new SqlConnection(db.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sproc_GET_INTRADAY_PCR]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new IIntradayPCR
                    {
                        LastTradeTime = Convert.ToDateTime(reader["LastTradeTime"]),
                        CEOI = Convert.ToInt64(reader["CEOI"]),
                        PEOI = Convert.ToInt64(reader["PEOI"]),
                        CEVolume = Convert.ToInt64(reader["CEVolume"]),
                        PEVolume = Convert.ToInt64(reader["PEVolume"]),
                        VolumePCR = Convert.ToDecimal(reader["VolumePCR"]),
                        OIPCR = Convert.ToDecimal(reader["OIPCR"]),
                        OISum = Convert.ToInt64(reader["OISum"]),
                        TotalVolumeSum = Convert.ToInt64(reader["TotalVolumeSum"]),
                    });
                }
            }
            connection.Close();

            bool afterMarket = false;
            if ((DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 15 || DateTime.Now.Hour < 9 || (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 15))
                afterMarket = true;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                afterMarket = true;
            var holidays = await commonBL.GetHolidays();
            if (holidays.Any(h => h.Date == DateTime.Now.Date)) afterMarket = true;
            if (afterMarket)
            {
                var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Hour < 9 ? DateTime.Now.Day : DateTime.Now.Day + 1, 9, 15, 0);
                await redisBL.SetValueWithExpiry(key, JsonConvert.SerializeObject(results), (startTime - DateTime.Now).TotalMinutes);
            }
            else
                await redisBL.SetValueWithExpiry(key, JsonConvert.SerializeObject(results), 1);

            return results;
        }

        public List<StocksModel.HistoricPerformance> GetHistoricPerformances(string Stock)
        {
            var results = new List<StocksModel.HistoricPerformance>();
            var param = new SqlParameter[]
            {
                new SqlParameter()
                {
                    ParameterName = "Stock",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = Stock
                }
            };
            using var connection = new SqlConnection(dbMaster.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sproc_GET_HistoricPerformance]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new StocksModel.HistoricPerformance
                    {
                        Change = reader.GetDecimal("Change"),
                        Month = reader.GetString("Month"),
                        Year = reader.GetString("Year"),
                    });
                }
            }
            connection.Close();
            return results;
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetOptionTouchline(string stockName, DateTime date)
        {
            string key = $"OPTIONSTOUCHLINENEW_{stockName}_{date:dd-MM-yyyy}";
            var value = await redisBL.GetValue(key);
            if (value != null)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<TouchlineSubscriptionDetails>>(value.ToString());
            }

            var _stockName = db.Stocks.Where(x => x.DisplayName == stockName.Trim()).FirstOrDefault().Name;

            //var symbols = await db.Symbols.Where(x => x.Symbol1 == stockName || (x.Symbol1.StartsWith(_stockName) && (x.Series == "CE" || x.Series == "PE" || x.Series == "XX"))).Select(x => x.Symbol1).Distinct().OrderBy(x => x).ToListAsync();


            var result = await dbMaster.TouchlineSubscriptions.Where(x => x.LastUpdatedTime.Date == date.Date
            //&& x.Ltp != 0 
            && ((x.Symbol.StartsWith(_stockName) && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))) || x.Symbol == stockName)
            //&& symbols.Contains(x.Symbol)
            ).Select(x => new TouchlineSubscriptionDetails
            {
                Atp = x.Atp,
                High = x.High,
                LastUpdatedTime = x.LastUpdatedTime,
                Low = x.Low,
                Ltp = x.Ltp,
                Open = x.Open,
                PreviousClose = x.PreviousClose,
                PreviousOiclose = x.PreviousOiclose,
                Symbol = x.Symbol,
                SymbolId = x.SymbolId,
                TickVolume = x.TickVolume,
                TodayOi = x.TodayOi,
                TotalVolume = x.TotalVolume,
                TurnOver = x.TurnOver,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).Distinct().OrderBy(x => x.LastUpdatedTime).ToListAsync();
            await redisBL.SetValue(key, System.Text.Json.JsonSerializer.Serialize(result));
            return result;
        }
        public async Task<List<HistoryRecord>> GetOptionHistory(string stockName, DateTime date)
        {
            string key = $"OPTIONSHISTORYNEW_{stockName}_{date:dd-MM-yyyy}";
            var value = await redisBL.GetValue(key);
            if (value != null)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<HistoryRecord>>(value.ToString());
            }

            var results = new List<HistoryRecord>();
            var param = new SqlParameter[]
            {
                new SqlParameter()
                {
                    ParameterName = "Stock",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = stockName
                },
                new SqlParameter()
                {
                    ParameterName = "Date",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = date
                }
            };
            using var connection = new SqlConnection(dbMaster.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sproc_GET_INTRADAYHISTORY]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new HistoryRecord
                    {
                        LastTradeTime = Convert.ToDateTime(reader["LastTradeTime"]),
                        Symbol = reader.GetString("Symbol"),
                        Open = reader.IsDBNull("Open") ? 0 : reader.GetDecimal("Open"),
                        High = reader.IsDBNull("High") ? 0 : reader.GetDecimal("High"),
                        Low = reader.IsDBNull("Low") ? 0 : reader.GetDecimal("Low"),
                        Close = reader.IsDBNull("Close") ? 0 : reader.GetDecimal("Close"),
                        Volume = reader.IsDBNull("Volume") ? 0 : reader.GetInt32("Volume"),
                        OpenInterest = reader.IsDBNull("OpenInterest") ? 0 : reader.GetInt32("OpenInterest"),
                        Atp = reader.IsDBNull("ATP") ? 0 : reader.GetDecimal("ATP"),
                        TotalVolume = reader.IsDBNull("TotalVolume") ? 0 : reader.GetInt64("TotalVolume"),
                    });
                }
            }
            connection.Close();
            await redisBL.SetValue(key, System.Text.Json.JsonSerializer.Serialize(results));
            return results;
        }
        public async Task<List<HistoryRecord>> GetHistory(List<string> symbols, DateTime time, string type = "daily")
        {
            dbMaster.Database.SetCommandTimeout(100000);
            if (time.Year.Equals(2025))
            {
                if (time.Month == 1)
                {
                    return await dbMaster.History012025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)
            && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
            {
                Symbol = h.Symbol,
                Atp = h.Atp,
                Close = h.Close,
                High = h.High,
                LastTradeTime = h.LastTradeTime,
                Low = h.Low,
                Open = h.Open,
                Interval = 1,
                OpenInterest = h.OpenInterest,
                Volume = h.Volume,
                TotalVolume = h.TotalVolume ?? 0

            }).ToListAsync();
                }
                else if (time.Month == 2)
                {
                    return await dbMaster.History022025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 3)
                {
                    return await dbMaster.History032025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 4)
                {
                    return await dbMaster.History042025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 5)
                {
                    return await dbMaster.History052025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 6)
                {
                    return await dbMaster.History062025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 7)
                {
                    return await dbMaster.History072025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 8)
                {
                    return await dbMaster.History082025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 9)
                {
                    return await dbMaster.History092025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 10)
                {
                    return await dbMaster.History102025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 11)
                {
                    return await dbMaster.History112025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 12)
                {
                    return await dbMaster.History122025s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
            }
            else if (time.Year.Equals(2024))
            {
                if (time.Month == 1)
                {
                    return await dbMaster.History012024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)
            && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
            {
                Symbol = h.Symbol,
                Atp = h.Atp,
                Close = h.Close,
                High = h.High,
                LastTradeTime = h.LastTradeTime,
                Low = h.Low,
                Open = h.Open,
                Interval = 1,
                OpenInterest = h.OpenInterest,
                Volume = h.Volume,
                TotalVolume = h.TotalVolume ?? 0

            }).ToListAsync();
                }
                else if (time.Month == 2)
                {
                    return await dbMaster.History022024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 3)
                {
                    return await dbMaster.History032024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 4)
                {
                    return await dbMaster.History042024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 5)
                {
                    return await dbMaster.History052024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 6)
                {
                    return await dbMaster.History062024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 7)
                {
                    return await dbMaster.History072024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 8)
                {
                    return await dbMaster.History082024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 9)
                {
                    return await dbMaster.History092024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 10)
                {
                    return await dbMaster.History102024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 11)
                {
                    return await dbMaster.History112024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 12)
                {
                    return await dbMaster.History122024s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
            }
            else if (time.Year.Equals(2023))
            {
                if (time.Month == 1)
                {
                    return await dbMaster.History012023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)

            && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
            {
                Symbol = h.Symbol,
                Atp = h.Atp,
                Close = h.Close,
                High = h.High,
                LastTradeTime = h.LastTradeTime,
                Low = h.Low,
                Open = h.Open,
                Interval = 1,
                OpenInterest = h.OpenInterest,
                Volume = h.Volume,
                TotalVolume = h.TotalVolume ?? 0

            }).ToListAsync();
                }
                else if (time.Month == 2)
                {
                    return await dbMaster.History022023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 3)
                {
                    return await dbMaster.History032023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 4)
                {
                    return await dbMaster.History042023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 5)
                {
                    return await dbMaster.History052023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 6)
                {
                    return await dbMaster.History062023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 7)
                {
                    return await dbMaster.History072023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 8)
                {
                    return await dbMaster.History082023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 9)
                {
                    return await dbMaster.History092023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 10)
                {
                    return await dbMaster.History102023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 11)
                {
                    return await dbMaster.History112023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 12)
                {
                    return await dbMaster.History122023s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
            }
            else if (time.Year.Equals(2022))
            {
                if (time.Month == 1)
                {
                    return await dbMaster.History012022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,
                 TotalVolume = h.TotalVolume ?? 0

             }).ToListAsync();
                }
                else if (time.Month == 2)
                {
                    return await dbMaster.History022022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 3)
                {
                    return await dbMaster.History032022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 4)
                {
                    return await dbMaster.History042022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 5)
                {
                    return await dbMaster.History052022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 6)
                {
                    return await dbMaster.History062022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 7)
                {
                    return await dbMaster.History072022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 8)
                {
                    return await dbMaster.History082022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 9)
                {
                    return await dbMaster.History092022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 10)
                {
                    return await dbMaster.History102022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 11)
                {
                    return await dbMaster.History112022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 12)
                {
                    return await dbMaster.History122022s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
            }
            else if (time.Year.Equals(2021))
            {
                if (time.Month == 1)
                {
                    return await dbMaster.History012021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)
             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 2)
                {
                    return await dbMaster.History022021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 3)
                {
                    return await dbMaster.History032021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 4)
                {
                    return await dbMaster.History042021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 5)
                {
                    return await dbMaster.History052021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 6)
                {
                    return await dbMaster.History062021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 7)
                {
                    return await dbMaster.History072021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 8)
                {
                    return await dbMaster.History082021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 9)
                {
                    return await dbMaster.History092021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 10)
                {
                    return await dbMaster.History102021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 11)
                {
                    return await dbMaster.History112021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }
                else if (time.Month == 12)
                {
                    return await dbMaster.History122021s.Where(h => (type == "monthly" || h.LastTradeTime.Day == time.Day)


             && symbols.Contains(h.Symbol)).Select(h => new HistoryRecord
             {
                 Symbol = h.Symbol,
                 Atp = h.Atp,
                 Close = h.Close,
                 High = h.High,
                 LastTradeTime = h.LastTradeTime,
                 Low = h.Low,
                 Open = h.Open,
                 Interval = 1,
                 OpenInterest = h.OpenInterest,
                 Volume = h.Volume,

             }).ToListAsync();
                }

            }
            return null;
        }
        public async Task<List<FutureRollover>> GetFutureRollOver()
        {
            return await dbMaster.FutureRollovers.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToListAsync();
        }
    }
}
