using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class Ivdatum
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public decimal Ceiv { get; set; }

    public decimal Peiv { get; set; }

    public DateTime UpdatedOn { get; set; }

    public DateTime? Expiry { get; set; }

    public decimal? Ivp { get; set; }

    public decimal? Ivr { get; set; }

    public decimal? Ceivp { get; set; }

    public decimal? Ceivr { get; set; }

    public decimal? Peivp { get; set; }

    public decimal? Peivr { get; set; }
}
