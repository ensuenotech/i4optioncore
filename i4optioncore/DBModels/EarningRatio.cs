using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class EarningRatio
{
    public int Id { get; set; }

    public string IndexName { get; set; }

    public DateTime IndexDate { get; set; }

    public decimal? OpenIndexValue { get; set; }

    public decimal? HighIndexValue { get; set; }

    public decimal? LowIndexValue { get; set; }

    public decimal? ClosingIndexValue { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public decimal? ClosingValue { get; set; }

    public decimal? PointsChange { get; set; }

    public decimal? ChangePercentage { get; set; }

    public long? Volume { get; set; }

    public decimal? Turnover { get; set; }

    public decimal? Pe { get; set; }

    public decimal? Pb { get; set; }

    public decimal? DivYield { get; set; }
}
