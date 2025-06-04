using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class Volatility
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Symbol { get; set; }

    public decimal Hv10 { get; set; }

    public decimal Hv20 { get; set; }
}
