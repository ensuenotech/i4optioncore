using i4optioncore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using i4optioncore.Repositories.UpdateRedis;

namespace i4optioncore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly IUpdateBL updateBL;

        public UpdateController(IUpdateBL updateBL)
        {
            this.updateBL = updateBL;
        }

        [Route("UpdateIV"), HttpPost]
        public async Task<IActionResult> UpdateIV([FromBody] List<CommonModel.IVRequest> req)
        {
            try
            {
                var _result = await updateBL.UpdateIV(req);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UpdateIVPIVR"), HttpPost]
        public async Task<IActionResult> UpdateIVPIVR([FromBody] DateTime req)
        {
            try
            {
                var _result = await updateBL.UpdateIVPIVR(req);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UpdateCommentary"), HttpGet]
        public async Task<IActionResult> UpdateVolumeCommentary()
        {
            try
            {
                await updateBL.UpdateVolumeCommentary();
                await updateBL.UpdateSpotVolumeCommentary();
                await updateBL.UpdateBreadth();
                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UpdateEODDATA"), HttpGet]
        public async Task<IActionResult> UpdateEODDATA()
        {
            try
            {
                await updateBL.UpdateRedisFOREODDATA();
                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("CopyMaster"), HttpGet]
        public IActionResult CopyMaster()
        {
            try
            {
                updateBL.CopyMasterSync();
                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UpdateFutureRollover"), HttpGet]
        public async Task<IActionResult> UpdateFutureRollover()
        {
            try
            {
                await updateBL.UpdateFutureRollover();
                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
