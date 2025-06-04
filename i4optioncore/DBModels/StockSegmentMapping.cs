using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class StockSegmentMapping
{
    public int Id { get; set; }

    public int SegmentId { get; set; }

    public int StockId { get; set; }

    public virtual Segment Segment { get; set; }

    public virtual Stock Stock { get; set; }
}
