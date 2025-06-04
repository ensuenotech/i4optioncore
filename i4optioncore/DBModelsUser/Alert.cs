using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Alert
{
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public string SymbolType { get; set; }

    public string Symbol { get; set; }

    public string AlertFor { get; set; }

    public string Condition { get; set; }

    public decimal Value { get; set; }

    public string Status { get; set; }

    public DateTime? TriggeredOn { get; set; }

    public bool Deleted { get; set; }

    public int UserId { get; set; }
}
