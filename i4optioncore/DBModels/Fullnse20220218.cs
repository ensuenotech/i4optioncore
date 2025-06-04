using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class Fullnse20220218
{
    public string Symbol { get; set; }

    public DateOnly? Column2 { get; set; }

    public TimeOnly? LastTradeTime { get; set; }

    public double? Open { get; set; }

    public double? High { get; set; }

    public double? Low { get; set; }

    public double? Close { get; set; }

    public double? Volume { get; set; }

    public double? OpenInterest { get; set; }
}
