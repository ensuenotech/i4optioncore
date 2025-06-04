using i4optioncore.Models;
using i4optioncore.Repositories;
using i4optioncore.Repositories.OptionWindow;
using i4optioncore.Repositories.Snapshot;
using i4optioncore.Repositories.UpdateRedis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Namotion.Reflection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static i4optioncore.Models.CommonModel;

namespace i4optioncore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [OutputCache(Duration = 60 * 1)]
    public class CommonController : ControllerBase
    {
        readonly ICommonBL commonBL;
        readonly IStocksBL stocksBL;
        readonly IRedisBL redisBL;
        private readonly ICacheBL cacheBL;
        private readonly ISnapshotBL snapshotBL;
        private readonly IOptionWindowBL optionWindowBL;
        private readonly IUpdateBL updateBL;
        public CommonController(ICommonBL _commonBL, IRedisBL _redisBL, IStocksBL stocksBL, IOptionWindowBL optionWindowBL, IUpdateBL updateBL, ISnapshotBL snapshotBL, ICacheBL cacheBL)
        {
            commonBL = _commonBL;
            redisBL = _redisBL;

            this.stocksBL = stocksBL;
            this.optionWindowBL = optionWindowBL;
            this.updateBL = updateBL;
            this.snapshotBL = snapshotBL;
            this.cacheBL = cacheBL;
        }

        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetStocks"), HttpGet]
        public async Task<IActionResult> GetStocks()
        {
            try
            {
                var _result = await commonBL.GetStocks();
                return Ok(_result.Select(x => new
                {
                    x.CalendarId,
                    x.SegmentIds,
                    x.Expiry,
                    x.Segments,
                    x.FreeFloat,
                    x.Change,
                    x.Id,
                    x.LotSize,
                    x.Depth,
                    x.MaxPain,
                    x.Name,
                    x.DisplayName
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetSegments"), HttpGet]
        public async Task<IActionResult> GetSegments()
        {
            try
            {
                var _result = await commonBL.GetSegments();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetStocksWithMPD"), HttpGet]
        public async Task<IActionResult> GetStocksWithMPD()
        {
            try
            {
                var _result = new List<CommonModel.StockDetails>();
                var key = "GetStocksWithMPD";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    _result = JsonConvert.DeserializeObject<List<CommonModel.StockDetails>>(redisValue);
                }
                else
                {
                    _result = await commonBL.GetStocks();
                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetExpiry"), HttpGet]
        public async Task<IActionResult> GetExpiry()
        {
            try
            {
                var _result = await commonBL.GetExpiryDates(null);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetLotSize"), HttpPost]
        public async Task<IActionResult> GetLotSize([FromBody] GetLotSizeRequest request)
        {
            try
            {
                int? _result;
                var key = "LotSize_" + JsonConvert.SerializeObject(request);
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null)
                    {
                        if ((DateTime.Parse(timekeyvalue).AddHours(1)) > time)
                        {
                            _result = JsonConvert.DeserializeObject<int>(redisValue);

                        }
                        else
                        {
                            _result = commonBL.GetLotSize(request.Stock, request.Expiry);
                            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                            await redisBL.SetValue(timekey, time.ToString());
                        }
                    }
                    else
                    {
                        _result = commonBL.GetLotSize(request.Stock, request.Expiry);
                        await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                        await redisBL.SetValue(timekey, time.ToString());
                    }
                }
                else
                {
                    _result = commonBL.GetLotSize(request.Stock, request.Expiry);
                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                    await redisBL.SetValue(timekey, time.ToString());
                }
                return Ok(_result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [OutputCache(Duration = 12 * 60 * 60)]
        [Route("GetExpiry/{stockId}"), HttpGet]
        public async Task<IActionResult> GetExpiry(int StockId)
        {
            try
            {
                var _result = await commonBL.GetExpiryDates(StockId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetAllExpiry/{stockId}"), HttpGet]
        public async Task<IActionResult> GetAllExpirybyStock(int StockId)
        {
            try
            {
                var _result = await commonBL.GetAllExpiryDates(StockId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetAllExpiry"), HttpGet]
        public async Task<IActionResult> GetAllExpiry()
        {
            try
            {
                var _result = await commonBL.GetAllExpiry();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetAllFinNiftyExpiry"), HttpGet]
        public async Task<IActionResult> GetAllFinNiftyExpiry()
        {
            try
            {
                var _result = await commonBL.GetAllFinNiftyExpiry();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        //[Route("SendSMS"), HttpGet]
        //public IActionResult SendSMS()
        //{
        //    try
        //    {
        //        var _result = twilioBL.SendSMS("Your OTP", "+919560940021");
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}

        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetCalendars"), HttpGet]
        public async Task<IActionResult> GetCalendars()
        {
            try
            {
                var _result = await commonBL.GetCalendars();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetCalendars"), HttpPost]
        public async Task<IActionResult> GetCalendarsPost()
        {
            try
            {
                var _result = await commonBL.GetCalendars();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("DeleteCalendarDate"), HttpPost]
        public async Task<IActionResult> DeleteCalendar([FromBody] DateTime date)
        {
            try
            {
                var _result = await commonBL.DeleteCalendarDate(date);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SubscribeSymbols"), HttpPost]
        public async Task<IActionResult> SubscribeSymbols([FromBody] List<string> symbol)
        {
            try
            {
                await commonBL.SubscribeSymbol(symbol);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SaveCalendar"), HttpPost]
        public async Task<IActionResult> AddCalendarDate([FromBody] SaveCalendarRequest data)
        {
            try
            {
                var _result = await commonBL.SaveCalendar(data.Id, data.Name);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SaveCalendarDate"), HttpPost]
        public async Task<IActionResult> SaveCalendar([FromBody] SaveCalendarDateRequest data)
        {
            try
            {
                var _result = await commonBL.SaveCalendarDate(data.Id, data.Date, data.CalendarId, data.Active);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        //[ResponseCache(Duration = 12 * 60 * 60)]
        //[Route("GetSymbols"), HttpGet]
        //public async Task<IActionResult> GetSymbols()
        //{
        //    try
        //    {
        //        var _result = await commonBL.GetSymbols();
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}
        //[ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetSymbols"), HttpGet]
        public async Task<IActionResult> GetSymbols()
        {
            try
            {
                var _result = new List<CommonModel.SymbolDetails>();
                var key = "AllSymbols";
                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                { _result = JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(redisValue); }
                else
                {
                    _result = await commonBL.GetSymbols();
                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("GetMaster"), HttpGet]
        public async Task<IActionResult> GetMaster()
        {
            try
            {
                var _result = await commonBL.GetDataForMaster();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("EarningRatio"), HttpPost]
        public async Task<IActionResult> GetEarningRatios([FromBody] string symbol)
        {
            try
            {
                var _result = await commonBL.GetEarningRatios(symbol);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("GetStrikes"), HttpPost]
        public async Task<IActionResult> GetStrikes([FromBody] CommonModel.GetStrikesForm values)
        {
            try
            {
                var key = $"Strikes_{values.StockName}_{values.Expiry.ToShortDateString()}_{values.Series.ToUpper()}";
                var value = await redisBL.GetValue(key);
                if (value != null)
                {
                    return Ok(JsonConvert.DeserializeObject<List<string>>(value));
                }
                var _result = await commonBL.GetStrikes(values.StockName, values.Expiry, values.Series);
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("UpdateMaxPain"), HttpPost]
        public async Task<IActionResult> UpdateMaxPain([FromBody] MaxPainRequest data)
        {
            try
            {
                var _result = await commonBL.UpdateMaxPain(data.MaxPain, data.StockId);
                var key = "Stocks";
                await redisBL.DeleteKey(key);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("Breadth"), HttpPost]
        public async Task<IActionResult> GetBreadth()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                var _result = new List<CommonModel.BreadthDetails>();
                var key = "Breadth";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";
                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);

                    if (timekeyvalue != null)
                    {
                        if (afterMarket)
                        {
                            _result = JsonConvert.DeserializeObject<List<CommonModel.BreadthDetails>>(redisValue);
                            return Ok(_result);
                        }
                        else if (DateTime.Parse(timekeyvalue).AddMinutes(1) > time)
                        {
                            _result = JsonConvert.DeserializeObject<List<CommonModel.BreadthDetails>>(redisValue);
                            return Ok(_result);
                        }
                    }
                }

                _result = commonBL.GetBreadth();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 60)]
        [Route("GetHistory"), HttpPost]
        [Route("GetHistoryApi"), HttpPost]
        public async Task<IActionResult> GetHistoryApi([FromBody] CommonModel.GetHistoryRequest request)
        {
            request.Symbols = request.Symbols.Distinct().ToList();

            if (!(request.Interval == 1 || request.Interval == 3 || request.Interval == 5 || request.Interval == 10 || request.Interval == 15))
                return BadRequest("Interval Problem");
            try
            {
                request.To = new DateTime(request.To.Year, request.To.Month, request.To.Day, 15, 30, 0);
                bool afterMarket = false;
                if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                var _result = new List<CommonModel.HistoryRecord>();
                var key = "GetHistoryApi" + string.Join("|", request.Symbols) + "_" + request.From.ToString("dd-MM-yy-HHmm") + "_" + request.To.ToString("dd-MM-yy-HHmm") + "_" + request.Interval;
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null)
                    {
                        if (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(request.Interval) > time))
                        {
                            _result = JsonConvert.DeserializeObject<List<CommonModel.HistoryRecord>>(redisValue);
                            return Ok(_result);
                        }
                    }
                }
                _result = await commonBL.GetHistory(request.Symbols, request.From, request.To, request.Interval);
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                if (_result.Count > 0)
                    await redisBL.SetValue(timekey, _result.OrderByDescending(x => x.LastTradeTime).FirstOrDefault().LastTradeTime.ToString());

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("GetFullHistory"), HttpPost]
        public async Task<IActionResult> GetFullHistory([FromBody] CommonModel.GetHistoryRequest request)
        {
            try
            {
                var _result = new List<CommonModel.HistoryRecord>();
                //var key = "GetHistory" + JsonConvert.SerializeObject(request.Symbols) + "_" + request.From + "_" + request.To + "_" + request.Interval;
                //var redisValue = await redisBL.GetValue(key);
                //if (redisValue != null)
                //    _result = JsonConvert.DeserializeObject<List<CommonModel.HistoryRecord>>(redisValue);
                //else
                {
                    _result = await commonBL.GetHistory(request.Symbols, request.From, request.To, request.Interval);
                    //await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SaveStock"), HttpPost]
        public async Task<IActionResult> SaveStock([FromBody] CommonModel.StockFormDetails request)
        {
            try
            {
                var _result = await commonBL.SaveStockDetails(request);
                var key = "Stocks";
                await redisBL.DeleteKey(key);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DeleteStock"), HttpPost]
        public async Task<IActionResult> SaveStDeleteStockock([FromBody] int StockId)
        {
            try
            {
                await commonBL.DeleteStock(StockId);
                var key = "Stocks";
                await redisBL.DeleteKey(key);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetMarketHoliday"), HttpGet]
        public async Task<IActionResult> GetMarketHoliday()
        {
            try
            {
                var _result = 0;
                //var key = "GetMarketHoliday";

                //var redisValue = await redisBL.GetValue(key);
                //if (redisValue != null)
                //{
                //    _result = int.Parse(redisValue);
                //}
                //else
                //{
                _result = await commonBL.GetMarketHoliday();
                //await redisBL.SetValue(key, _result.ToString());
                //}
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UpdateMarketHoliday"), HttpPost]
        public async Task<IActionResult> UpdateMarketHoliday([FromBody] UpdateMarketHolidayRequest request)
        {
            try
            {
                var _result = updateBL.UpdateMarketHoliday(request.IsOpen, request.Days);
                await redisBL.DeleteKey("GetMarketHoliday");
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 60)]
        [Route("GetTouchline"), HttpPost]
        public async Task<IActionResult> GetTouchline([FromBody] List<string> Symbols)
        {
            Symbols = Symbols.Distinct().ToList();
            try
            {
                //bool afterMarket = false;
                //if ((DateTime.Now.Minute > 30 && DateTime.Now.Hour == 21) || DateTime.Now.Hour > 21 || DateTime.Now.Hour < 9 || (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 15))
                //    afterMarket = true;
                //if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                //    afterMarket = true;

                //var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                //var key = "GetTouchline_" + JsonConvert.SerializeObject(Symbols);
                //var time = DateTime.Now; //10000000 = 1 sec
                //var timekey = key + "_time";

                //var redisValue = await redisBL.GetValue(key);
                //if (redisValue != null)
                //{
                //    var timekeyvalue = await redisBL.GetValue(timekey);
                //    if (timekeyvalue != null)
                //    {
                //        if (afterMarket || ((DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                //        {
                //            _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                //            return Ok(_result);
                //        }
                //    }
                //}

                var _result = await commonBL.GetTouchline(Symbols);
                //await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                //await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 60)]
        [Route("ExpiryOI"), HttpPost]
        public async Task<IActionResult> GetExpiryOI([FromBody] IExpiryOIRequest request)
        {
            try
            {
                var res = new List<IExpiryOI>();
                foreach (var exp in request.Expiries)
                {
                    res.Add(await commonBL.GetExpiryOI(request.Symbol, exp));
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("Get1minTouchline"), HttpPost]
        public async Task<IActionResult> Get1minTouchline([FromBody] List<string> Symbols)
        {
            Symbols = Symbols.Distinct().ToList();
            try
            {
                var _result = new List<CommonModel.TouchlineSubscription1minDetails>();
                var key = "Get1minTouchline_" + JsonConvert.SerializeObject(Symbols);
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null)
                    {
                        if ((DateTime.Parse(timekeyvalue).AddSeconds(59)) > time)
                        {
                            _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscription1minDetails>>(redisValue);

                        }
                        else
                        {
                            _result = await commonBL.Get1minTouchline(Symbols);
                            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                            await redisBL.SetValue(timekey, time.ToString());
                        }
                    }
                    else
                    {
                        _result = await commonBL.Get1minTouchline(Symbols);
                        await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                        await redisBL.SetValue(timekey, time.ToString());
                    }
                }
                else
                {
                    _result = await commonBL.Get1minTouchline(Symbols);
                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                    await redisBL.SetValue(timekey, time.ToString());
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("TouchlineByDate"), HttpPost]
        [OutputCache]
        public async Task<IActionResult> GetTouchlineByDate([FromBody] ITouchlineBySingleDateRequest request)
        {
            request.Symbols = request.Symbols.Distinct().ToList();
            try
            {

                return Ok(await commonBL.GetTouchlineByDate(request.Symbols, request.Date, request.Date));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
        [Route("TouchlineStockByDate"), HttpPost]
        public async Task<IActionResult> GetTTouchlineStockByDate([FromBody] ITouchlineBySingleDateRequest request)
        {
            request.Symbols = request.Symbols.Distinct().ToList();
            try
            {

                return Ok(await commonBL.GetTouchlineStockByDate(request.Symbols, request.Date, request.Date));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
        [Route("GetTouchlineByDate"), HttpPost]
        public async Task<IActionResult> GetTouchlineByDate([FromBody] GetTouchlineByDateRequest request)
        {
            request.Symbols = request.Symbols.Distinct().ToList();
            try
            {
                bool afterMarket = false;
                //if ((DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19) || (DateTime.Now.Hour < 23 && DateTime.Now.Hour > 19))
                //    refreshResults = true;
                //if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                //    afterMarket = true;

                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetTouchlineByDate" + JsonConvert.SerializeObject(request.Symbols) + "_" + request.FromDate.ToString("dd-MM-yyyy") + "_" + request.ToDate.ToString("dd-MM-yyyy");
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null)
                    {
                        if (afterMarket)
                        {
                            _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        }
                        else if ((DateTime.Parse(timekeyvalue).AddMinutes(5)) > time)
                        {
                            _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);

                        }
                        else
                        {
                            _result = await commonBL.GetTouchlineByDate(request.Symbols, request.FromDate, request.ToDate);
                            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                            await redisBL.SetValue(timekey, time.ToString());
                        }
                    }
                }
                else
                {
                    _result = await commonBL.GetTouchlineByDate(request.Symbols, request.FromDate, request.ToDate);
                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                    await redisBL.SetValue(timekey, time.ToString());
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetTouchlineStockByDate"), HttpPost]
        public async Task<IActionResult> GetTouchlineStockByDate([FromBody] GetTouchlineByDateRequest request)
        {

            request.Symbols = request.Symbols.Distinct().ToList();
            try
            {
                bool afterMarket = false;

                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetTouchlineStockByDate" + JsonConvert.SerializeObject(request.Symbols) + "_" + request.FromDate.ToString("dd-MM-yyyy") + "_" + request.ToDate.ToString("dd-MM-yyyy");
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null)
                    {
                        if (afterMarket)
                        {
                            _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        }
                        else if ((DateTime.Parse(timekeyvalue).AddMinutes(5)) > time)
                        {
                            _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);

                        }
                        else
                        {
                            _result = await commonBL.GetTouchlineStockByDate(request.Symbols, request.FromDate, request.ToDate);
                            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                            await redisBL.SetValue(timekey, time.ToString());
                        }
                    }
                }
                else
                {
                    _result = await commonBL.GetTouchlineStockByDate(request.Symbols, request.FromDate, request.ToDate);
                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                    await redisBL.SetValue(timekey, time.ToString());
                }
                return Ok(_result.Where(x => x.LastUpdatedTime.DayOfWeek != DayOfWeek.Saturday
            && x.LastUpdatedTime.DayOfWeek != DayOfWeek.Sunday));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("TouchlineWithMTO"), HttpPost]
        public async Task<IActionResult> TouchlineWithMTO([FromBody] GetTouchlineByDateRequest request)
        {
            request.Symbols = request.Symbols.Distinct().ToList();
            try
            {

                var _result = await commonBL.GetTouchlineWithMTOBulk(request.Symbols, request.FromDate, request.ToDate);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("SegmentTL"), HttpPost]
        public async Task<IActionResult> GetSegmentTouchline([FromBody] ISegmentTouchlineRequest request)
        {
            try
            {
                var res = await commonBL.GetSegmentTouchline(request.Segment, request.FromDate, request.ToDate);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        #region Option Window
        [ResponseCache(Duration = 60)]
        [Route("GetIndexActive"), HttpPost]
        public async Task<IActionResult> GetIndexActive()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetIndexActive";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = _result = await optionWindowBL.GetIndexActive();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetIndexFarActivity"), HttpPost]
        public async Task<IActionResult> GetIndexFarActivity()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetIndexFarActivity";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetIndexFarActivity();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetIndexOH"), HttpPost]
        public async Task<IActionResult> GetIndexOH()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetIndexOH";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = _result = await optionWindowBL.GetIndexOH();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetIndexOL"), HttpPost]
        public async Task<IActionResult> GetIndexOL()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetIndexOL";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = _result = await optionWindowBL.GetIndexOL();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetIndexOIGainer"), HttpPost]
        public async Task<IActionResult> GetIndexOIGainer()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetIndexOIGainer";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetIndexOIGainer();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetIndexOILooser"), HttpPost]
        public async Task<IActionResult> GetIndexOILooser()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetIndexOILooser";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetIndexOILooser();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetIndexBuyers"), HttpPost]
        public async Task<IActionResult> GetIndexBuyers()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetIndexBuyers";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetIndexBuyers();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetIndexWriters"), HttpPost]
        public async Task<IActionResult> GetIndexWriters()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetIndexWriters";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetIndexWriters();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("IndexItmUnwinding"), HttpPost]
        public async Task<IActionResult> GetIndexItmUnwinding()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "IndexItmUnwinding";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetIndexItmUnwinding();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 60)]
        [Route("GetStocksActive"), HttpPost]
        public async Task<IActionResult> GetStocksActive()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetStocksActive";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = _result = await optionWindowBL.GetStocksActive();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetStocksFarActivity"), HttpPost]
        public async Task<IActionResult> GetStocksFarActivity()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetStocksFarActivity";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetStocksFarActivity();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetStocksOH"), HttpPost]
        public async Task<IActionResult> GetStocksOH()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetStocksOH";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = _result = await optionWindowBL.GetStocksOH();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetStocksOL"), HttpPost]
        public async Task<IActionResult> GetStocksOL()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetStocksOL";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = _result = await optionWindowBL.GetStocksOL();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetStocksOIGainer"), HttpPost]
        public async Task<IActionResult> GetStocksOIGainer()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetStocksOIGainer";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetStocksOIGainer();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetStocksOILooser"), HttpPost]
        public async Task<IActionResult> GetStocksOILooser()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetStocksOILooser";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetStocksOILooser();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetStocksBuyers"), HttpPost]
        public async Task<IActionResult> GetStocksBuyers()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetStocksBuyers";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetStocksBuyers();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("GetStocksWriters"), HttpPost]
        public async Task<IActionResult> GetStocksWriters()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "GetStocksWriters";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetStocksWriters();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("StocksItmUnwinding"), HttpPost]
        public async Task<IActionResult> GetStocksItmUnwinding()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                    afterMarket = false;
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = "StocksItmUnwinding";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";

                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null && (afterMarket || (DateTime.Parse(timekeyvalue).AddMinutes(1)) > time))
                    {
                        _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
                        return Ok(_result);
                    }
                }

                _result = await optionWindowBL.GetStocksItmUnwinding();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        //[Route("GetTouchlineIndexOption"), HttpGet]
        //public async Task<IActionResult> GetTouchlineIndexOption()
        //{
        //    try
        //    {
        //        bool afterMarket = false;
        //        if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
        //            afterMarket = true;
        //        if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
        //            afterMarket = false;
        //        var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
        //        var key = "GetTouchlineIndexOption";
        //        var time = DateTime.Now; //10000000 = 1 sec
        //        var timekey = key + "_time";

        //        var redisValue = await redisBL.GetValue(key);
        //        if (redisValue != null)
        //        {
        //            var timekeyvalue = await redisBL.GetValue(timekey);
        //            if (timekeyvalue != null)
        //            {
        //                if (afterMarket)
        //                {
        //                    _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
        //                }
        //                else if ((DateTime.Parse(timekeyvalue).AddMinutes(1)) > time)
        //                {
        //                    _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);

        //                }
        //                else
        //                {
        //                    _result = await commonBL.GetTouchlineIndexOption();
        //                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
        //                    await redisBL.SetValue(timekey, time.ToString());
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _result = _result = await commonBL.GetTouchlineIndexOption();
        //            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
        //            await redisBL.SetValue(timekey, time.ToString());
        //        }
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}
        //[Route("GetTouchlineOption"), HttpGet]
        //public async Task<IActionResult> GetTouchlineOption()
        //{
        //    try
        //    {
        //        bool afterMarket = false;
        //        if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
        //            afterMarket = true;
        //        if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
        //            afterMarket = false;
        //        var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
        //        var key = "GetTouchlineOption";
        //        var time = DateTime.Now; //10000000 = 1 sec
        //        var timekey = key + "_time";

        //        var redisValue = await redisBL.GetValue(key);
        //        if (redisValue != null)
        //        {
        //            var timekeyvalue = await redisBL.GetValue(timekey);

        //            if (timekeyvalue != null)
        //            {
        //                if (afterMarket)
        //                {
        //                    _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
        //                }
        //                else if ((DateTime.Parse(timekeyvalue).AddMinutes(1)) > time)
        //                {
        //                    _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);

        //                }
        //                else
        //                {
        //                    _result = await commonBL.GetTouchlineOption();
        //                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
        //                    await redisBL.SetValue(timekey, time.ToString());
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _result = _result = await commonBL.GetTouchlineOption();
        //            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
        //            await redisBL.SetValue(timekey, time.ToString());
        //        }
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}
        //[Route("GetTouchlineIndexWriting"), HttpGet]
        //public async Task<IActionResult> GetTouchlineIndexWriting()
        //{
        //    try
        //    {
        //        var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
        //        var key = "GetTouchlineIndexWriting";
        //        var time = DateTime.Now; //10000000 = 1 sec
        //        var timekey = key + "_time";

        //        var redisValue = await redisBL.GetValue(key);
        //        if (redisValue != null)
        //        {
        //            var timekeyvalue = await redisBL.GetValue(timekey);

        //            if (timekeyvalue != null)
        //            {
        //                if ((DateTime.Parse(timekeyvalue).AddMinutes(1)) > time)
        //                {
        //                    _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);

        //                }
        //                else
        //                {
        //                    _result = await commonBL.GetTouchlineIndexWriting();
        //                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
        //                    await redisBL.SetValue(timekey, time.ToString());
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _result = _result = await commonBL.GetTouchlineIndexWriting();
        //            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
        //            await redisBL.SetValue(timekey, time.ToString());
        //        }
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}
        //[Route("GetFarActivity"), HttpGet]
        //public async Task<IActionResult> GetFarActivity()
        //{
        //    try
        //    {
        //        bool afterMarket = false;
        //        if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
        //            afterMarket = true;
        //        if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
        //            afterMarket = false;

        //        var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
        //        var key = "GetFarActivity";
        //        var time = DateTime.Now; //10000000 = 1 sec
        //        var timekey = key + "_time";

        //        var redisValue = await redisBL.GetValue(key);
        //        if (redisValue != null)
        //        {
        //            var timekeyvalue = await redisBL.GetValue(timekey);

        //            if (timekeyvalue != null)
        //            {
        //                if (afterMarket)
        //                {
        //                    _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);
        //                }
        //                else if ((DateTime.Parse(timekeyvalue).AddMinutes(1)) > time)
        //                {
        //                    _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(redisValue);

        //                }
        //                else
        //                {
        //                    _result = await commonBL.GetFarActivity();
        //                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
        //                    await redisBL.SetValue(timekey, time.ToString());
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _result = _result = await commonBL.GetFarActivity();
        //            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
        //            await redisBL.SetValue(timekey, time.ToString());
        //        }
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}
        #endregion

        [ResponseCache(Duration = 60)]
        [Route("GetCommentary"), HttpPost]
        public async Task<IActionResult> GetCommentary()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                var _result = new List<DBModels.VolumeCommentary>();
                var key = "GetCommentary";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";
                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);

                    if (timekeyvalue != null)
                    {
                        if (afterMarket)
                        {
                            _result = JsonConvert.DeserializeObject<List<DBModels.VolumeCommentary>>(redisValue);
                        }
                        else if (DateTime.Parse(timekeyvalue).AddMinutes(1) > time)
                        {
                            _result = JsonConvert.DeserializeObject<List<DBModels.VolumeCommentary>>(redisValue);
                        }

                    }
                }

                _result = await commonBL.GetVolumeCommentary();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SpotCommentary"), HttpPost]
        public async Task<IActionResult> GetSpotCommentary()
        {
            try
            {
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                var _result = new List<DBModels.SpotVolumeCommentary>();
                var key = "GetSpotCommentary";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";
                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);

                    if (timekeyvalue != null)
                    {
                        if (afterMarket)
                        {
                            _result = JsonConvert.DeserializeObject<List<DBModels.SpotVolumeCommentary>>(redisValue);
                        }
                        else if (DateTime.Parse(timekeyvalue).AddMinutes(1) > time)
                        {
                            _result = JsonConvert.DeserializeObject<List<DBModels.SpotVolumeCommentary>>(redisValue);
                        }

                    }
                }

                _result = await commonBL.GetSpotVolumeCommentary();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("GetMaxpain"), HttpPost]
        public async Task<IActionResult> GetMaxpain([FromBody] CommonModel.MaxpainRequest req)
        {
            try
            {
                var fromTime = req.From;
                var toTime = req.To;
                var _result = await commonBL.GetMaxPain(req.Symbol, req.Expiry, fromTime, toTime);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        [Route("GetIVData"), HttpPost]
        public async Task<IActionResult> GetIVData([FromBody] DateTime date)
        {
            try
            {
                var _result = await commonBL.GetIvdata(date);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetIVDataByDate"), HttpPost]
        [OutputCache]
        public async Task<IActionResult> GetIVDataByDate([FromBody] GetIVDataByDateRequest request)
        {
            try
            {
                return Ok(await commonBL.GetIvdata(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetIVDataByDateWeeklyMonthly"), HttpPost]
        [OutputCache]
        public IActionResult GetIVDataByDateWeeklyMonthly([FromBody] GetIVDataByDateRequestWeeklyMonthly request)
        {
            try
            {
                return Ok(commonBL.GetIvdataWeeklyMonthly(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        [Route("Volatility"), HttpPost]
        [OutputCache(Duration = 60 * 60 * 12)]
        public IActionResult GetVolatility([FromBody] GetVolatilityRequest request)
        {
            try
            {
                return Ok(commonBL.GetVolatility(request));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("FutureRollover"), HttpPost]
        [OutputCache(Duration = 60 * 60 * 12)]
        public async Task<IActionResult> GetFutureRollover([FromBody] GetFutureRolloverRequest request)
        {
            try
            {
                return Ok(await commonBL.GetFutureRollOver(request.Expiry));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetInternalSymbols"), HttpPost]
        public async Task<IActionResult> GetInternalSymbols([FromBody] SearchInternalSymbolRequest SearchTerm)
        {
            try
            {
                var _result = new List<CommonModel.SymbolDetails>();
                var key = $"InternalSymbols_{SearchTerm.SearchTerm}";
                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    return Ok(JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(redisValue).Select(x => new { x.Symbol, x.Alias, x.TradingSymbol, x.SymbolId, x.Expiry, x.LotSize, x.SearchTerm, x.LTP }).ToList());
                }

                var allSymbolsRedisValue = await redisBL.GetValue("AllInternalSymbols");
                if (allSymbolsRedisValue != null)
                {
                    _result = JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(allSymbolsRedisValue);
                }
                else
                {
                    _result = await commonBL.GetTouchlineSymbols();
                    await redisBL.SetValue("AllInternalSymbols", JsonConvert.SerializeObject(_result));
                }

                if (SearchTerm.SearchTerm == "#all#")
                {
                    _result = _result.ToList();
                }
                else if (!string.IsNullOrEmpty(SearchTerm.SearchTerm))
                {
                    _result = _result.Where(x => x.SearchTerm.Any(s => s.StartsWith(SearchTerm.SearchTerm.ToUpper()))).ToList();
                }
                else
                {
                    _result = _result.Where(x => !(x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))).ToList();
                }
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                return Ok(_result.Select(x => new { x.Symbol, x.Alias, x.TradingSymbol, x.SymbolId, x.Expiry, x.LotSize, x.LTP }).ToList());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetInternalStocks"), HttpPost]
        public async Task<IActionResult> GetInternalStocks([FromBody] SearchInternalSymbolRequest SearchTerm)
        {
            try
            {
                var _result = new List<CommonModel.SymbolDetails>();
                var key = $"InternalStocks_{SearchTerm.SearchTerm}";
                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    return Ok(JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(redisValue).Select(x => new { x.Symbol, x.Alias, x.TradingSymbol, x.SymbolId, x.Expiry, x.LotSize }).ToList());
                }

                var allSymbolsRedisValue = await redisBL.GetValue("AllInternalStocks");
                if (allSymbolsRedisValue != null)
                {
                    _result = JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(allSymbolsRedisValue);
                }
                else
                {
                    _result = (await commonBL.GetTouchlineSymbols()).Where(x => x.Expiry != null && x.Strike == 0 && x.Series == "EQ").ToList();
                    await redisBL.SetValue("AllInternalStocks", JsonConvert.SerializeObject(_result));
                }

                if (SearchTerm.SearchTerm == "#all#")
                {
                    _result = _result.ToList();
                }
                else if (!string.IsNullOrEmpty(SearchTerm.SearchTerm))
                {
                    _result = _result.Where(x => x.SearchTerm.Any(s => s.StartsWith(SearchTerm.SearchTerm.ToUpper()))).ToList();
                }
                else
                {
                    _result = _result.Where(x => !(x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))).ToList();
                }
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                return Ok(_result.Select(x => new { x.Symbol, x.Alias, x.TradingSymbol, x.SymbolId, x.Expiry, x.LotSize }).ToList());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("GetInternalOptions"), HttpPost]
        public async Task<IActionResult> GetInternalOptions([FromBody] SearchInternalOptionsSymbolRequest SearchTerm)
        {
            try
            {
                var _result = new List<CommonModel.SymbolDetails>();
                var key = $"InternalOptions_{SearchTerm.SearchTerm}";
                var redisValue = cacheBL.GetValue(key);
                if (redisValue != null)
                {
                    return Ok(JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(redisValue).Select(x => new { x.Symbol, x.Alias, x.TradingSymbol, x.SymbolId, x.Expiry, x.LotSize }).ToList());
                }

                var allSymbolsRedisValue = cacheBL.GetValue("AllInternalOptions");
                if (allSymbolsRedisValue != null)
                {
                    _result = JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(allSymbolsRedisValue);
                }
                else
                {
                    _result = (await commonBL.GetTouchlineSymbols()).Where(x => ((SearchTerm.Expiry.HasValue && x.Expiry.HasValue && x.Expiry.Value.Date == SearchTerm.Expiry.Value.Date) || (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))) && x.LotSize != 0).ToList();
                    cacheBL.SetValue("AllInternalOptions", JsonConvert.SerializeObject(_result));
                }

                if (SearchTerm.SearchTerm == "#all#")
                {
                    _result = _result.ToList();
                }
                else if (!string.IsNullOrEmpty(SearchTerm.SearchTerm))
                {
                    _result = _result.Where(x => x.SearchTerm.Any(s => s.StartsWith(SearchTerm.SearchTerm.ToUpper()))).ToList();
                }

                cacheBL.SetValue(key, JsonConvert.SerializeObject(_result));
                return Ok(_result.OrderBy(x => x.Expiry).Select(x => new { x.Symbol, x.Alias, x.TradingSymbol, x.SymbolId, x.Expiry, x.LotSize }).Take(25).ToList());

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetInternalSymbolsDetails"), HttpPost]
        public async Task<IActionResult> GetInternalSymbolsDetails([FromBody] List<string> Symbols)
        {
            try
            {
                Symbols = Symbols.Distinct().ToList();
                var _result = new List<CommonModel.SymbolDetails>();
                var key = "AllInternalSymbols";
                var redisValue = await redisBL.GetValue(key);
                var timekey = key + "_time";
                var time = DateTime.Now; //10000000 = 1 sec

                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    if (timekeyvalue != null)
                    {
                        if (DateTime.Parse(timekeyvalue).AddHours(1) > time)
                        {
                            _result = JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(redisValue);
                            _result = _result.Where(x => Symbols.Contains(x.Symbol)).ToList();
                            return Ok(_result.Select(x => new { x.Symbol, x.Alias, x.TradingSymbol, x.SymbolId, x.Expiry, x.LotSize, x.Strike }).ToList());
                        }
                    }
                }

                _result = await commonBL.GetTouchlineSymbols();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());

                return Ok(_result.Select(x => new { x.Symbol, x.TradingSymbol, x.Alias, x.SymbolId, x.Expiry, x.LotSize, x.Strike }).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("GetMovingAverage"), HttpPost]
        public async Task<IActionResult> GetMovingAverage([FromBody] string Symbol)
        {
            try
            {
                var _result = await commonBL.GetMovingAverage(Symbol);

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetMovingExponential"), HttpPost]
        public async Task<IActionResult> GetMovingExponential([FromBody] string Symbol)
        {
            try
            {
                var _result = await commonBL.GetMovingExponential(Symbol);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 60)]
        [Route("ActiveOI"), HttpPost]
        public async Task<IActionResult> GetActiveOI([FromBody] IActiveOIRequest request)
        {
            try
            {
                var key = $"ActiveOI_{request.Symbol}_{request.Strike}_{request.Expiry:dd-MM-yyyy}";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";
                var value = await redisBL.GetValue(key);
                if (value != null)
                {
                    var hhmm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
                    if (hhmm > 1545 || hhmm < 915)
                    {
                        return Ok(JsonConvert.DeserializeObject<List<IActiveOI>>(value));

                    }
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    var results = JsonConvert.DeserializeObject<List<IActiveOI>>(value);
                    //var lastTick = results.OrderByDescending(x => x.LastTradeTime).FirstOrDefault().LastTradeTime;
                    if (DateTime.Parse(timekeyvalue).AddMinutes(1) > time)
                    //if (lastTick.AddMinutes(10) > DateTime.Now)
                    {
                        return Ok(results);
                    }

                }
                var _result = commonBL.GetActiveOI(request.Symbol, request.Expiry, request.Strike);
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("ActiveVOL"), HttpPost]
        public async Task<IActionResult> GetActiveVOL([FromBody] IActiveOIRequest request)
        {
            try
            {
                var key = $"ActiveVOL_{request.Symbol}_{request.Strike}_{request.Expiry:dd-MM-yyyy}";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";
                var value = await redisBL.GetValue(key);
                if (value != null)
                {
                    var hhmm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
                    if (hhmm > 1545 || hhmm < 915)
                    {
                        return Ok(JsonConvert.DeserializeObject<List<IActiveVOL>>(value));

                    }
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    var results = JsonConvert.DeserializeObject<List<IActiveVOL>>(value);
                    //var lastTick = results.OrderByDescending(x => x.LastTradeTime).FirstOrDefault().LastTradeTime;
                    if (DateTime.Parse(timekeyvalue).AddMinutes(1) > time)
                    //if (lastTick.AddMinutes(10) > DateTime.Now)
                    {
                        return Ok(results);
                    }

                }
                var _result = commonBL.GetActiveVOL(request.Symbol, request.Expiry, request.Strike);
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("FScalping"), HttpPost]
        public async Task<IActionResult> FScalping([FromBody] List<string> symbols)
        {
            try
            {
                symbols = symbols.Distinct().ToList();

                var key = $"FScalping_{string.Join("_", symbols)}";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";
                var value = await redisBL.GetValue(key);
                if (value != null)
                {
                    var hhmm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
                    if (hhmm > 1545 || hhmm < 915)
                    {
                        return Ok(JsonConvert.DeserializeObject<List<IScalping>>(value));

                    }
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    var results = JsonConvert.DeserializeObject<List<IScalping>>(value);
                    //var lastTick = results.OrderByDescending(x => x.LastTradeTime).FirstOrDefault().LastTradeTime;
                    if (DateTime.Parse(timekeyvalue).AddMinutes(1) > time)
                    //if (lastTick.AddMinutes(1) > DateTime.Now)
                    {
                        return Ok(results);
                    }

                }

                var _result = await commonBL.GetScalping(symbols);
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [OutputCache(Duration = 60 * 60 * 24)]
        [Route("PCRByDate"), HttpPost]
        public  IActionResult GetPCRByDate([FromBody] IPCRByDateRequest request)
        {
            try
            {
                var value = cacheBL.GetValue(JsonConvert.SerializeObject(request));
                if (value != null)
                {
                    return Ok(JsonConvert.DeserializeObject<List<IIntradayPCR>>(value));
                }
                var res = stocksBL.GetPcrByDate(request);
                cacheBL.SetValue(JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(res));
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("PCRByInterval"), HttpPost]
        public async Task<IActionResult> GetPCRByInterval([FromBody] GetPCRRequest request)
        {
            try
            {
                var result = await stocksBL.GetPcrByInterval(request);

                return Ok(result.Where(r => r.LastTradeTime.Year != 1).OrderBy(x => x.LastTradeTime).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("IntradayPCR"), HttpPost]
        public async Task<IActionResult> GetIntradayPCR([FromBody] GetPCRRequest request)
        {
            try
            {
                var result = await stocksBL.GetIntradayPCR(request.Symbol, request.Expiry);

                return Ok(result.Where(r => r.LastTradeTime.Year != 1).OrderBy(x => x.LastTradeTime).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("PremiumDecay"), HttpPost]
        public async Task<IActionResult> GetPremiumDecay([FromBody] IActiveOIRequest request)
        {
            try
            {
                var key = $"PremiumDecay_{request.Symbol}_{request.Strike}_{request.Expiry:dd-MM-yyyy}";

                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";
                var value = await redisBL.GetValue(key);

                if (value != null)
                {
                    var hhmm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
                    if (hhmm > 1545 || hhmm < 915)
                    {
                        return Ok(JsonConvert.DeserializeObject<List<IPremiumDecay>>(value));

                    }
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    var results = JsonConvert.DeserializeObject<List<IPremiumDecay>>(value);
                    if (DateTime.Parse(timekeyvalue).AddMinutes(1) > time)
                    {
                        return Ok(results);
                    }

                }
                var _result = commonBL.GetPremiumDecay(request.Symbol, request.Expiry, request.Strike);
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, time.ToString());

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetHolidays"), HttpGet]
        public async Task<IActionResult> GetHolidays()
        {
            try
            {
                var _result = await commonBL.GetHolidays();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 1 * 60 * 60)]
        [Route("EOD"), HttpPost]
        public async Task<IActionResult> EOD([FromBody] IEODRequest request)
        {
            try
            {
                var _result = await stocksBL.GetEods(request.Date, request.Symbols);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        //[ResponseCache(Duration = 12 * 60 * 60)]
        [Route("FutureDashboard"), HttpPost]
        public async Task<IActionResult> FutureDashboard()
        {
            try
            {

                var _result = await commonBL.GetFutureDashboard();

                return Ok(new
                {
                    priceGainer = _result.PriceGainer.Select(x => new { Symbol = x.Symbol.Split('-')[0], x.ChangePercentage }).ToList(),
                    priceLooser = _result.PriceLooser.Select(x => new { Symbol = x.Symbol.Split('-')[0], x.ChangePercentage }).ToList(),
                    oiGainer = _result.OIGainer.Select(x => new { Symbol = x.Symbol.Split('-')[0], x.OiChangePercentage }),
                    oiLooser = _result.OILooser.Select(x => new { Symbol = x.Symbol.Split('-')[0], x.OiChangePercentage }),
                    longBuildUp = _result.LongBuildUp.Select(x => new { Symbol = x.Symbol.Split('-')[0], x.ChangePercentage, x.OiChangePercentage }),
                    shortBuildUp = _result.ShortBuildUp.Select(x => new { Symbol = x.Symbol.Split('-')[0], x.ChangePercentage, x.OiChangePercentage }),
                    mostActiveByVolume = _result.MostActiveByVolume.Select(x => new { Symbol = x.Symbol.Split('-')[0], Volume = x.TickVolume }),

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("Snapshot"), HttpPost]
        public async Task<IActionResult> GetSnapshot([FromBody] ISnapshotRequest request)
        {
            try
            {
                var _result = new List<CommonModel.HistoryRecord>();
                if (request.time.Date == DateTime.Now.Date)
                    _result = await snapshotBL.GetIntradaySnapshot(request.Symbol, request.time);
                else
                    _result = await snapshotBL.GetHistorySnapshot(request.Symbol, request.time);

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("Snapshots"), HttpPost]
        public async Task<IActionResult> GetSnapshots([FromBody] ISnapshotsRequest request)
        {
            try
            {
                var _result = new List<CommonModel.HistoryRecord>();
                if (request.time.Date == DateTime.Now.Date)
                    _result = await snapshotBL.GetIntradaySnapshots(request.Symbols, request.time);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("OptionSnapshot"), HttpPost]
        public async Task<IActionResult> GetOptionSnapshot([FromBody] ISnapshotRequest request)
        {
            try
            {
                var key = $"OptionSnapshot_{request.Symbol}_{request.expiry:dd-MM-yyyy}_{request.time:dd-MM-yyyy HH:mm}";

                var value = await redisBL.GetValue(key);
                if (value != null)
                {
                    var result = JsonConvert.DeserializeObject<List<CommonModel.HistoryRecord>>(value);
                    if (result.Count != 0)
                        return Ok(result);
                }
                var _result = new List<CommonModel.HistoryRecord>();
                if (request.time.Date == DateTime.Now.Date)
                    _result = await snapshotBL.GetOptionIntradaySnapshot(request.Symbol, request.time, request.expiry.Value);
                else
                    _result = await snapshotBL.GetOptionHistorySnapshot(request.Symbol, request.time, request.expiry.Value);
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("OptionsTouchline"), HttpPost]
        public async Task<IActionResult> GetOptionTouchline([FromBody] IOptionsTouchlineRequest request)
        {
            try
            {
                var _result = new List<CommonModel.TouchlineSubscriptionDetails>();
                var key = $"OptionsTouchline_{request.Symbol}_{request.Expiry:dd-MM-yy}";
                bool afterMarket = false;
                if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    afterMarket = true;
                var timekey = key + "_time";
                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redisBL.GetValue(timekey);

                    if (timekeyvalue != null)
                    {
                        var value = await redisBL.GetValue(key);
                        var result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(value);
                        if (result.Count > 0)
                        {
                            if (afterMarket)
                            {
                                _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(value);
                                return Ok(_result);

                            }
                            else if (DateTime.Parse(timekeyvalue).AddMinutes(1) > DateTime.Now)
                            {
                                _result = JsonConvert.DeserializeObject<List<CommonModel.TouchlineSubscriptionDetails>>(value);
                                return Ok(_result);
                            }

                        }

                    }
                }

                _result = await commonBL.GetOptionTouchline(request.Symbol, request.Expiry);
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                await redisBL.SetValue(timekey, DateTime.Now.ToString());
                return Ok(_result);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 60)]
        [Route("FOIG"), HttpPost]
        public async Task<IActionResult> GetFOIG([FromBody] FOIGRequest request)
        {
            try
            {
                var key = $"FOIG_{request.Interval}_{request.Type}";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";
                var value = await redisBL.GetValue(key);
                if (value != null)
                {
                    var hhmm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
                    if (hhmm > 1545 || hhmm < 915)
                    {
                        return Ok(JsonConvert.DeserializeObject<List<FOIG>>(value));

                    }
                    var timekeyvalue = await redisBL.GetValue(timekey);
                    var results = JsonConvert.DeserializeObject<List<FOIG>>(value);
                    if (DateTime.Parse(timekeyvalue).AddMinutes(1) > time)
                    {
                        return Ok(results);
                    }

                }
                var _result = stocksBL.GetFOIG(request);
                if (_result.Count > 0)
                {
                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                    await redisBL.SetValue(timekey, time.ToString());
                }

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 60)]
        [Route("OptionDashboard"), HttpGet]
        public async Task<IActionResult> GetOptionDashboard()
        {
            try
            {
                var key = $"OPTION_DASHBOARD";
                var time = DateTime.Now; //10000000 = 1 sec
                var value = await redisBL.GetValue(key);
                if (value != null)
                {

                    var results = JsonConvert.DeserializeObject<List<OptionDashboard>>(value);
                    return Ok(results);

                }
                var _result = stocksBL.GetOptionDashboards();
                if (_result.Count > 0)
                {
                    var hhmm = DateTime.Now.Hour * 100 + DateTime.Now.Minute;

                    await redisBL.SetValueWithExpiry(key, JsonConvert.SerializeObject(_result), 60);
                }

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("historicperformance"), HttpPost]
        [OutputCache(Duration = 60 * 60 * 12)]
        public IActionResult Historicperformance([FromBody] string stock)
        {
            try
            {

                return Ok(stocksBL.GetHistoricPerformances(stock));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        //[ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetExcelCalendars"), HttpGet]
        public IActionResult GetExcelCalendars()
        {
            try
            {
                return Ok(commonBL.GetExcelCalendar());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        public class UpdateMarketHolidayRequest
        {
            public int Days { get; set; }
            public bool IsOpen { get; set; }
        }

        public class SaveCalendarDateRequest
        {
            public int Id { get; set; }
            public int CalendarId { get; set; }
            public bool Active { get; set; }
            public DateTime Date { get; set; }
        }
        public class SaveCalendarRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class MaxPainRequest
        {
            public int StockId { get; set; }
            public decimal MaxPain { get; set; }
        }
        public class GetLotSizeRequest
        {
            public string Stock { get; set; }
            public DateTime Expiry { get; set; }
        }
        public class GetTouchlineByDateRequest
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public List<string> Symbols { get; set; }
        }
        public class ITouchlineBySingleDateRequest
        {
            public DateTime Date { get; set; }
            public List<string> Symbols { get; set; }
        }
        public class ISegmentTouchlineRequest
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string Segment { get; set; }
        }
        public static string CheckHistoryRequest { get; set; }
        public class SearchInternalSymbolRequest
        {
            public required string SearchTerm { get; set; }
        }
        public class SearchInternalOptionsSymbolRequest
        {
            public string SearchTerm { get; set; }
            public DateTime? Expiry { get; set; }
        }
        public class GetIVDataByDateRequest
        {
            public List<string> Symbols { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public DateTime Expiry { get; set; }
        }
        public class GetIVDataByDateRequestWeeklyMonthly
        {
            public string Symbol { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string Type { get; set; }
        }

        public class GetVolatilityRequest
        {
            public string Symbol { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string Type { get; set; }
        }
        public class GetFutureRolloverRequest
        {
            public string Expiry { get; set; }
        }
        public class ISnapshotRequest
        {
            public string Symbol { get; set; }
            public DateTime time { get; set; }
            public DateTime? expiry { get; set; }
        }
        public class ISnapshotsRequest
        {
            public List<string> Symbols { get; set; }
            public DateTime time { get; set; }
        }
        public class IEODRequest
        {
            public List<string> Symbols { get; set; }
            public DateTime Date { get; set; }
        }
        public class IOptionsTouchlineRequest
        {
            public string Symbol { get; set; }
            public DateTime Expiry { get; set; }
        }
        public class IExpiryOIRequest
        {
            public string Symbol { get; set; }
            public List<DateTime> Expiries { get; set; }
        }

    }

}
