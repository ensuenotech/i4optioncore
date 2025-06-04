using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class MtoStat
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Symbol { get; set; }

    public long? DeliverableQuantity { get; set; }

    public decimal? DeliverablePercentage { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}
