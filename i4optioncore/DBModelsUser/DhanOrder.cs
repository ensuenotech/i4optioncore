using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class DhanOrder
{
    public int Id { get; set; }

    public string TransactionType { get; set; }

    public string ExchangeSegment { get; set; }

    public string ProductType { get; set; }

    public string OrderType { get; set; }

    public string TradingSymbol { get; set; }

    public string SecurityId { get; set; }

    public int? Quantity { get; set; }

    public int? DisclosedQuantity { get; set; }

    public decimal? Price { get; set; }

    public decimal? TriggerPrice { get; set; }

    public decimal? BoprofitValue { get; set; }

    public decimal? BostopLossValue { get; set; }

    public string DrvexpiryDate { get; set; }

    public string DrvoptionType { get; set; }

    public decimal? DrvstrikePrice { get; set; }

    public int UserId { get; set; }

    public string Status { get; set; }

    public string CorrelationId { get; set; }
}
