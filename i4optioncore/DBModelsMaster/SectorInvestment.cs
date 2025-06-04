using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class SectorInvestment
{
    public int Id { get; set; }

    public int? SectorId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public DateTime UpdatedOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public decimal Value { get; set; }

    public decimal CashFlow { get; set; }
}
