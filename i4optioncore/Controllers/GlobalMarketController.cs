using i4optioncore.Models;
using i4optioncore.Repositories;
using i4optioncore.Repositories.GlobalMarket;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TwelveDataSharp.Library.ResponseModels;
using Microsoft.AspNetCore.OutputCaching;

namespace i4optioncore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [OutputCache(Duration = 60 * 15)]
    public class GlobalMarketController : ControllerBase
    {
        private readonly IGlobalMarketBL globalMarketBL;
        public GlobalMarketController(IGlobalMarketBL globalMarketBL)
        {
            this.globalMarketBL = globalMarketBL;
        }
        [Route("Quote"), HttpGet]
        [OutputCache(Duration = 60 * 5 * 24)]
        public async Task<IActionResult> Quote()
        {
            try
            {
                //List<TwelveDataQuote> quotes = new();
                //foreach (var symbol in symbols)
                //{
                //quotes.Add(await globalMarketBL.GetQuote());
                //}
                return Ok(await globalMarketBL.GetQuote());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Currency"), HttpGet]
        [OutputCache(Duration = 60 * 5 * 24)]
        public async Task<IActionResult> GetCurrency()
        {
            try
            {
                return Ok(await globalMarketBL.GetCurrencies());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Commodities"), HttpGet]
        [OutputCache(Duration = 60 * 5)]
        public async Task<IActionResult> Commodities()
        {
            try
            {

                return Ok(await globalMarketBL.GetCommodities());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Bonds"), HttpGet]
        [OutputCache(Duration = 60 * 5)]
        public async Task<IActionResult> Bonds()
        {
            try
            {

                return Ok(await globalMarketBL.GetBonds());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("IndianAdrs"), HttpGet]
        [OutputCache(Duration = 60 * 5)]
        public async Task<IActionResult> IndianAdrs()
        {
            try
            {

                return Ok(await globalMarketBL.GetADRs());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
