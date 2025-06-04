using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class StockSegmentMapping
{
    public int Id { get; set; }

    public int? SegmentId { get; set; }

    public int? StockId { get; set; }
}
