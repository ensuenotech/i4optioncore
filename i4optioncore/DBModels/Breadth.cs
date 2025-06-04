using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class Breadth
{
    public int Id { get; set; }

    public decimal Change { get; set; }

    public decimal ChangeWrtOpen { get; set; }

    public decimal ChangeWrtAtp { get; set; }

    public string Symbol { get; set; }

    public DateTime LastTradeTime { get; set; }
}
