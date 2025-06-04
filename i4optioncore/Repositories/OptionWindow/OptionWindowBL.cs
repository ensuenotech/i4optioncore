using i4optioncore.DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static i4optioncore.Models.CommonModel;

namespace i4optioncore.Repositories.OptionWindow
{
    public class OptionWindowBL:IOptionWindowBL
    {
        private readonly ICommonBL commonBL;
        private readonly i4option_dbContext db;


        public OptionWindowBL(ICommonBL commonBL, i4option_dbContext db)
        {
            this.commonBL = commonBL;
            this.db = db;
        }
        #region Stocks
        public async Task<List<TouchlineSubscriptionDetails>> GetStocksActive()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);
            return await db.TouchlineSubscriptions.Where(x =>
            x.TodayOi > 0 &&
            x.LastUpdatedTime > tradetime
            && !(x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.TurnOver).Take(40).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetStocksFarActivity()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);
            var expiries = db.CalendarDates.Where(x => x.Date > DateTime.Now.AddDays(45)).Select(x => x.Date.ToString("yyMMdd")).ToArray();
            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime &&
            !(x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.TotalVolume * x.Atp) > 100000 && x.TodayOi > 20000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.TurnOver).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetStocksOH()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && !(x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.Open == x.High)
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetStocksOL()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && !(x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.Open == x.Low)
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetStocksOIGainer()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && !(x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
           && (x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0) > 10
            && (x.Atp * x.TotalVolume) > 1000000 && x.TodayOi > 50000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetStocksOILooser()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && !(x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0) < -10
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderBy(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetStocksBuyers()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && !(x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
           && (x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0) > 0
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderBy(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetStocksWriters()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && !(x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0) < 0
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetStocksItmUnwinding()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && !(x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0) < 0
            && (x.Atp * x.TotalVolume) > 1000000 && x.TotalVolume > 10000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        #endregion

        #region  Index
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexActive()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);
            return await db.TouchlineSubscriptions.Where(x =>
            x.TodayOi > 0 &&
            x.LastUpdatedTime > tradetime
            && (x.Symbol.Contains("NIFTY")
            || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.TurnOver).Take(40).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexFarActivity()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);
            var expiries = db.CalendarDates.Where(x => x.Date > DateTime.Now.AddDays(45)).Select(x => x.Date.ToString("yyMMdd")).ToArray();
            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime &&
            (x.Symbol.Contains("NIFTY") || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            //&& (x.Symbol.Length > 8 && expiries.Contains(x.Symbol.Remove(0, 8)))
            && (x.TotalVolume * x.Atp) > 100000 && x.TodayOi > 20000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.TurnOver).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexOH()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Symbol.Contains("NIFTY") || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.Open == x.High)
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexOL()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Symbol.Contains("NIFTY") || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.Open == x.Low)
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexOIGainer()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Symbol.Contains("NIFTY") || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
           && (x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0) > 10
            && (x.Atp * x.TotalVolume) > 1000000 && x.TodayOi > 50000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexOILooser()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Symbol.Contains("NIFTY") || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0) < -10
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderBy(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexBuyers()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Symbol.Contains("NIFTY") || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
           && (x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0) > 0
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.Atp * x.TotalVolume,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderBy(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexWriters()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Symbol.Contains("NIFTY") || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
            && (x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0) < 0
            && (x.Atp * x.TotalVolume) > 1000000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexItmUnwinding()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Symbol.Contains("NIFTY") || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("SENSEX")|| x.Symbol.Contains("BANKEX"))
             && (x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0) < 0
            && (x.Atp * x.TotalVolume) > 1000000 && x.TotalVolume > 10000).Select(x => new TouchlineSubscriptionDetails
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
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.OiChangePercentage).Take(30).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetIndexOHOL()
        {
            var marketholiday = await commonBL.GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);

            var expiries = await commonBL.GetExpiryDates(2);
            string symbolFilter = expiries.FirstOrDefault().Year.ToString("00").Remove(0, 2) + expiries.FirstOrDefault().Month.ToString("00");

            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && x.Symbol.Contains(symbolFilter)
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && (x.Open == x.Low)
            && x.Open > .1m
            && x.TotalVolume > 20000
            && x.TodayOi > 300).Select(x => new TouchlineSubscriptionDetails
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
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).ToListAsync();
        }
        #endregion
    }
}
