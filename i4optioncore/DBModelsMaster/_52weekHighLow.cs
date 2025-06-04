using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class _52weekHighLow
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public DateTime Date { get; set; }

    public decimal High { get; set; }

    public DateTime HighDate { get; set; }

    public decimal Low { get; set; }

    public DateTime LowDate { get; set; }
}
