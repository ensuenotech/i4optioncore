using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class SegmentTouchline
{
    public int Id { get; set; }

    public string Segment { get; set; }

    public int SegmentId { get; set; }

    public int Open { get; set; }

    public decimal Ltp { get; set; }

    public decimal Low { get; set; }

    public long TickVolume { get; set; }

    public int Atp { get; set; }

    public long TurnOver { get; set; }

    public int High { get; set; }

    public int TodayOi { get; set; }

    public int PreviousClose { get; set; }

    public long PreviousOiclose { get; set; }

    public decimal Change { get; set; }

    public decimal ChangePercentage { get; set; }

    public decimal Oichange { get; set; }

    public decimal OichangePercentage { get; set; }

    public long? DeliverableQuantity { get; set; }

    public long? TotalVolume { get; set; }

    public decimal? DeliverablePercentage { get; set; }

    public DateTime LastUpdatedTime { get; set; }
}
