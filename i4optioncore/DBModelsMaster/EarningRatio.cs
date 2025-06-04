using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class EarningRatio
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public DateTime Date { get; set; }

    public decimal Pe { get; set; }

    public decimal Dy { get; set; }

    public decimal Pbr { get; set; }

    public string Series { get; set; }

    public decimal? High { get; set; }

    public DateTime? HighDate { get; set; }

    public decimal? Low { get; set; }

    public DateTime? LowDate { get; set; }

    public decimal? OpenValue { get; set; }

    public decimal? ClosingValue { get; set; }

    public decimal? PointsChange { get; set; }

    public decimal? ChangePercentage { get; set; }

    public long? Volume { get; set; }

    public decimal? Turnover { get; set; }

    public decimal? Pb { get; set; }

    public decimal? DivYield { get; set; }
}
