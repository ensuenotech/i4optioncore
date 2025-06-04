using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class Rtdatum
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public int? SymbolId { get; set; }

    public string Data { get; set; }

    public DateTime? UpdatedOnUtc { get; set; }
}
