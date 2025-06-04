using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class History122023
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public DateTime LastTradeTime { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal Close { get; set; }

    public int Volume { get; set; }

    public int OpenInterest { get; set; }

    public decimal Atp { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public long? TotalVolume { get; set; }
}
