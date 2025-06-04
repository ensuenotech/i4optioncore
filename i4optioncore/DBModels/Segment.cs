using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class Segment
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool? Deleted { get; set; }

    public virtual ICollection<StockSegmentMapping> StockSegmentMappings { get; set; } = new List<StockSegmentMapping>();
}
