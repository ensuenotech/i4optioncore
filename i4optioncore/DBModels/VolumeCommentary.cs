using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class VolumeCommentary
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
}
