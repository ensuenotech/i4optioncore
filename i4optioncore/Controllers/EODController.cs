using i4optioncore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using i4optioncore.Repositories.EOD;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.OutputCaching;
using i4optioncore.Repositories;
using static i4optioncore.Controllers.StatisticsResponse;

namespace i4optioncore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EODController : ControllerBase
    {
        private readonly IEODBL eodBL;
        private readonly ICacheBL cacheBL;
        public EODController(IEODBL eodBL,
ICacheBL cacheBL)
        {
            this.eodBL = eodBL;
            this.cacheBL = cacheBL;
        }

        [Route("EODScreener"), HttpPost]
        public IActionResult EODScreener([FromBody] List<IEODScreenerDataRequest> request)
        {
            try
            {
                var res = eodBL.EODScreener(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("EODScan"), HttpPost]
        public async Task<IActionResult> EODScan([FromBody] EODScanRequest request)
        {
            try
            {
                await eodBL.SaveEODScan(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DeleteEODScan"), HttpPost]
        public async Task<IActionResult> DeleteEODScan([FromBody] int id)
        {
            try
            {
                await eodBL.DeleteEODScan(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetEODScan"), HttpPost]
        public async Task<IActionResult> EODScan([FromBody] int userId)
        {
            try
            {
                return Ok(await eodBL.GetEODScan(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("EarningsData"), HttpPost]
        //[OutputCache(Duration = 60 * 1 * 24)]
        public async Task<IActionResult> EarningsData([FromBody] string symbol)
        {
            try
            {

                if (!string.IsNullOrEmpty(cacheBL.GetValue($"Earnings_{symbol}")))
                    return Ok(JsonConvert.DeserializeObject<Earnings.EarningsResponse>(cacheBL.GetValue($"Earnings_{symbol}")));

                // Replace these values with your actual API key and other parameters
                string apiKey = "d1bc1f0e9f5c4385b23f040b15c288a1";
                string exchange = "NSE";

                // URL for the API endpoint
                string apiUrl = $"https://api.twelvedata.com/earnings?symbol={symbol}&exchange={exchange}&apikey={apiKey}&source=docs";

                using (HttpClient httpClient = new HttpClient())
                {
                    try
                    {
                        // Send a POST request
                        HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the content as a string
                            string result = await response.Content.ReadAsStringAsync();

                            cacheBL.SetValue($"Earnings_{symbol}", result);
                            return Ok(JsonConvert.DeserializeObject<Earnings.EarningsResponse>(result));

                        }
                        else
                        {
                            // Handle the error, if any
                            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        throw new Exception($"Exception: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("StatisticsData"), HttpPost]
        //[OutputCache(Duration = 60 * 1 * 24)]
        public async Task<IActionResult> StatisticsData([FromBody] string symbol)
        {
            try
            {

                if (!string.IsNullOrEmpty(cacheBL.GetValue($"StatisticsResponse_{symbol}")))

                    return Ok(JsonConvert.DeserializeObject<StatisticsResponse.Root>(cacheBL.GetValue($"StatisticsResponse_{symbol}")));
                // Replace these values with your actual API key and other parameters
                string apiKey = "d1bc1f0e9f5c4385b23f040b15c288a1";
                string exchange = "NSE";

                // URL for the API endpoint
                string apiUrl = $"https://api.twelvedata.com/statistics?symbol={symbol}&exchange={exchange}&apikey={apiKey}&source=docs";

                using HttpClient httpClient = new();
                try
                {
                    // Send a POST request
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the content as a string
                        string result = await response.Content.ReadAsStringAsync();
                        var finalResult = JsonConvert.DeserializeObject<Root>(result);
                        cacheBL.SetValue($"StatisticsResponse_{symbol}", result);
                        return Ok(finalResult);

                    }
                    else
                    {
                        // Handle the error, if any
                        throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    throw new Exception($"Exception: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("IncomeStatementData"), HttpPost]
        //[OutputCache(Duration = 60 * 1 * 24)]
        public async Task<IActionResult> IncomeStatementData([FromBody] string symbol)
        {
            try
            {

                if (!string.IsNullOrEmpty(cacheBL.GetValue($"IncomeStatement_{symbol}")))
                    return Ok(JsonConvert.DeserializeObject<IncomeStatementResponse.Root>(cacheBL.GetValue($"IncomeStatement_{symbol}")));

                // Replace these values with your actual API key and other parameters
                string apiKey = "d1bc1f0e9f5c4385b23f040b15c288a1";
                string exchange = "NSE";

                // URL for the API endpoint
                string apiUrl = $"https://api.twelvedata.com/income_statement?symbol={symbol}&exchange={exchange}&apikey={apiKey}&source=docs";

                using (HttpClient httpClient = new HttpClient())
                {
                    try
                    {
                        // Send a POST request
                        HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the content as a string
                            string result = await response.Content.ReadAsStringAsync();

                            cacheBL.SetValue($"IncomeStatement_{symbol}", result);
                            return Ok(JsonConvert.DeserializeObject<IncomeStatementResponse.Root>(result));

                        }
                        else
                        {
                            // Handle the error, if any
                            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        throw new Exception($"Exception: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("BalanceSheetData"), HttpPost]
        //[OutputCache(Duration = 60 * 1 * 24)]
        public async Task<IActionResult> BalanceSheetData([FromBody] string symbol)
        {
            try
            {

                if (!string.IsNullOrEmpty(cacheBL.GetValue($"BalanceSheet_{symbol}")))
                    return Ok(JsonConvert.DeserializeObject<BalanceSheetResponse.Root>(cacheBL.GetValue($"BalanceSheet_{symbol}")));

                // Replace these values with your actual API key and other parameters
                string apiKey = "d1bc1f0e9f5c4385b23f040b15c288a1";
                string exchange = "NSE";

                // URL for the API endpoint
                string apiUrl = $"https://api.twelvedata.com/balance_sheet?symbol={symbol}&exchange={exchange}&apikey={apiKey}&source=docs";

                using (HttpClient httpClient = new HttpClient())
                {
                    try
                    {
                        // Send a POST request
                        HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the content as a string
                            string result = await response.Content.ReadAsStringAsync();

                            cacheBL.SetValue($"BalanceSheet_{symbol}", result);
                            return Ok(JsonConvert.DeserializeObject<BalanceSheetResponse.Root>(result));

                        }
                        else
                        {
                            // Handle the error, if any
                            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        throw new Exception($"Exception: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("CashflowData"), HttpPost]
        //[OutputCache(Duration = 60 * 1 * 24)]
        public async Task<IActionResult> CashflowData([FromBody] string symbol)
        {
            try
            {

                if (!string.IsNullOrEmpty(cacheBL.GetValue($"CashflowData_{symbol}")))
                    return Ok(JsonConvert.DeserializeObject<CashflowResponse.RootCashFlow>(cacheBL.GetValue($"CashflowData_{symbol}")));

                // Replace these values with your actual API key and other parameters
                string apiKey = "d1bc1f0e9f5c4385b23f040b15c288a1";
                string exchange = "NSE";

                // URL for the API endpoint
                string apiUrl = $"https://api.twelvedata.com/cash_flow?symbol={symbol}&exchange={exchange}&apikey={apiKey}&source=docs";

                using (HttpClient httpClient = new HttpClient())
                {
                    try
                    {
                        // Send a POST request
                        HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the content as a string
                            string result = await response.Content.ReadAsStringAsync();

                            cacheBL.SetValue($"CashflowData_{symbol}", result);
                            return Ok(JsonConvert.DeserializeObject<CashflowResponse.RootCashFlow>(result));

                        }
                        else
                        {
                            // Handle the error, if any
                            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        throw new Exception($"Exception: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("AboutData"), HttpPost]
        [OutputCache(Duration = 60 * 1 * 24)]
        public async Task<IActionResult> AboutData([FromBody] string symbol)
        {
            try
            {

                if (!string.IsNullOrEmpty(cacheBL.GetValue($"AboutData_{symbol}")))
                    return Ok(JsonConvert.DeserializeObject<About>(cacheBL.GetValue($"AboutData_{symbol}")));

                // Replace these values with your actual API key and other parameters
                string apiKey = "d1bc1f0e9f5c4385b23f040b15c288a1";
                string exchange = "NSE";

                // URL for the API endpoint
                string apiUrl = $"https://api.twelvedata.com/profile?symbol={symbol}&exchange={exchange}&apikey={apiKey}&source=docs";

                using (HttpClient httpClient = new HttpClient())
                {
                    try
                    {
                        // Send a POST request
                        HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the content as a string
                            string result = await response.Content.ReadAsStringAsync();

                            cacheBL.SetValue($"AboutData_{symbol}", result);
                            return Ok(JsonConvert.DeserializeObject<About>(result));

                        }
                        else
                        {
                            // Handle the error, if any
                            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        throw new Exception($"Exception: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
    public class Earnings
    {
        public class Meta
        {
            public string Symbol { get; set; }
            public string Name { get; set; }
            public string Currency { get; set; }
            public string Exchange { get; set; }
            public string MicCode { get; set; }
            public string ExchangeTimezone { get; set; }
        }

        public class Earning
        {
            public string Date { get; set; }
            public string Time { get; set; }
            public double? EpsEstimate { get; set; }
            public double? EpsActual { get; set; }
            public double? Difference { get; set; }
            public double? SurprisePrc { get; set; }
        }

        public class EarningsResponse
        {
            public Meta Meta { get; set; }
            public List<Earning> Earnings { get; set; }
            public string Status { get; set; }
        }
    }
    public class IncomeStatementResponse
    {
        public class Meta
        {
            public string Symbol { get; set; }
            public string Name { get; set; }
            public string Currency { get; set; }
            public string Exchange { get; set; }
            public string Mic_Code { get; set; }
            public string Exchange_Timezone { get; set; }
            public string Period { get; set; }
        }

        public class OperatingExpense
        {
            public object Research_And_Development { get; set; }
            public long? Selling_General_And_Administrative { get; set; }
            public long? Other_Operating_Expenses { get; set; }
        }

        public class NonOperatingInterest
        {
            public long? Income { get; set; }
            public long? Expense { get; set; }
        }

        public class IncomeStatement
        {
            public DateTime Fiscal_Date { get; set; }
            public long? Sales { get; set; }
            public long? Cost_Of_Goods { get; set; }
            public long? Gross_Profit { get; set; }
            public OperatingExpense Operating_Expense { get; set; }
            public long? Operating_Income { get; set; }
            public NonOperatingInterest Non_Operating_Interest { get; set; }
            public long? Other_Income_Expense { get; set; }
            public long? Pretax_Income { get; set; }
            public long? Income_Tax { get; set; }
            public long? Net_Income { get; set; }
            public double Eps_Basic { get; set; }
            public double Eps_Diluted { get; set; }
            public long? Basic_Shares_Outstanding { get; set; }
            public long? Diluted_Shares_Outstanding { get; set; }
            public long? Ebit { get; set; }
            public long? Ebitda { get; set; }
            public long? Net_Income_Continuous_Operations { get; set; }
            public long? Minority_Interests { get; set; }
            public object Preferred_Stock_Dividends { get; set; }
        }

        public class Root
        {
            public Meta Meta { get; set; }
            public List<IncomeStatement> Income_Statement { get; set; }
        }
    }
    public class StatisticsResponse
    {
        public class _Meta
        {
            public string symbol { get; set; }
            public string name { get; set; }
            public string currency { get; set; }
            public string exchange { get; set; }
            public string mic_code { get; set; }
            public string exchange_timezone { get; set; }
        }

        public class _Statistics
        {
            public _ValuationsMetrics valuations_metrics { get; set; }
            public _Financials financials { get; set; }
            public _StockStatistics stock_statistics { get; set; }
            public _StockPriceSummary stock_price_summary { get; set; }
            public _DividendsAndSplits dividends_and_splits { get; set; }
        }

        public class _ValuationsMetrics
        {
            public long market_capitalization { get; set; }
            public long enterprise_value { get; set; }
            public object trailing_pe { get; set; }
            public object forward_pe { get; set; }
            public object peg_ratio { get; set; }
            public object price_to_sales_ttm { get; set; }
            public object price_to_book_mrq { get; set; }
            public object enterprise_to_revenue { get; set; }
            public object enterprise_to_ebitda { get; set; }
        }

        public class _Financials
        {
            public string fiscal_year_ends { get; set; }
            public string most_recent_quarter { get; set; }
            public double profit_margin { get; set; }
            public double operating_margin { get; set; }
            public double? return_on_assets_ttm { get; set; }
            public double return_on_equity_ttm { get; set; }
            public _IncomeStatement income_statement { get; set; }
            public _BalanceSheet balance_sheet { get; set; }
            public _CashFlow cash_flow { get; set; }
        }

        public class _IncomeStatement
        {
            public long revenue_ttm { get; set; }
            public double revenue_per_share_ttm { get; set; }
            public double quarterly_revenue_growth { get; set; }
            public long gross_profit_ttm { get; set; }
            public long ebitda { get; set; }
            public long net_income_to_common_ttm { get; set; }
            public double diluted_eps_ttm { get; set; }
            public double quarterly_earnings_growth_yoy { get; set; }
        }

        public class _BalanceSheet
        {
            public long total_cash_mrq { get; set; }
            public double total_cash_per_share_mrq { get; set; }
            public long total_debt_mrq { get; set; }
            public double total_debt_to_equity_mrq { get; set; }
            public double? current_ratio_mrq { get; set; }
            public double book_value_per_share_mrq { get; set; }
        }

        public class _CashFlow
        {
            public long operating_cash_flow_ttm { get; set; }
            public long levered_free_cash_flow_ttm { get; set; }
        }

        public class _StockStatistics
        {
            public long shares_outstanding { get; set; }
            public long float_shares { get; set; }
            public long avg_10_volume { get; set; }
            public long avg_30_volume { get; set; }
            public object shares_short { get; set; }
            public object short_ratio { get; set; }
            public object short_percent_of_shares_outstanding { get; set; }
            public double percent_held_by_insiders { get; set; }
            public double percent_held_by_institutions { get; set; }
        }

        public class _StockPriceSummary
        {
            public double fifty_two_week_low { get; set; }
            public double fifty_two_week_high { get; set; }
            public double fifty_two_week_change { get; set; }
            public double beta { get; set; }
            public double day_50_ma { get; set; }
            public double day_200_ma { get; set; }
        }

        public class _DividendsAndSplits
        {
            public long forward_annual_dividend_rate { get; set; }
            public double forward_annual_dividend_yield { get; set; }
            public long trailing_annual_dividend_rate { get; set; }
            public double trailing_annual_dividend_yield { get; set; }
            public double _5_year_average_dividend_yield { get; set; }
            public double payout_ratio { get; set; }
            public object dividend_date { get; set; }
            public string ex_dividend_date { get; set; }
            public string last_split_factor { get; set; }
            public string last_split_date { get; set; }
        }
        public class Root
        {
            public _Meta Meta { get; set; }
            public _Statistics Statistics { get; set; }
        }
    }
    public class BalanceSheetResponse
    {
        public class _Meta
        {
            public string symbol { get; set; }
            public string name { get; set; }
            public string currency { get; set; }
            public string exchange { get; set; }
            public string mic_code { get; set; }
            public string exchange_timezone { get; set; }
            public string period { get; set; }
        }

        public class _BalanceSheet
        {
            public string fiscal_date { get; set; }
            public _Assets assets { get; set; }
            public _Liabilities liabilities { get; set; }
            public _ShareholdersEquity shareholders_equity { get; set; }
        }

        public class _Assets
        {
            public _CurrentAssets current_assets { get; set; }
            public _NonCurrentAssets non_current_assets { get; set; }
            public long? total_assets { get; set; }
        }

        public class _CurrentAssets
        {
            public long? cash { get; set; }
            public long? cash_equivalents { get; set; }
            public long? cash_and_cash_equivalents { get; set; }
            public long? other_short_term_investments { get; set; }
            public long? accounts_receivable { get; set; }
            public long? other_receivables { get; set; }
            public object inventory { get; set; }
            public long? prepaid_assets { get; set; }
            public object restricted_cash { get; set; }
            public object assets_held_for_sale { get; set; }
            public object hedging_assets { get; set; }
            public long? other_current_assets { get; set; }
            public long? total_current_assets { get; set; }
        }

        public class _NonCurrentAssets
        {
            public long? properties { get; set; }
            public long? land_and_improvements { get; set; }
            public long? machinery_furniture_equipment { get; set; }
            public long? construction_in_progress { get; set; }
            public object leases { get; set; }
            public long? accumulated_depreciation { get; set; }
            public long? goodwill { get; set; }
            public object investment_properties { get; set; }
            public object financial_assets { get; set; }
            public long? intangible_assets { get; set; }
            public long? investments_and_advances { get; set; }
            public long? other_non_current_assets { get; set; }
            public long? total_non_current_assets { get; set; }
        }

        public class _Liabilities
        {
            public _CurrentLiabilities current_liabilities { get; set; }
            public _NonCurrentLiabilities non_current_liabilities { get; set; }
            public long? total_liabilities { get; set; }
        }

        public class _CurrentLiabilities
        {
            public long? accounts_payable { get; set; }
            public long? accrued_expenses { get; set; }
            public long? short_term_debt { get; set; }
            public long? deferred_revenue { get; set; }
            public long? tax_payable { get; set; }
            public long? pensions { get; set; }
            public long? other_current_liabilities { get; set; }
            public long? total_current_liabilities { get; set; }
        }

        public class _NonCurrentLiabilities
        {
            public object long_term_provisions { get; set; }
            public long? long_term_debt { get; set; }
            public object provision_for_risks_and_charges { get; set; }
            public long? deferred_liabilities { get; set; }
            public long? derivative_product_liabilities { get; set; }
            public long? other_non_current_liabilities { get; set; }
            public long? total_non_current_liabilities { get; set; }
        }

        public class _ShareholdersEquity
        {
            public long? common_stock { get; set; }
            public long? retained_earnings { get; set; }
            public long? other_shareholders_equity { get; set; }
            public long? total_shareholders_equity { get; set; }
            public long? additional_paid_in_capital { get; set; }
            public object treasury_stock { get; set; }
            public long? minority_interest { get; set; }
        }
        public class Root
        {
            public _Meta Meta { get; set; }
            public List<_BalanceSheet> Balance_Sheet { get; set; }
        }
    }
    public class CashflowResponse
    {
        public class OperatingActivities
        {
            public long? Net_Income { get; set; }
            public long? Depreciation { get; set; }
            public long? Deferred_Taxes { get; set; }
            public long? Stock_Based_Compensation { get; set; }
            public long? Other_Non_Cash_Items { get; set; }
            public long? Accounts_Receivable { get; set; }
            public long? Accounts_Payable { get; set; }
            public long? Other_Assets_Liabilities { get; set; }
            public long? Operating_Cash_Flow { get; set; }
        }

        public class InvestingActivities
        {
            public object Capital_Expenditures { get; set; }
            public object Net_Intangibles { get; set; }
            public long? Net_Acquisitions { get; set; }
            public long? Purchase_Of_Investments { get; set; }
            public long? Sale_Of_Investments { get; set; }
            public long? Other_Investing_Activity { get; set; }
            public long? Investing_Cash_Flow { get; set; }
        }

        public class FinancingActivities
        {
            public object Long_Term_Debt_Issuance { get; set; }
            public long? Long_Term_Debt_Payments { get; set; }
            public object Short_Term_Debt_Issuance { get; set; }
            public object Common_Stock_Issuance { get; set; }
            public long? Common_Stock_Repurchase { get; set; }
            public long? Common_Dividends { get; set; }
            public long? Other_Financing_Charges { get; set; }
            public long? Financing_Cash_Flow { get; set; }
        }

        public class CashFlowEntry
        {
            public DateTime Fiscal_Date { get; set; }
            public OperatingActivities Operating_Activities { get; set; }
            public InvestingActivities Investing_Activities { get; set; }
            public FinancingActivities Financing_Activities { get; set; }
            public long? End_Cash_Position { get; set; }
            public object Income_Tax_Paid { get; set; }
            public object Interest_Paid { get; set; }
            public long? Free_Cash_Flow { get; set; }
        }

        public class MetaCashFlow
        {
            public string Symbol { get; set; }
            public string Name { get; set; }
            public string Currency { get; set; }
            public string Exchange { get; set; }
            public string Mic_Code { get; set; }
            public string Exchange_Timezone { get; set; }
            public string Period { get; set; }
        }

        public class RootCashFlow
        {
            public MetaCashFlow Meta { get; set; }
            public List<CashFlowEntry> Cash_Flow { get; set; }
        }
    }

    public class About
    {

        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Exchange { get; set; }
        public string Mic_Code { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public int Employees { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string CEO { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }


    }

}
