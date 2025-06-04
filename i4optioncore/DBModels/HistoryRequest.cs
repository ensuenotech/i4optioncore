using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class HistoryRequest
{
    public int Id { get; set; }

    public DateTime? RequestOn { get; set; }

    public int Interval { get; set; }

    public string Symbol { get; set; }

    public string Data { get; set; }

    public int? SymbolId { get; set; }
}
