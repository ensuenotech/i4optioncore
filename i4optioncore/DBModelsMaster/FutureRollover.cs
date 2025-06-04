using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class FutureRollover
{
    public int Id { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public string Symbol { get; set; }

    public decimal Rollover { get; set; }
}
