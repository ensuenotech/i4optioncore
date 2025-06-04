using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class ActiveOi
{
    public int Id { get; set; }

    public DateTime? LastTradeTime { get; set; }

    public long? Ceoi { get; set; }

    public long? Peoi { get; set; }

    public double? FutLtp { get; set; }

    public double? FutVwap { get; set; }

    public DateTime? InsertedAt { get; set; }

    public string Symbol { get; set; }
}
