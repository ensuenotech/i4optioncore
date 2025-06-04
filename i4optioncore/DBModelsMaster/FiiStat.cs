using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class FiiStat
{
    public int Id { get; set; }

    public string VerticleHeading { get; set; }

    public decimal BuyNoOfContracts { get; set; }

    public decimal BuyAmtInCrores { get; set; }

    public decimal SellNoOfContracts { get; set; }

    public decimal SellAmtInCrores { get; set; }

    public decimal OiEodnoOfContracts { get; set; }

    public decimal OiEodamtInCrores { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public DateTime Date { get; set; }
}
