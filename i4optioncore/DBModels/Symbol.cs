using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class Symbol
{
    public int Id { get; set; }

    public int SymbolId { get; set; }

    public string Symbol1 { get; set; }

    public string Series { get; set; }

    public string Isin { get; set; }

    public string Exchange { get; set; }

    public int LotSize { get; set; }

    public decimal Strike { get; set; }

    public DateTime? Expiry { get; set; }

    public string Alias { get; set; }

    public string Symbol15 { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public string Segment { get; set; }
}
