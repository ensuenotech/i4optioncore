using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class Cashflow
{
    public int Id { get; set; }

    public string Category { get; set; }

    public decimal? BuyValueFii { get; set; }

    public decimal? SellValueFii { get; set; }

    public decimal? NetValueFii { get; set; }

    public decimal? BuyValueDii { get; set; }

    public decimal? SellValueDii { get; set; }

    public decimal? NetValueDii { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public string Year { get; set; }

    public DateTime? Date { get; set; }

    public DateOnly? Month { get; set; }
}
