using i4optioncore.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Threading.Tasks;
using System;
using i4optioncore.DBModelsMaster;
using System.Collections.Generic;
using i4optioncore.Models;

namespace i4optioncore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IImportBL importBL;

        public ImportController(IImportBL importBL)
        {
            this.importBL = importBL;
        }

        [Route("52WeekHighLow"), HttpPost]
        public IActionResult _52WeekHighLow([FromBody] List<_52weekHighLow> request)
        {
            try
            {
                importBL.Import52WeekData(request);
                return Ok(new { result = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [Route("EarningRatio"), HttpPost]
        public IActionResult EarningRatio([FromBody] ImportModel.EarningRatioRequest request)
        {
            try
            {
                importBL.ImportEarningRatioData(request);
                return Ok(new { result = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
