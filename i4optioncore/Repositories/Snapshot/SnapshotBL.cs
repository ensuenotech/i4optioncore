using i4optioncore.DBModels;
using i4optioncore.DBModelsMaster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static i4optioncore.Models.CommonModel;

namespace i4optioncore.Repositories.Snapshot
{
    public class SnapshotBL : ISnapshotBL
    {
        private readonly IRedisBL redisBL;
        private readonly i4option_dbContext db;
        private readonly MasterdataDbContext dbMaster;
        public SnapshotBL(IRedisBL redisBL, i4option_dbContext db, MasterdataDbContext dbMaster)
        {
            this.redisBL = redisBL;
            this.db = db;
            this.dbMaster = dbMaster;
        }

        public async Task<List<HistoryRecord>> GetOptionIntradaySnapshot(string displayName, DateTime time, DateTime expiry)
        {
            var stockName = db.Stocks.FirstOrDefault(s => s.DisplayName == displayName)?.Name;
            string expiryString = expiry.ToString("yyMMdd");
            return await db.HistorySubscriptions.Where(h => h.LastTradeTime.Day == time.Day
           && h.LastTradeTime.Hour == time.Hour
           && h.LastTradeTime.Minute == time.Minute
           && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
        public async Task<List<HistoryRecord>> GetIntradaySnapshots(List<string> symbols, DateTime time)
        {

            return await db.HistorySubscriptions.Where(h => h.LastTradeTime.Day == time.Day
           && h.LastTradeTime.Hour == time.Hour
           && h.LastTradeTime.Minute == time.Minute
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


            var results = new List<HistoryRecord>();
            var _snapshots = await Task.WhenAll(symbols.Select(displayName => redisBL.GetValue($"Snapshot_{displayName}_{time:dd-MM-yyyy HH:mm}")));
            foreach (var snapshot in _snapshots.Where(t => t != null))
            {
                try
                {
                    if (snapshot != null)
                    {
                        var _t = JsonConvert.DeserializeObject<List<HistoryRecord>>(snapshot).FirstOrDefault();
                        if (_t != null)
                            results.Add(_t);
                    }
                }
                catch (Exception ex)
                {
                    var __ = results;
                }
            }
            var newLines = symbols.Where(x => !results.Where(t => t != null).Select(t => t.Symbol).Contains(x)).ToList();
            foreach (var snapshot in newLines.Where(t => t != null))
            {
                try
                {
                    if (snapshot != null)
                    {
                        var _t = await GetIntradaySnapshot(snapshot, time);
                        var t = _t.FirstOrDefault();
                        if (t != null)
                            results.Add(t);
                    }
                }
                catch (Exception ex)
                {
                    var __ = results;
                }
            }
            return results;
        }
        public async Task<List<HistoryRecord>> GetIntradaySnapshot(string displayName, DateTime time)
        {

            var key = $"Snapshot_{displayName}_{time:dd-MM-yyyy HH:mm}";
            var value = await redisBL.GetValue(key);
            if (value != null)
            {
                var result = JsonConvert.DeserializeObject<List<HistoryRecord>>(value);
                if (result.Count != 0)
                    return result;
            }

            var _result = await db.HistorySubscriptions.Where(h => h.LastTradeTime.Day == time.Day
           && h.LastTradeTime.Hour == time.Hour
           && h.LastTradeTime.Minute == time.Minute
           && h.Symbol == displayName).Select(h => new HistoryRecord
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
            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));

            return _result;
        }
        public async Task<List<HistoryRecord>> GetHistorySnapshot(string displayName, DateTime time)
        {
            var key = $"Snapshot_{displayName}_{time:dd-MM-yyyy HH:mm}";
            var value = await redisBL.GetValue(key);
            if (value != null)
            {
                var result = JsonConvert.DeserializeObject<List<HistoryRecord>>(value);
                if (result.Count != 0)
                    return result;
            }
            var _result = new List<HistoryRecord>();

            if (time.Year.Equals(2023))
            {
                if (time.Month == 1)
                {
                    _result = await dbMaster.History012023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                    _result = await dbMaster.History022023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 3)
                {
                    _result = await dbMaster.History032023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 4)
                {
                    _result = await dbMaster.History042023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 5)
                {
                    _result = await dbMaster.History052023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 6)
                {
                    _result = await dbMaster.History062023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 7)
                {
                    _result = await dbMaster.History072023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 8)
                {
                    _result = await dbMaster.History082023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 9)
                {
                    _result = await dbMaster.History092023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 10)
                {
                    _result = await dbMaster.History102023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                    _result = await dbMaster.History112023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 12)
                {
                    _result = await dbMaster.History122023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
            }
            else if (time.Year.Equals(2022))
            {
                if (time.Month == 1)
                {
                    _result = await dbMaster.History012022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                    _result = await dbMaster.History022022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 3)
                {
                    _result = await dbMaster.History032022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 4)
                {
                    _result = await dbMaster.History042022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 5)
                {
                    _result = await dbMaster.History052022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 6)
                {
                    _result = await dbMaster.History062022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 7)
                {
                    _result = await dbMaster.History072022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 8)
                {
                    _result = await dbMaster.History082022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 9)
                {
                    _result = await dbMaster.History092022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 10)
                {
                    _result = await dbMaster.History102022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                    _result = await dbMaster.History112022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 12)
                {
                    _result = await dbMaster.History122022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
            }
            else if (time.Year.Equals(2021))
            {
                if (time.Month == 1)
                {
                    _result = await dbMaster.History012021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                    _result = await dbMaster.History022021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 3)
                {
                    _result = await dbMaster.History032021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 4)
                {
                    _result = await dbMaster.History042021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 5)
                {
                    _result = await dbMaster.History052021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 6)
                {
                    _result = await dbMaster.History062021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 7)
                {
                    _result = await dbMaster.History072021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 8)
                {
                    _result = await dbMaster.History082021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 9)
                {
                    _result = await dbMaster.History092021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 10)
                {
                    _result = await dbMaster.History102021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                    _result = await dbMaster.History112021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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
                else if (time.Month == 12)
                {
                    _result = await dbMaster.History122021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol == displayName).Select(h => new HistoryRecord
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

            }

            //_result = await dbMaster.HistorySubscriptions.Where(h => h.LastTradeTime.Year == time.Year
            //&& h.LastTradeTime.Month == time.Month
            //&& h.LastTradeTime.Day == time.Day
            //&& h.LastTradeTime.Hour == time.Hour
            //&& h.LastTradeTime.Minute == time.Minute
            //&& h.Symbol == displayName).Select(h => new HistoryRecord
            //{
            //    Symbol = h.Symbol,
            //    Atp = h.Atp,
            //    Close = h.Close,
            //    High = h.High,
            //    LastTradeTime = h.LastTradeTime,
            //    Low = h.Low,
            //    Open = h.Open,
            //    Interval = 1,
            //    OpenInterest = h.OpenInterest,
            //    Volume = h.Volume,
            //    TotalVolume = h.TotalVolume ?? 0

            //}).ToListAsync();
            if (_result.Count > 0)
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));

            return _result;
        }
        public async Task<List<HistoryRecord>> GetOptionHistorySnapshot(string displayName, DateTime time, DateTime expiry)
        {
            var stockName = db.Stocks.FirstOrDefault(s => s.DisplayName == displayName)?.Name;
            string expiryString = expiry.ToString("yyMMdd");

            if (time.Year.Equals(2023))
            {
                if (time.Month == 1)
                {
                    return await dbMaster.History012023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History022023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History032023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History042023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History052023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History062023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History072023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History082023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History092023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History102023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History112023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History122023s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History012022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History022022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History032022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History042022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History052022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History062022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History072022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History082022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History092022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History102022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History112022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History122022s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History012021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History022021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History032021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History042021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History052021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History062021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History072021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History082021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History092021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History102021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History112021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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
                    return await dbMaster.History122021s.Where(h => h.LastTradeTime.Day == time.Day
            && h.LastTradeTime.Hour == time.Hour
            && h.LastTradeTime.Minute == time.Minute
            && h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
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

            //return await dbMaster.HistorySubscriptions.Where(h =>
            //h.LastTradeTime.Year == time.Year
            //&& h.LastTradeTime.Month == time.Month
            //&& h.LastTradeTime.Day == time.Day
            //&& h.LastTradeTime.Hour == time.Hour
            //&& h.LastTradeTime.Minute == time.Minute
            //&& h.Symbol.StartsWith(stockName) && h.Symbol.Contains(expiryString) && (h.Symbol.EndsWith("CE") || h.Symbol.EndsWith("PE"))).Select(h => new HistoryRecord
            //{
            //    Symbol = h.Symbol,
            //    Atp = h.Atp,
            //    Close = h.Close,
            //    High = h.High,
            //    LastTradeTime = h.LastTradeTime,
            //    Low = h.Low,
            //    Open = h.Open,
            //    Interval = 1,
            //    OpenInterest = h.OpenInterest,
            //    Volume = h.Volume
            //}).ToListAsync();
            return null;
        }
    }
}
