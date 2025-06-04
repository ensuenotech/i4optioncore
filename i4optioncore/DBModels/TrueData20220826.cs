using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class TrueData20220826
{
    public string Symbol { get; set; }

    public DateOnly? Ltd { get; set; }

    public TimeOnly? Ltt { get; set; }

    public double? Open { get; set; }

    public double? High { get; set; }

    public double? Low { get; set; }

    public double? Close { get; set; }

    public long? Volume { get; set; }

    public long? OpenInterest { get; set; }
}
