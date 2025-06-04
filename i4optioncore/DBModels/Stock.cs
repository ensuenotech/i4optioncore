using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class Stock
{
    public int Id { get; set; }

    public string Type { get; set; }

    public string DisplayName { get; set; }

    public int Depth { get; set; }

    public decimal Change { get; set; }

    public int LotSize { get; set; }

    public int? CalendarId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public decimal? MaxPain { get; set; }

    public DateTime? MaxPainLastUpdatedUtc { get; set; }

    public bool Active { get; set; }

    public bool Deleted { get; set; }

    public string Name { get; set; }

    public decimal? FreeFloat { get; set; }

    public virtual Calendar Calendar { get; set; }

    public virtual ICollection<StockSegmentMapping> StockSegmentMappings { get; set; } = new List<StockSegmentMapping>();
}
