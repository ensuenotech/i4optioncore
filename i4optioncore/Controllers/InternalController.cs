using DocumentFormat.OpenXml.Office2013.PowerPoint;
using i4optioncore.DBModels;
using i4optioncore.DBModelsMaster;
using i4optioncore.Models;
using i4optioncore.Repositories;
using i4optioncore.Repositories.Dhan;
using i4optioncore.Repositories.UpdateRedis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace i4optioncore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InternalController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("success");
        }
        private readonly IRedisBL redisBL;
        private readonly ICommonBL commonBL;
        private readonly IKiteBL kiteBL;
        private readonly IUserBL userBL;
        private readonly IStocksBL stocksBL;
        private readonly IDhanBL dhanBL;
        private readonly IUpdateBL updateBL;
        public InternalController(ICommonBL _commonBL, IRedisBL _redisBL, IUserBL userBL, IKiteBL kiteBL, IStocksBL stocksBL, IUpdateBL updateBL, IDhanBL dhanBL)
        {
            commonBL = _commonBL;
            redisBL = _redisBL;
            this.userBL = userBL;
            this.kiteBL = kiteBL;
            this.stocksBL = stocksBL;
            this.updateBL = updateBL;
            this.dhanBL = dhanBL;
        }
        //This method is used by syncup app to update the touchline based on symbols received.
        [Route("UpdateTouchline"), HttpPost]
        public async Task<IActionResult> UpdateTouchlineRedis([FromBody] List<string> Symbols)
        {
            Symbols = Symbols.Distinct().ToList();
            try
            {
                await updateBL.UpdateTouchlineRedis(Symbols);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UpdateSymbolList"), HttpGet]
        public async Task<IActionResult> UpdateSymbolList()
        {
            try
            {
                if (await IsTodayHoliday(DateTime.Now)) return Ok("HOLIDAY");
                await updateBL.UpdateSymbolList();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UpdateFullTouchline"), HttpGet]
        public async Task<IActionResult> UpdateFullTouchline()
        {
            try
            {
                await updateBL.UpdateFullTouchlineRedis();
                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
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
        [Route("GetTradingSymbols"), HttpGet]
        public async Task<IActionResult> GetTradingSymbols()
        {
            try
            {
                var _result = new List<CommonModel.SymbolDetails>();
                var key = "AllTradingSymbols";
                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                { _result = JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(redisValue); }
                else
                {
                    _result = (await commonBL.GetTouchlineSymbols());
                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                }
                return Ok(_result.Select(x => new { x.Symbol, x.TradingSymbol, x.SymbolId, x.Expiry, x.Strike }).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetStocks"), HttpGet]
        public async Task<IActionResult> GetStocks()
        {
            try
            {
                var _result = new List<CommonModel.StockDetails>();
                var key = "GetStocks";
                var redisValue = await redisBL.GetValue(key);
                if (redisValue != null)
                { _result = JsonConvert.DeserializeObject<List<CommonModel.StockDetails>>(redisValue); }
                else
                {
                    _result = (await commonBL.GetStocks());
                    await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
                }
                return Ok(_result.Select(x => new { x.DisplayName, x.Name, x.Id, x.CalendarId, x.Expiry }).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("TodayOpenTrades"), HttpGet]
        public async Task<IActionResult> TodayOpenTrades()
        {
            try
            {
                var result = await userBL.GetTodayTrades();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("SnapshotREDIS"), HttpGet]
        public async Task<IActionResult> UpdateSnapshot()
        {
            try
            {
                await updateBL.UpdateSnapshotRedis();
                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Test"), HttpGet]
        public async Task<IActionResult> Test()
        {
            try
            {
                //var _result = commonBL.SendMsg91Email("wlecome_1", "TEST USER 1", "TEST USER 2", "ensuenotechnologies@gmail.com", "TESTM");
                var _result = kiteBL.GetHistory("NIFTY 50", new DateTime(2022, 5, 1), new DateTime(2022, 6, 30), "minute");
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("datafeed"), HttpGet]
        public async Task<IActionResult> DataFeed([FromQuery] FeedQueryParameters parameters)
        {
            try
            {
                var res = await commonBL.GetTouchlineByDate([parameters.identifier], parameters.startdate, parameters.enddate);
                var result = res.Select(x => new { Close = x.Ltp, x.High, x.Low, x.Open, Volume = x.TotalVolume, DT = x.LastUpdatedTime });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }


        }
        
        [Route("UpdateTouchlineByDate"), HttpPost]
        public async Task<IActionResult> UpdateTouchline([FromBody] DateTime date)
        {
            try
            {
                if (await IsTodayHoliday(date)) return Ok("HOLIDAY");
                await updateBL.UpdateTouchline(date);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
      
        [Route("UpdateVolatility"), HttpPost]
        public async Task<IActionResult> UpdateVolatility([FromBody] DateTime date)
        {
            try
            {
                if (await IsTodayHoliday(date)) return Ok("HOLIDAY");
                await updateBL.UpdateVolatility(date);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UpdateMaxPain"), HttpPost]
        public async Task<IActionResult> UpdateMaxPain()
        {
            try
            {
                await updateBL.UpdateMaxPain();
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UpdatePCR"), HttpPost]
        public async Task<IActionResult> UpdatePCR()
        {
            try
            {
                await updateBL.UpdatePCR();
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("updatedhantoken"), HttpGet]
        public async Task<IActionResult> UpdateDhanToken([FromQuery] string tokenId)
        {
            try
            {

                await dhanBL.UpdateDhanToken(tokenId);
                //return Ok();
                return Redirect("https://dashboard.i4option.com/dashboard/connect");
            }

            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("updatedhanorders"), HttpPost]
        public async Task<IActionResult> UpdateDhanOrders([FromBody] DhanModel.OrderDetails request)
        {
            try
            {

                await dhanBL.UpdateDhanOrder(request);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DhanConsent"), HttpGet]
        public async Task<IActionResult> GetDhanConsent()
        {
            var value = await dhanBL.GenerateDhanConsent();
            if (value != null)
            {
                return Ok(new { value });
            }
            return NotFound();
        }
        [Route("DownloadDhanSymbols"), HttpGet]
        public async Task<IActionResult> DownloadDhanSymbols()
        {
            await dhanBL.DownloadDhanSymbols();
            return Ok();
        }
        [Route("ValidateInternal"), HttpPost]
        public ActionResult ValidateInternal([FromBody] IValidateInternal request)
        {
            try
            {
                var res =  userBL.ValidateInternal(request.username, request.password);
                return Ok(res);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("BulkExecuteOrders"), HttpPost]
        public async Task<IActionResult> BulkExecuteOrders([FromBody] List<BulkTradeOrdersRequest> orders)
        {
            try
            {
                var _result = await userBL.BulkExecuteTradeOrders(orders);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("UpdateAllEOD"), HttpPost]
        public async Task<IActionResult> UpdateAllEOD([FromBody] DateTime date)
        {
            try
            {
                if (await IsTodayHoliday(date)) return Ok("HOLIDAY");
                if (date.Date == DateTime.Now.Date)
                {
                    using MasterdataDbContext dbMaster = new();
                    dbMaster.Database.SetCommandTimeout(900);
                    dbMaster.Database.ExecuteSqlInterpolated($"EXEC SPROC_MASTERSYNCTOUCHLINEDATA");
                }
                await updateBL.UpdateTouchline(date);
                await updateBL.UpdateSegmentTouchline(date);
                await updateBL.UpdateEODSegment(date);
                await updateBL.UpdateEOD(date);
                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("CopyTouchlineData"), HttpPost]
        public async Task<IActionResult> CopyTouchlineByDate([FromBody] DateTime date)
        {
            try
            {
                if (await IsTodayHoliday(date)) return Ok("HOLIDAY");

                if (date.Date == DateTime.Now.Date)
                {
                    using MasterdataDbContext dbMaster = new();
                    dbMaster.Database.SetCommandTimeout(900);
                    dbMaster.Database.ExecuteSqlInterpolated($"EXEC SPROC_COPYMASTERTOUCHLINEDATA @Date={date.Date}");
                }
                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private async Task<bool> IsTodayHoliday(DateTime date)
        {
            var holidays = await commonBL.GetHolidays();
            return holidays.Any(h => h.Date == date.Date);
        }

        public class FeedQueryParameters
        {
            public string identifier { get; set; }
            public DateTime startdate { get; set; }
            public DateTime enddate { get; set; }
            public string interval { get; set; }
        }
        public class IValidateInternal
        {
            public string username { get; set; }
            public string password { get; set; }
        }
    }
}