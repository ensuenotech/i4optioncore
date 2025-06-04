using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class BrokeragePlan
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal SellMargin { get; set; }

    public decimal BuyMargin { get; set; }

    public decimal SellBrokerage { get; set; }

    public decimal BuyBrokerage { get; set; }

    public int StrategyId { get; set; }

    public string Type { get; set; }

    public virtual Strategy Strategy { get; set; }
}
