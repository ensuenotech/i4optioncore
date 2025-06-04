using System;

namespace i4optioncore.Models
{
    public class DhanModel
    {
        public class OrderDetails
        {
            public string DhanClientId { get; set; }
            public string CorrelationId { get; set; }
            public string TransactionType { get; set; }
            public string ExchangeSegment { get; set; }
            public string ProductType { get; set; }
            public string OrderType { get; set; }
            public string Validity { get; set; }
            public string TradingSymbol { get; set; }
            public string SecurityId { get; set; }
            public int Quantity { get; set; }
            public int DisclosedQuantity { get; set; }
            public decimal Price { get; set; }
            public decimal TriggerPrice { get; set; }
            public bool AfterMarketOrder { get; set; }
            public string AmoTime { get; set; }
            public decimal BoProfitValue { get; set; }
            public decimal BoStopLossValue { get; set; }
            public string DrvExpiryDate { get; set; }
            public string DrvOptionType { get; set; }
            public decimal DrvStrikePrice { get; set; }
            public string OrderStatus { get; set; }
            public string OrderId { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime UpdatedTime { get; set; }
            public DateTime ExchangeTime { get; set; }
        }
        public class PositionDetails
        {
            public string DhanClientId { get; set; }
            public string TradingSymbol { get; set; }
            public string SecurityId { get; set; }
            public string PositionType { get; set; }
            public string ExchangeSegment { get; set; }
            public string ProductType { get; set; }
            public decimal BuyAvg { get; set; }
            public int BuyQty { get; set; }
            public decimal SellAvg { get; set; }
            public int SellQty { get; set; }
            public int NetQty { get; set; }
            public decimal RealizedProfit { get; set; }
            public decimal UnrealizedProfit { get; set; }
            public decimal RbiReferenceRate { get; set; }
            public int Multiplier { get; set; }
            public int CarryForwardBuyQty { get; set; }
            public int CarryForwardSellQty { get; set; }
            public decimal CarryForwardBuyValue { get; set; }
            public decimal CarryForwardSellValue { get; set; }
            public int DayBuyQty { get; set; }
            public int DaySellQty { get; set; }
            public decimal DayBuyValue { get; set; }
            public decimal DaySellValue { get; set; }
            public string DrvExpiryDate { get; set; }
            public string DrvOptionType { get; set; }
            public decimal DrvStrikePrice { get; set; }
            public bool CrossCurrency { get; set; }
        }
        public class DhanOrderRequest
        {
            public string dhanClientId { get; set; } = "1000724947";
            public string correlationId { get; set; } = "NA";
            public string transactionType { get; set; }
            public string exchangeSegment { get; set; }
            public string productType { get; set; }
            public string orderType { get; set; }
            public string validity { get; set; } = "DAY";
            public string tradingSymbol { get; set; }
            public string securityId { get; set; }
            public int quantity { get; set; }
            public int disclosedQuantity { get; set; } = 0;
            public decimal price { get; set; }
            public decimal triggerPrice { get; set; }
            public bool afterMarketOrder { get; set; } = false;
            public string amoTime { get; set; } = "OPEN";
            public decimal boProfitValue { get; set; }
            public decimal boStopLossValue { get; set; }
            public string drvExpiryDate { get; set; }
            public string drvOptionType { get; set; }
            public decimal drvStrikePrice { get; set; }
            public string AuthToken { get; set; }
        }
        public class DhanSymbolDetails
        {
            public string SEM_EXM_EXCH_ID { get; set; }
            public string SEM_SEGMENT { get; set; }
            public string SEM_SMST_SECURITY_ID { get; set; }
            public string SEM_INSTRUMEMT_NAME { get; set; }
            public string SEM_EXPIRY_CODE { get; set; }
            public string SEM_TRADING_SYMBOL { get; set; }
            public string SEM_LOT_UNITS { get; set; }
            public string SEM_CUSTOM_SYMBOL { get; set; }
        }

    }
}
