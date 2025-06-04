using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class EarningRatio
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public DateTime ErDate { get; set; }

    public int? Index { get; set; }

    public int? Open { get; set; }

    public int? High { get; set; }

    public int? Low { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }
}
