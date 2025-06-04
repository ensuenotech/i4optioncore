using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class Tl2007
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public DateTime LastTradeTime { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal Close { get; set; }

    public long Volume { get; set; }

    public long OpenInterest { get; set; }

    public decimal Atp { get; set; }

    public long TotalVolume { get; set; }
}
