using DocumentFormat.OpenXml.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using i4optioncore.DBModels;
using i4optioncore.DBModelsMaster;
using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace i4optioncore.Repositories.EOD
{
    public class EODBL : IEODBL
    {
        readonly private MasterdataDbContext dbMaster;
        readonly private i4option_dbContext db;
        private readonly I4optionUserDbContext dbUser;
        readonly private ICacheBL cacheBL;

        public EODBL(MasterdataDbContext dbMaster, i4option_dbContext db, ICacheBL cacheBL, I4optionUserDbContext dbUser)
        {
            this.dbMaster = dbMaster;
            this.db = db;
            this.cacheBL = cacheBL;
            this.dbUser = dbUser;
        }

        public List<IEODScreenerData> EODScreener(List<IEODScreenerDataRequest> requests)
        {
            var currentYear = DateTime.Now.Year;
            var previousYear = currentYear - 1;

            var todayIVData = new List<Ivdatum>();
            var yesterdayIVData = new List<Ivdatum>();

            var eod = new List<Eod>();
            var previous_eod = new List<Eod>();

            var today = dbMaster.TouchlineSubscriptionsStocks.Select(x => x.LastUpdatedTime.Date).Distinct().OrderByDescending(x => x).FirstOrDefault();
            var yesterday = dbMaster.TouchlineSubscriptionsStocks.Select(x => x.LastUpdatedTime.Date).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefault();
            var daybeforeyesterday = dbMaster.TouchlineSubscriptionsStocks.Select(x => x.LastUpdatedTime.Date).Distinct().OrderByDescending(x => x).Skip(2).FirstOrDefault();
            var daybeforeyesterday1 = dbMaster.TouchlineSubscriptionsStocks.Select(x => x.LastUpdatedTime.Date).Distinct().OrderByDescending(x => x).Skip(3).FirstOrDefault();

            var todayIVDate = db.Ivdata.Select(x => x.UpdatedOn.Date).Distinct().OrderByDescending(x => x).FirstOrDefault();
            var yesterdayIVDate = db.Ivdata.Select(x => x.UpdatedOn.Date).Distinct().OrderByDescending(x => x).Skip(1).FirstOrDefault();

            var stocks = db.Stocks.Where(s => s.Active).ToList();


            var touchline = (from s in stocks
                             join t in dbMaster.TouchlineSubscriptionsStocks
                             on s.DisplayName equals t.Symbol
                             where t.LastUpdatedTime.Date == today
                             orderby t.LastUpdatedTime descending
                             select t).ToList();

            var previous_touchline = (from s in stocks
                                      join t in dbMaster.TouchlineSubscriptionsStocks
                             on s.DisplayName equals t.Symbol
                                      where t.LastUpdatedTime.Date == yesterday
                                      orderby t.LastUpdatedTime descending
                                      select t).ToList();

            var future_touchline = (from t in dbMaster.TouchlineSubscriptions
                                    where t.LastUpdatedTime.Date == today
                                    && t.Symbol.EndsWith("FUT")
                                    orderby t.LastUpdatedTime descending
                                    select t).ToList();
            var daybeforeyesterday_touchline = (from s in stocks
                                                join t in dbMaster.TouchlineSubscriptionsStocks
                                       on s.DisplayName equals t.Symbol
                                                where t.LastUpdatedTime.Date == daybeforeyesterday
                                                orderby t.LastUpdatedTime descending
                                                select t).ToList();
            var daybeforeyesterday1_touchline = (from s in stocks
                                                 join t in dbMaster.TouchlineSubscriptionsStocks
                                        on s.DisplayName equals t.Symbol
                                                 where t.LastUpdatedTime.Date == daybeforeyesterday1
                                                 orderby t.LastUpdatedTime descending
                                                 select t).ToList();

            var today_PCR = (from s in stocks
                             join t in dbMaster.Pcrs
                             on s.DisplayName equals t.Stock
                             where t.Date.Date == today
                             orderby t.Date descending
                             select t).ToList();
            var previous_PCR = (from s in stocks
                                join t in dbMaster.Pcrs
                                on s.DisplayName equals t.Stock
                                where t.Date.Date == yesterday
                                orderby t.Date descending
                                select t).ToList();


            eod = (from s in stocks
                   join t in dbMaster.Eods
                   on s.DisplayName equals t.Stock
                   where t.Date.Date == today
                   orderby t.Date descending
                   select t).ToList();
            previous_eod = (from s in stocks
                            join t in dbMaster.Eods
                            on s.DisplayName equals t.Stock
                            where t.Date.Date == yesterday
                            orderby t.Date descending
                            select t).ToList();

            if (requests.Any(r => r.Column.Contains("iv increase") || r.Column.Contains("iv fall")))
            {

                todayIVData = (from s in stocks
                               join t in db.Ivdata
                               on s.DisplayName equals t.Symbol
                               where t.UpdatedOn.Date == todayIVDate.Date
                               orderby t.UpdatedOn descending
                               select t).ToList();
                yesterdayIVData = (from s in stocks
                                   join t in db.Ivdata
                                   on s.DisplayName equals t.Symbol
                                   where t.UpdatedOn.Date == yesterdayIVDate.Date
                                   orderby t.UpdatedOn descending
                                   select t).ToList();
            }

            var result = new List<IEODScreenerData>();
            foreach (var param in requests)
            {

                string column = param.Column.ToLower(), type = param.Type;
                decimal value = param.Value;
                IEODScreenerENUM condition = param.Condition;

                if (!string.IsNullOrEmpty(column))
                {
                    column = column.ToLower();

                    if (column == "ddel5")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "ddel5";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Ddel5 > value : s.Ddel5 < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, Delivery = g.Select(x => x.Ddel5).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                   ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = _r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "ddel10")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "ddel10";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Ddel10 > value : s.Ddel10 < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, Delivery = g.Select(x => x.Ddel10).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = _r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "ddel20")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "ddel20";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Ddel20 > value : s.Ddel20 < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, Delivery = g.Select(x => x.Ddel20).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                   ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = _r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "davol5")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "davol5";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Davol5 > value : s.Davol5 < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, Volume = g.Select(x => x.Davol5).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = _r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "davol10")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "davol10";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Davol10 > value : s.Davol10 < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, Volume = g.Select(x => x.Davol10).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = _r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "davol20")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "davol20";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Davol20 > value : s.Davol20 < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, Volume = g.Select(x => x.Davol20).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = _r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "delivery")
                    {
                        var res = (from s in touchline
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.DeliverablePercentage > value : s.DeliverablePercentage < value
                                   orderby s.LastUpdatedTime descending
                                   group s by s.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, Delivery = g.Select(x => x.DeliverablePercentage).FirstOrDefault(), Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault() }
                                   ).OrderByDescending(x => x.Date).ToList();


                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = _r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "gap-1")
                    {
                        var res = (from t in touchline
                                   where
                                   condition == IEODScreenerENUM.Greater ? (t.Open - t.PreviousClose) > value : (t.Open - t.PreviousClose) < value
                                   orderby (t.Open - t.PreviousClose) descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       PriceAction = g.Select(x => x.Open - x.PreviousClose).FirstOrDefault().ToString("0.00"),
                                       Date = g.Select(x => x.LastUpdatedTime.Date).FirstOrDefault()
                                   }
                                     ).OrderByDescending(x => x.Date).ToList();

                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "gap-2")
                    {
                        var res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   where
                                   condition == IEODScreenerENUM.Greater ?
                                   (t.Open - t.PreviousClose) > value && (pt.Open - pt.PreviousClose) > value
                                   : (t.Open - t.PreviousClose) < value && (pt.Open - pt.PreviousClose) < value
                                   orderby (t.Open - t.PreviousClose) descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       PriceAction = g.Select(x => x.Open - x.PreviousClose).FirstOrDefault().ToString("0.00"),
                                       Date = g.Select(x => x.LastUpdatedTime.Date).FirstOrDefault()
                                   }
                                     ).OrderByDescending(x => x.Date).ToList();

                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "gap-3")
                    {
                        var res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   join daypt in daybeforeyesterday_touchline on pt.Symbol equals daypt.Symbol
                                   where
                                   condition == IEODScreenerENUM.Greater ?
                                     (t.Open - t.PreviousClose) > value && (pt.Open - pt.PreviousClose) > value && (daypt.Open - daypt.PreviousClose) > value
                                   : (t.Open - t.PreviousClose) < value && (pt.Open - pt.PreviousClose) < value && (daypt.Open - daypt.PreviousClose) < value
                                   orderby (t.Open - t.PreviousClose) descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       PriceAction = g.Select(x => x.Open - x.PreviousClose).FirstOrDefault().ToString("0.00"),
                                       Date = g.Select(x => x.LastUpdatedTime.Date).FirstOrDefault()
                                   }
                                     ).OrderByDescending(x => x.Date).ToList();

                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "trend-1")
                    {
                        var res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   where
                                   condition == IEODScreenerENUM.Greater ?
                                   t.High > pt.High && t.Low > pt.Low
                                   : (t.High < pt.High && t.Low < pt.Low)
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       PriceAction = g.Select(x => x.Low).FirstOrDefault().ToString("0.00"),
                                       Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault()
                                   }
                                 ).OrderByDescending(x => x.Date).ToList();

                        if (result.Count > 0)
                            result = (from r in result join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol } select _r).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "trend-2")
                    {
                        var res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   join daypt in daybeforeyesterday_touchline on t.Symbol equals daypt.Symbol
                                   where
                                   condition == IEODScreenerENUM.Greater ?
                                   t.High > pt.High && t.Low > pt.Low
                                   && pt.High > daypt.High && pt.Low > daypt.Low
                                   : (t.High < pt.High && t.Low < pt.Low
                                   && (pt.High < daypt.High && pt.Low < daypt.Low))
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       PriceAction = g.Select(x => x.Low).FirstOrDefault().ToString("0.00"),
                                       Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault()
                                   }
                                 ).OrderByDescending(x => x.Date).ToList();

                        if (result.Count > 0)
                            result = (from r in result join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol } select _r).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "trend-3")
                    {
                        var res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   join daypt in daybeforeyesterday_touchline on pt.Symbol equals daypt.Symbol
                                   join daypt1 in daybeforeyesterday1_touchline on t.Symbol equals daypt1.Symbol
                                   where
                                   condition == IEODScreenerENUM.Greater ?
                                   t.High > pt.High && t.Low > pt.Low && pt.High > daypt.High && pt.Low > daypt.Low && daypt.High > daypt1.High && daypt.Low > daypt1.Low
                                   : t.High < pt.High && t.Low < pt.Low && pt.High < daypt.High && pt.Low < daypt.Low && daypt.High < daypt1.High && daypt.Low < daypt1.Low
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       PriceAction = g.Select(x => x.Low).FirstOrDefault().ToString("0.00"),
                                       Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault()
                                   }
                                 ).OrderByDescending(x => x.Date).ToList();

                        if (result.Count > 0)
                            result = (from r in result join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol } select _r).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "price-day")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Day > value : s.Day < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, PriceAction = g.Select(x => x.Day).FirstOrDefault()?.ToString("0.00"), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "price-week")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Week > value : s.Week < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, PriceAction = g.Select(x => x.Week).FirstOrDefault()?.ToString("0.00"), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "price-month")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Month > value : s.Month < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, PriceAction = g.Select(x => x.Month).FirstOrDefault()?.ToString("0.00"), Date = g.Select(x => x.Date).FirstOrDefault() }
                                      ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "price-3month")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Month3 > value : s.Month3 < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, PriceAction = g.Select(x => x.Month3).FirstOrDefault()?.ToString("0.00"), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "price-6month")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Month6 > value : s.Month6 < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, PriceAction = g.Select(x => x.Month6).FirstOrDefault()?.ToString("0.00"), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "price-1year")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Year > value : s.Year < value
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, PriceAction = g.Select(x => x.Year).FirstOrDefault()?.ToString("0.00"), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "price52-week-1")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? (s.YearHigh - s.Ltp) * 100 / s.Ltp < 1 : (s.Ltp - s.YearLow) * 100 / s.Ltp < 1
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, PriceAction = g.Select(x => condition == IEODScreenerENUM.Greater ? (x.YearHigh - x.Ltp) * 100 / x.Ltp : (x.Ltp - x.YearLow) * 100 / x.Ltp).FirstOrDefault()?.ToString("0.00"), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "price52-week-1-2")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? (s.YearHigh - s.Ltp) * 100 / s.Ltp > 1 && (s.YearHigh - s.Ltp) * 100 / s.Ltp < 2 : (s.Ltp - s.YearLow) * 100 / s.Ltp > 1 && (s.Ltp - s.YearLow) * 100 / s.Ltp < 2
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, PriceAction = g.Select(x => condition == IEODScreenerENUM.Greater ? (x.YearHigh - x.Ltp) * 100 / x.Ltp : (x.Ltp - x.YearLow) * 100 / x.Ltp).FirstOrDefault()?.ToString("0.00"), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "price52-week")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.YearHighDate > DateTime.Now.AddDays(-7) : s.YearLowDate > DateTime.Now.AddDays(-7)
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, PriceAction = g.Select(x => condition == IEODScreenerENUM.Greater ? x.YearHighDate : x.YearLowDate).FirstOrDefault()?.ToString("d"), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = _r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "dsma20-close")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Lesser ? s.Dsma20 > s.Ltp : s.Dsma20 < s.Ltp
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, MovingAverage = g.Select(x => x.Dsma20).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();

                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = _r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "dsma50-close")
                    {
                        var res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Lesser ? s.Dsma50 > s.Ltp : s.Dsma50 < s.Ltp
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, MovingAverage = g.Select(x => x.Dsma50).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();

                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = _r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "dsma100-close")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "dsma100-close";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Lesser ? s.Dsma100 > s.Ltp : s.Dsma100 < s.Ltp
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, MovingAverage = g.Select(x => x.Dsma100).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = _r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "dsma200-close")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "dsma200-close";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   where
                                   condition == IEODScreenerENUM.Lesser ? s.Dsma200 > s.Ltp : s.Dsma200 < s.Ltp
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, MovingAverage = g.Select(x => x.Dsma200).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = _r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "dsma20-cross")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "dsma20-cross";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   join t in touchline on s.Stock equals t.Symbol
                                   where
                                   condition == IEODScreenerENUM.Lesser ?
                                   (s.Dsma20 > s.Ltp && t.PreviousClose > s.Dsma20) :
                                   (s.Dsma20 < s.Ltp && t.PreviousClose < s.Dsma20)
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, MovingAverage = g.Select(x => x.Dsma20).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = _r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "dsma50-cross")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "dsma50-cross";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   join t in touchline on s.Stock equals t.Symbol
                                   where
                                   condition == IEODScreenerENUM.Lesser ?
                                    (s.Dsma50 > s.Ltp && t.PreviousClose > s.Dsma50) :
                                   (s.Dsma50 < s.Ltp && t.PreviousClose < s.Dsma50)
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, MovingAverage = g.Select(x => x.Dsma50).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                     ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = _r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "dsma100-cross")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "dsma100-cross";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   join t in touchline on s.Stock equals t.Symbol
                                   where
                                   condition == IEODScreenerENUM.Lesser ?
                                   (s.Dsma100 > s.Ltp && t.PreviousClose > s.Dsma100) : (s.Dsma100 < s.Ltp && t.PreviousClose < s.Dsma100)
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, MovingAverage = g.Select(x => x.Dsma100).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = _r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "dsma200-cross")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "dsma200-cross";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from s in eod
                                   join t in touchline on s.Stock equals t.Symbol
                                   where
                                   condition == IEODScreenerENUM.Lesser ?
                                   (s.Dsma200 > s.Ltp && t.PreviousClose > s.Dsma200)
                                   : (s.Dsma200 < s.Ltp && t.PreviousClose < s.Dsma200)
                                   orderby s.Date descending
                                   group s by s.Stock into g
                                   select new IEODScreenerData { Symbol = g.Key, MovingAverage = g.Select(x => x.Dsma200).FirstOrDefault(), Date = g.Select(x => x.Date).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = _r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "hammer")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "hammer";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in touchline
                                   where t.Open > t.Ltp ? (((t.Ltp - t.Low) >= 2 * (t.Open - t.Ltp)) && ((t.High - t.Open) <= t.Open - t.Ltp))
                                  : ((t.Open - t.Low) >= 2 * (t.Ltp - t.Open) && (t.High - t.Ltp) <= (t.Ltp - t.Open))
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, CandleStick = "hammer", Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = _r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "bullish engulfing")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "bullish engulfing";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   where (pt.Open > pt.Ltp) && t.Open <= pt.Ltp && t.Ltp >= pt.Open
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, CandleStick = "bullish engulfing", Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = _r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "bearish engulfing")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "bearish engulfing";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   where (pt.Ltp > pt.Open) && t.Open >= pt.Ltp && t.Ltp <= pt.Open
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, CandleStick = "bearish engulfing", Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = _r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "dark cloud cover")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "dark cloud cover";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   where (pt.Ltp < pt.Open) && t.Ltp <= (pt.Ltp - (pt.Ltp - pt.Open)) && t.Open >= pt.Ltp
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, CandleStick = "dark cloud cover", Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault() }
                                   ).OrderByDescending(x => x.Date).ToList();

                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = _r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "piercing")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "piercing";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   where (pt.Open > pt.Ltp) && t.Ltp >= (pt.Ltp + (pt.Open - pt.Ltp)) && t.Open <= pt.Ltp
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, CandleStick = "piercing", Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = _r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "shooting star")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "shooting star";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in touchline
                                   where t.Open > t.Ltp ? ((t.High - t.Open >= 2 * (t.Open - t.Ltp)) && ((t.Ltp - t.Low) <= t.Open - t.Ltp))
                                  : ((t.High - t.Ltp >= 2 * (t.Ltp - t.Open)) && ((t.Open - t.Low) <= t.Open - t.Low))
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, CandleStick = "shooting star", Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = _r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "bullish harami")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "bullish harami";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   where (pt.Open > pt.Ltp) && t.High < pt.High && t.Low > pt.Low && t.Ltp <= (t.Open + (.5m * t.Open / 100)) && t.Ltp > t.Open
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, CandleStick = "bullish harami", Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = _r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "bearish harami")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "bearish harami";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in touchline
                                   join pt in previous_touchline on t.Symbol equals pt.Symbol
                                   where (pt.Ltp > pt.Open) && t.High < pt.High && t.Low > pt.Low && t.Ltp <= (t.Open - (.5m * t.Open / 100)) && t.Ltp < t.Open
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, CandleStick = "bearish harami", Date = g.Select(x => x.LastUpdatedTime).FirstOrDefault() }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = _r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "iv increase")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "iv increase";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in todayIVData
                                   join pt in yesterdayIVData on t.Symbol equals pt.Symbol
                                   where ((t.Ceiv + t.Peiv) / 2) > ((pt.Ceiv + pt.Peiv) / 2)
                                   orderby t.UpdatedOn descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       Volatility = (g.FirstOrDefault().Ceiv + g.FirstOrDefault().Peiv) * 100 / 2,
                                       Date = g.Select(x => x.UpdatedOn).FirstOrDefault()
                                   }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = _r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "iv fall")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "iv fall";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in todayIVData
                                   join pt in yesterdayIVData on t.Symbol equals pt.Symbol
                                   where ((t.Ceiv + t.Peiv) / 2) < ((pt.Ceiv + pt.Peiv) / 2)
                                   orderby t.UpdatedOn descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       Volatility = (g.FirstOrDefault().Ceiv + g.FirstOrDefault().Peiv) * 100 / 2,
                                       Date = g.Select(x => x.UpdatedOn).FirstOrDefault()
                                   }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = _r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "ce oi increase")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "ce oi reduce";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in today_PCR
                                   join pt in previous_PCR on t.Stock equals pt.Stock
                                   where t.Ceoi > pt.Ceoi
                                   orderby t.Date descending
                                   group t by t.Stock into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       Option = g.FirstOrDefault().Expiry.Date,
                                       Date = g.Select(x => x.Date.Date).FirstOrDefault()
                                   }
                                   ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = _r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "pe oi increase")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "pe oi reduce";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in today_PCR
                                   join pt in previous_PCR on t.Stock equals pt.Stock
                                   where t.Peoi > pt.Peoi
                                   orderby t.Date descending
                                   group t by t.Stock into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       Option = g.FirstOrDefault().Expiry.Date,
                                       Date = g.Select(x => x.Date.Date).FirstOrDefault()
                                   }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = _r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "ce oi reduce")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "ce oi reduce";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in today_PCR
                                   join pt in previous_PCR on t.Stock equals pt.Stock
                                   where t.Ceoi < pt.Ceoi
                                   orderby t.Date descending
                                   group t by t.Stock into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       Option = g.FirstOrDefault().Expiry.Date,
                                       Date = g.Select(x => x.Date.Date).FirstOrDefault()
                                   }
                                     ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = _r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "pe oi reduce")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "pe oi reduce";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in today_PCR
                                   join pt in previous_PCR on t.Stock equals pt.Stock
                                   where t.Peoi < pt.Peoi
                                   orderby t.Date descending
                                   group t by t.Stock into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       Option = g.FirstOrDefault().Expiry.Date,
                                       Date = g.Select(x => x.Date.Date).FirstOrDefault()
                                   }
                                    ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = _r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "future oi increase")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = "future oi increase";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in future_touchline
                                   where t.PreviousOiclose < t.TodayOi && t.Symbol.EndsWith("FUT")
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = stocks.FirstOrDefault(s => s.Name == g.Key.Remove(g.Key.Length - 8, 8))?.DisplayName,
                                       Future = $"{g.Key.Substring(g.Key.Length - 8, 8)} : {g.Select(x => (x.TodayOi- x.PreviousOiclose)*100/x.PreviousOiclose).FirstOrDefault().ToString("0.00")} %",
                                       Date = g.Select(x => x.LastUpdatedTime.Date).FirstOrDefault()
                                   }
                                        ).OrderByDescending(x => x.Date).ToList();
                            var __ = JsonConvert.SerializeObject(res);
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      where _r.Symbol != null
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = _r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "future oi reduce")
                    {

                        var res = new List<IEODScreenerData>();
                        var key = "future oi reduce";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {

                            res = (from t in future_touchline
                                   where t.PreviousOiclose > t.TodayOi && t.Symbol.EndsWith("FUT")
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = stocks.FirstOrDefault(s => s.Name == g.Key.Remove(g.Key.Length - 8, 8))?.DisplayName,
                                       Future = $"{g.Key.Substring(g.Key.Length - 8, 8)} : {g.Select(x => (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose).FirstOrDefault().ToString("0.00")} %",
                                       Date = g.Select(x => x.LastUpdatedTime.Date).FirstOrDefault()
                                   }
                                        ).OrderByDescending(x => x.Date).ToList();
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      where _r.Symbol != null
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = _r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "buildup")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = $"buildup_{condition}";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in future_touchline
                                   where condition == IEODScreenerENUM.Greater ? t.PreviousOiclose < t.TodayOi && t.Symbol.EndsWith("FUT") && t.Ltp < t.PreviousClose : t.PreviousOiclose < t.TodayOi && t.Symbol.EndsWith("FUT") && t.Ltp > t.PreviousClose
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = stocks.FirstOrDefault(s => s.Name == g.Key.Remove(g.Key.Length - 8, 8))?.DisplayName,
                                       Future = $"{g.Key.Substring(g.Key.Length - 8, 8)} : {g.Select(x => (x.TodayOi- x.PreviousOiclose)*100/x.PreviousOiclose).FirstOrDefault().ToString("0.00")} %",
                                      
                                       Date = g.Select(x => x.LastUpdatedTime.Date).FirstOrDefault()
                                   }
                                        ).OrderByDescending(x => x.Date).ToList();
                            var __ = JsonConvert.SerializeObject(res);
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      where _r.Symbol != null
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = _r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "liquidate")
                    {
                        var res = new List<IEODScreenerData>();
                        var key = $"liquidate_{condition}";
                        var redisValue = cacheBL.GetValue(key);
                        if (redisValue != null)
                        {
                            res = JsonConvert.DeserializeObject<List<IEODScreenerData>>(redisValue);
                        }
                        else
                        {
                            res = (from t in future_touchline
                                   where condition == IEODScreenerENUM.Greater ? t.PreviousOiclose > t.TodayOi && t.Symbol.EndsWith("FUT") && t.Ltp < t.PreviousClose : t.PreviousOiclose > t.TodayOi && t.Symbol.EndsWith("FUT") && t.Ltp > t.PreviousClose
                                   orderby t.LastUpdatedTime descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = stocks.FirstOrDefault(s => s.Name == g.Key.Remove(g.Key.Length - 8, 8))?.DisplayName,
                                       Future = $"{g.Key.Substring(g.Key.Length - 8, 8)} : {g.Select(x => (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose).FirstOrDefault().ToString("0.00")} %",
                                       
                                       Date = g.Select(x => x.LastUpdatedTime.Date).FirstOrDefault()
                                   }
                                        ).OrderByDescending(x => x.Date).ToList();
                            var __ = JsonConvert.SerializeObject(res);
                            if (res.Count > 0)
                                cacheBL.SetValue(key, JsonConvert.SerializeObject(res));
                        }
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      where _r.Symbol != null
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = _r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "ivp")
                    {
                        var res = (from s in db.Ivdata
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Ivp > value : s.Ivp < value
                                   orderby s.UpdatedOn descending
                                   group s by s.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, Volatility = g.Select(x => x.Ivp).FirstOrDefault(), Date = g.Select(x => x.UpdatedOn).FirstOrDefault() }
                                   ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = _r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "ivr")
                    {
                        var res = (from s in db.Ivdata
                                   where
                                   condition == IEODScreenerENUM.Greater ? s.Ivr > value : s.Ivr < value
                                   orderby s.UpdatedOn descending
                                   group s by s.Symbol into g
                                   select new IEODScreenerData { Symbol = g.Key, Volatility = g.Select(x => x.Ivr).FirstOrDefault(), Date = g.Select(x => x.UpdatedOn).FirstOrDefault() }
                                   ).OrderByDescending(x => x.Date).ToList();
                        if (result.Count > 0)
                            result = (from r in result
                                      join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol }
                                      select new IEODScreenerData
                                      {
                                          CandleStick = r.CandleStick,
                                          Future = r.Future,
                                          Delivery = r.Delivery,
                                          MovingAverage = r.MovingAverage,
                                          Option = r.Option,
                                          PriceAction = r.PriceAction,
                                          Sector = r.Sector,
                                          Volatility = _r.Volatility,
                                          Volume = r.Volume,
                                          Date = r.Date,
                                          Symbol = r.Symbol
                                      }).ToList();
                        else
                            result.AddRange(res);
                    }
                    if (column == "iv-increase-fall")
                    {
                        var res = (from t in db.Ivdata
                                   join pt in db.Ivdata on t.Symbol equals pt.Symbol
                                   where
                                   condition == IEODScreenerENUM.Greater ?
                                   (t.Ceiv + t.Peiv) * 100 / 2 > (pt.Ceiv + pt.Peiv) * 100 / 2
                                   : (t.Ceiv + t.Peiv) * 100 / 2 < (pt.Ceiv + pt.Peiv) * 100 / 2
                                   orderby t.UpdatedOn descending
                                   group t by t.Symbol into g
                                   select new IEODScreenerData
                                   {
                                       Symbol = g.Key,
                                       Volatility = g.Select(x => (x.Ceiv + x.Peiv) * 100 / 2).FirstOrDefault(),
                                       Date = g.Select(x => x.UpdatedOn).FirstOrDefault()
                                   }
                                 ).OrderByDescending(x => x.Date).ToList();

                        if (result.Count > 0)
                            result = (from r in result join _r in res on new { r.Date, r.Symbol } equals new { _r.Date, _r.Symbol } select _r).ToList();
                        else
                            result.AddRange(res);
                    }
                }

            };
            return result.ToList();
        }

        #region EODScan
        public async Task SaveEODScan(EODScanRequest request)
        {
            var value = dbUser.Eodscans.FirstOrDefault(x => x.Id == request.Id) ?? new Eodscan();
            value.Option = request.Option;
            value.ScanDataName = request.ScanDataName;
            value.UserId = request.UserId;
            value.PriceAction = request.PriceAction;
            value.MovingAverage = request.MovingAverage;
            value.CandleStick = request.CandleStick;
            value.Delivery = request.Delivery;
            value.Future = request.Future;
            value.Volume = request.Volume;
            value.Volatility = request.Volatility;
            if (value.Id == 0)
            {
                value.CreatedOn = DateTime.Now;
                dbUser.Eodscans.Add(value);
            }
            await dbUser.SaveChangesAsync();
        }
        public async Task DeleteEODScan(int id)
        {
            dbUser.Eodscans.RemoveRange(dbUser.Eodscans.Where(x => x.Id == id));
            await dbUser.SaveChangesAsync();
        }
        public async Task<List<Eodscan>> GetEODScan(int userId)
        {
            return await dbUser.Eodscans.Where(x => x.UserId == userId).ToListAsync();
        }
        #endregion
    }
}
