using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace i4optioncore.Models
{
    public class GlobalMarketModel
    {
        public class ADR
        {
            public string Shortname { get; set; }
            public string Lastprice { get; set; }
            public string Change { get; set; }
            public string Percentchange { get; set; }
            public string High { get; set; }
            public string Low { get; set; }
            public string Open { get; set; }
            public string Updated_at { get; set; }
            public string Prev_close { get; set; }
            public string Adr_class { get; set; }
            public string Direction { get; set; }
        }
        public class Bond
        {
            public string Symbol { get; set; }
            public string Weekly_per_change { get; set; }
            public string Monthly_per_change { get; set; }
            public string Ytd_per_change { get; set; }
            public string Net_change { get; set; }
            public string Weekly_change { get; set; }
            public string High { get; set; }
            public string Low { get; set; }
            public string WkLow52 { get; set; }
            public string Years_per_change3 { get; set; }
            public string Months_per_change6 { get; set; }
            public string Percent_change { get; set; }
            public string Market_type { get; set; }
            public string WkHigh52 { get; set; }
            public string Last_upd { get; set; }
            public string Years_per_change2 { get; set; }
            public string Years_change5 { get; set; }
            public string Market_state { get; set; }
            public string Months_change6 { get; set; }
            public string Years_change3 { get; set; }
            public string Last_epoch { get; set; }
            public string Prev_close { get; set; }
            public string Current_price { get; set; }
            public string Name { get; set; }
            public string Open { get; set; }
        }
        public class Commodity
        {
            public string Symbol { get; set; }
            public string Name { get; set; }
            public string Price { get; set; }
            public string Net_change { get; set; }
            public string Percent_change { get; set; }
            public string High { get; set; }
            public string Low { get; set; }
            public string Open { get; set; }
            public string Prev_close { get; set; }
            public string WkHigh52 { get; set; }
            public string WkLow52 { get; set; }
            public string Weekly_change { get; set; }
            public string Weekly_per_change { get; set; }
            public string Monthly_change { get; set; }
            public string Monthly_per_change { get; set; }
            public string Months_change3 { get; set; }
            public string Months_per_change3 { get; set; }
            public string Months_change6 { get; set; }
            public string Months_per_change6 { get; set; }
            public string Ytd_change { get; set; }
            public string Ytd_per_change { get; set; }
            public string Yearly_change { get; set; }
            public string Yearly_per_change { get; set; }
            public string Years_change2 { get; set; }
            public string Years_per_change2 { get; set; }
            public string Years_change3 { get; set; }
            public string Years_per_change3 { get; set; }
            public string Years_change5 { get; set; }
            public string Years_per_change5 { get; set; }
            public string Technical_rating { get; set; }
            public string Last_updated { get; set; }
        }
        public class MoneyControlCommodityResponse
        {
            public List<MoneyControlNameType> Header { get; set; }
            public long LastUpdated { get; set; }
            public List<string[]> DataList { get; set; }
        }
        public class MoneyControlNameType
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }
        public class MoneyControlResponse {
            public string Code { get; set; }
            public string Message { get; set; }
            public Bond Data { get; set; }
        }
        public class CurrencyResponse
        {
            public int Success { get; set; }
            public List<Currency> Data { get; set; }
        }
        public class Currency
        {
            public string Name { get; set; }
            public string Open { get; set; }
            public string High { get; set; }
            public string Low { get; set; }
            public string PrevClose { get; set; }
            public string Ltp { get; set; }
            public string Chg { get; set; }
            public string ChgPer { get; set; }
            public string Time { get; set; }
            public string Href { get; set; }
            public string MarketState { get; set; }
            public int LinkFlag { get; set; }
            public string Message { get; set; }
            public bool Derived { get; set; }
            public long LastEpoch { get; set; }
        }
    }
    public class MarketData
    {
        public string Heading { get; set; }
        public List<MarketDataItem> Data { get; set; }
    }
    public class MarketDataItem
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }

        [JsonPropertyName("net_change")]
        public string NetChange { get; set; }

        [JsonPropertyName("percent_change")]
        public string PercentChange { get; set; }

        [JsonPropertyName("high")]
        public string High { get; set; }

        [JsonPropertyName("low")]
        public string Low { get; set; }

        [JsonPropertyName("open")]
        public string Open { get; set; }

        [JsonPropertyName("prev_close")]
        public string PrevClose { get; set; }

        [JsonPropertyName("52wkHigh")]
        public string FiftyTwoWkHigh { get; set; }

        [JsonPropertyName("52wkLow")]
        public string FiftyTwoWkLow { get; set; }

        [JsonPropertyName("weekly_per_change")]
        public string WeeklyPerChange { get; set; }

        [JsonPropertyName("monthly_per_change")]
        public string MonthlyPerChange { get; set; }

        [JsonPropertyName("3months_per_change")]
        public string ThreeMonthsPerChange { get; set; }

        [JsonPropertyName("6months_per_change")]
        public string SixMonthsPerChange { get; set; }

        [JsonPropertyName("ytd_per_change")]
        public string YtdPerChange { get; set; }

        [JsonPropertyName("yearly_per_change")]
        public string YearlyPerChange { get; set; }

        [JsonPropertyName("2years_per_change")]
        public string TwoYearsPerChange { get; set; }

        [JsonPropertyName("3years_per_change")]
        public string ThreeYearsPerChange { get; set; }

        [JsonPropertyName("5years_per_change")]
        public string FiveYearsPerChange { get; set; }

        [JsonPropertyName("technical_rating")]
        public string TechnicalRating { get; set; }

        [JsonPropertyName("last_updated")]
        public string LastUpdated { get; set; }

        [JsonPropertyName("flag_url")]
        public string FlagUrl { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("isDerived")]
        public string IsDerived { get; set; }

        [JsonPropertyName("link_flag")]
        public string LinkFlag { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class MarketDataGroup
    {
        [JsonPropertyName("heading")]
        public string Heading { get; set; }

        [JsonPropertyName("data")]
        public List<string[]> Data { get; set; }
    }

    public class MarketDataResponse
    {
        [JsonPropertyName("header")]
        public List<HeaderItem> Header { get; set; }

        [JsonPropertyName("dataList")]
        public List<MarketDataGroup> DataList { get; set; }
    }

    public class HeaderItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}

