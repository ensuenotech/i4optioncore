using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class MaxPain
{
    public int Id { get; set; }

    public string Stock { get; set; }

    public DateTime UpdatedOn { get; set; }

    public DateTime Expiry { get; set; }

    public decimal MaxPain1 { get; set; }
}
