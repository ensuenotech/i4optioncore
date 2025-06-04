using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Position
{
    public int Id { get; set; }

    public decimal? SellAvg { get; set; }

    public int? SellQuantity { get; set; }

    public decimal? BuyAvg { get; set; }

    public int? BuyQuantity { get; set; }

    public decimal Ltp { get; set; }

    public string OrderType { get; set; }

    public decimal PandL { get; set; }

    public string Symbol { get; set; }

    public int UserId { get; set; }

    public DateTime UpdatedOn { get; set; }

    public int? Quantity { get; set; }

    public DateTime? Expiry { get; set; }

    public string Strategy { get; set; }

    public int? Strike { get; set; }

    public string Exchange { get; set; }

    public string InstrumentType { get; set; }
}
