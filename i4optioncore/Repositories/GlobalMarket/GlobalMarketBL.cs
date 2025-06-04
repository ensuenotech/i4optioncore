using DocumentFormat.OpenXml.Office2010.ExcelAc;
using i4optioncore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TwelveDataSharp;
using TwelveDataSharp.Interfaces;
using TwelveDataSharp.Library.ResponseModels;

namespace i4optioncore.Repositories.GlobalMarket
{
    public class GlobalMarketBL : IGlobalMarketBL
    {
        private readonly IRedisBL redisBL;
        private readonly ICommonBL commonBL;
        public GlobalMarketBL(IRedisBL redisBL, ICommonBL commonBL)
        {
            this.redisBL = redisBL;
            this.commonBL = commonBL;
        }

        private readonly string twelveDataApiKey = "d1bc1f0e9f5c4385b23f040b15c288a1";

        public async Task<List<MarketData>> GetQuote()
        {
            try
            {
                var url = "https://priceapi.moneycontrol.com/technicalCompanyData/globalMarket/getGlobalIndicesListingData?view=overview&deviceType=W";
                using HttpClient _httpClient = new HttpClient();
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Ensure we got a successful response

                string jsonString = await response.Content.ReadAsStringAsync();
                var rawData = JsonConvert.DeserializeObject<MarketDataResponse>(jsonString);
                var result = new List<MarketData>();
                if (rawData?.DataList != null && rawData.Header != null)
                {
                    foreach (var marketGroup in rawData.DataList)
                    {

                        var mappedDataItems = new List<MarketDataItem>();
                        foreach (var dataItemArray in marketGroup.Data)
                        {
                            var dataItem = new MarketDataItem
                            {
                                Symbol = dataItemArray[rawData.Header.FindIndex(x => x.Name == "symbol")],
                                Name = dataItemArray[rawData.Header.FindIndex(x => x.Name == "name")],
                                Price = dataItemArray[rawData.Header.FindIndex(x => x.Name == "price")],
                                NetChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "net_change")],
                                PercentChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "percent_change")],
                                High = dataItemArray[rawData.Header.FindIndex(x => x.Name == "high")],
                                Low = dataItemArray[rawData.Header.FindIndex(x => x.Name == "low")],
                                Open = dataItemArray[rawData.Header.FindIndex(x => x.Name == "open")],
                                PrevClose = dataItemArray[rawData.Header.FindIndex(x => x.Name == "prev_close")],
                                FiftyTwoWkHigh = dataItemArray[rawData.Header.FindIndex(x => x.Name == "52wkHigh")],
                                FiftyTwoWkLow = dataItemArray[rawData.Header.FindIndex(x => x.Name == "52wkLow")],
                                WeeklyPerChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "weekly_per_change")],
                                MonthlyPerChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "monthly_per_change")],
                                ThreeMonthsPerChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "3months_per_change")],
                                SixMonthsPerChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "6months_per_change")],
                                YtdPerChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "ytd_per_change")],
                                YearlyPerChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "yearly_per_change")],
                                TwoYearsPerChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "2years_per_change")],
                                ThreeYearsPerChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "3years_per_change")],
                                FiveYearsPerChange = dataItemArray[rawData.Header.FindIndex(x => x.Name == "5years_per_change")],
                                TechnicalRating = dataItemArray[rawData.Header.FindIndex(x => x.Name == "technical_rating")],
                                LastUpdated = dataItemArray[rawData.Header.FindIndex(x => x.Name == "last_updated")],
                                FlagUrl = dataItemArray[rawData.Header.FindIndex(x => x.Name == "flag_url")],
                                State = dataItemArray[rawData.Header.FindIndex(x => x.Name == "state")],
                                IsDerived = dataItemArray[rawData.Header.FindIndex(x => x.Name == "isDerived")],
                                LinkFlag = dataItemArray[rawData.Header.FindIndex(x => x.Name == "link_flag")],
                                Message = dataItemArray[rawData.Header.FindIndex(x => x.Name == "message")]
                            };
                            mappedDataItems.Add(dataItem);
                        }

                        result.Add(new MarketData
                        {
                            Heading = marketGroup.Heading,
                            Data = mappedDataItems
                        });
                    }
                    return result;
                }

                return null; // Or throw an exception if data is missing
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error fetching data: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new Exception($"Error deserializing JSON: {ex.Message}");
            }
        }
        public async Task<List<GlobalMarketModel.Currency>> GetCurrencies()
        {

            var response = await commonBL.GETREQUEST("https://api.moneycontrol.com/mcapi/v1/us-markets/getCurrencies");
            var res = JsonConvert.DeserializeObject<GlobalMarketModel.CurrencyResponse>(response);
            if (res != null && res.Success == 1)
            {
                return res.Data;
            }
            return null;
        }
        public async Task<List<GlobalMarketModel.Commodity>> GetCommodities()
        {
            var result = new List<GlobalMarketModel.Commodity>();
            var response = await commonBL.GETREQUEST("https://priceapi.moneycontrol.com/technicalCompanyData/globalMarket/getCommodityListingData?view=overview&deviceType=W");
            var res = JsonConvert.DeserializeObject<GlobalMarketModel.MoneyControlCommodityResponse>(response);
            if (res != null)
            {
                res.DataList.ForEach(data =>
                {
                    result.Add(new GlobalMarketModel.Commodity
                    {
                        Name = data[res.Header.FindIndex(x => x.Name == "name")],
                        Symbol = data[res.Header.FindIndex(x => x.Name == "symbol")],
                        Price = data[res.Header.FindIndex(x => x.Name == "price")],
                        Net_change = data[res.Header.FindIndex(x => x.Name == "net_change")],
                        Percent_change = data[res.Header.FindIndex(x => x.Name == "percent_change")],
                        High = data[res.Header.FindIndex(x => x.Name == "high")],
                        Low = data[res.Header.FindIndex(x => x.Name == "low")],
                        Open = data[res.Header.FindIndex(x => x.Name == "open")],
                        Prev_close = data[res.Header.FindIndex(x => x.Name == "prev_close")],
                        WkHigh52 = data[res.Header.FindIndex(x => x.Name == "52wkHigh")],
                        WkLow52 = data[res.Header.FindIndex(x => x.Name == "52wkLow")],
                        Weekly_change = data[res.Header.FindIndex(x => x.Name == "weekly_change")],
                        Weekly_per_change = data[res.Header.FindIndex(x => x.Name == "weekly_per_change")],
                        Monthly_change = data[res.Header.FindIndex(x => x.Name == "monthly_change")],
                        Monthly_per_change = data[res.Header.FindIndex(x => x.Name == "monthly_per_change")],
                        Months_change3 = data[res.Header.FindIndex(x => x.Name == "3months_change")],
                        Months_per_change3 = data[res.Header.FindIndex(x => x.Name == "3months_per_change")],
                        Months_change6 = data[res.Header.FindIndex(x => x.Name == "6months_change")],
                        Months_per_change6 = data[res.Header.FindIndex(x => x.Name == "6months_per_change")],
                        Ytd_change = data[res.Header.FindIndex(x => x.Name == "ytd_change")],
                        Ytd_per_change = data[res.Header.FindIndex(x => x.Name == "ytd_per_change")],
                        Yearly_change = data[res.Header.FindIndex(x => x.Name == "yearly_change")],
                        Yearly_per_change = data[res.Header.FindIndex(x => x.Name == "yearly_per_change")],
                        Years_change2 = data[res.Header.FindIndex(x => x.Name == "2years_change")],
                        Years_per_change2 = data[res.Header.FindIndex(x => x.Name == "2years_per_change")],
                        Years_change3 = data[res.Header.FindIndex(x => x.Name == "3years_change")],
                        Years_per_change3 = data[res.Header.FindIndex(x => x.Name == "3years_per_change")],
                        Years_change5 = data[res.Header.FindIndex(x => x.Name == "5years_change")],
                        Years_per_change5 = data[res.Header.FindIndex(x => x.Name == "5years_per_change")],
                        Technical_rating = data[res.Header.FindIndex(x => x.Name == "technical_rating")],
                        Last_updated = data[res.Header.FindIndex(x => x.Name == "last_updated")]
                    });
                });
            }
            return result;

        }
        public async Task<List<GlobalMarketModel.Bond>> GetBonds()
        {
            var symbols = new List<string>()
            {
                "GIND5Y:IND","GIND10YR:IND","GIND30Y:IND","USGG10YR:IND","GCNY10YR:GOV","GJGB10:IND",
                "GUKG10:IND","GDBR10:IND","GFRN10:IND","GBTPGR10:IND","GSAB10YR:GOV","GCAN10YR:IND",
                "GEBR10Y:IND","RUGE10Y:GOV","GACGB10:IND","GVSK10YR:GOV","GMXN10YR:IND","GIDN10YR:GOV"
            };
            var result = new List<GlobalMarketModel.Bond>();


            foreach (var data in symbols)
            {
                var response = await commonBL.GETREQUEST($"https://priceapi.moneycontrol.com/pricefeed/usMarket/bond/{data}");
                var res = JsonConvert.DeserializeObject<dynamic>(response);
                if (res != null)
                {
                    result.Add(new GlobalMarketModel.Bond
                    {

                        High = res.data["high"],
                        Open = res.data["open"],
                        Low = res.data["low"],
                        Last_epoch = res.data["lastupd_epoch"],
                        Last_upd = res.data["lastupd"],
                        Market_state = res.data["market_state"],
                        Market_type = res.data["market_type"],
                        Monthly_per_change = res.data["monthly_per_change"],
                        Months_change6 = res.data["6months_change"],
                        Months_per_change6 = res.data["6months_per_change"],
                        Net_change = res.data["net_change"],
                        Percent_change = res.data["percent_change"],
                        Symbol = res.data["symbol"],
                        Weekly_change = res.data["weekly_change"],
                        Weekly_per_change = res.data["weekly_per_change"],
                        WkHigh52 = res.data["52wkHigh"],
                        WkLow52 = res.data["52wkLow"],
                        Years_change3 = res.data["3years_per_change"],
                        Years_change5 = res.data["5years_change"],
                        Years_per_change2 = res.data["2years_per_change"],
                        Years_per_change3 = res.data["3years_per_change"],
                        Ytd_per_change = res.data["ytd_per_change"],
                        Prev_close = res.data["prev_close"],
                        Current_price = res.data["current_price"],
                        Name = res.data["name"],
                    });
                }
            }

            return result;

        }
        public async Task<List<GlobalMarketModel.ADR>> GetADRs()
        {

            var response = await commonBL.GETREQUEST($"https://appfeeds.moneycontrol.com/jsonapi/market/get_indian_adrs");
            return JsonConvert.DeserializeObject<List<GlobalMarketModel.ADR>>(response);


        }

    }
}
