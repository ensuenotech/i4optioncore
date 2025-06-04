using i4optioncore.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static i4optioncore.Controllers.CommonController;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.OutputCaching;

namespace i4optioncore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStocksBL stocksBL;

        public StocksController(IStocksBL stocksBL)
        {
            this.stocksBL = stocksBL;
        }
        [Route("OptionsTouchline"), HttpPost]
        [OutputCache(Duration = 60 * 60 * 12)]
        public async Task<IActionResult> GetOptionTouchline([FromBody] OptionsTouchlineRequest request)
        {
            try
            {
                return Ok(await stocksBL.GetOptionTouchline(request.Symbol, request.Date));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [Route("OptionsHistory"), HttpPost]
        [OutputCache(Duration = 60 * 60 * 12)]
        public async Task<IActionResult> GetOptionsHistory([FromBody] OptionsTouchlineRequest request)
        {
            try
            {
                return Ok(await stocksBL.GetOptionHistory(request.Symbol, request.Date));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [Route("History"), HttpPost]
        [OutputCache(Duration = 60 * 60 * 12)]
        public async Task<IActionResult> GetHistory([FromBody] HistoryRequest request)
        {
            try
            {
                return Ok(await stocksBL.GetHistory(request.Symbols, request.Date, request.Type));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("FutureRollOver"), HttpPost]
        [OutputCache(Duration = 60 * 60 * 12)]
        public async Task<IActionResult> GetFutureRollOver()
        {
            try
            {
                return Ok(await stocksBL.GetFutureRollOver());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
    public class OptionsTouchlineRequest
    {
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
    }
    public class HistoryRequest
    {
        public List<string> Symbols { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = "daily";
    }
}
