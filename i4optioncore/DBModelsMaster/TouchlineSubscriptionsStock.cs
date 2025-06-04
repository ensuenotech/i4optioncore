using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class TouchlineSubscriptionsStock
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public int SymbolId { get; set; }

    public DateTime LastUpdatedTime { get; set; }

    public decimal Ltp { get; set; }

    public decimal TickVolume { get; set; }

    public decimal Atp { get; set; }

    public long TotalVolume { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal PreviousClose { get; set; }

    public decimal TodayOi { get; set; }

    public decimal PreviousOiclose { get; set; }

    public decimal TurnOver { get; set; }

    public DateTime? CreatedOnUtc { get; set; }

    public DateTime? UpdatedOnUtc { get; set; }

    public decimal? DeliverablePercentage { get; set; }

    public decimal? DeliverableQuantity { get; set; }

    public decimal? Ddel20 { get; set; }

    public decimal? Davol20 { get; set; }
}
