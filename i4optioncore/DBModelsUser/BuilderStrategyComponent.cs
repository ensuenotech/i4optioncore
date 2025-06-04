using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class BuilderStrategyComponent
{
    public int Id { get; set; }

    public int StrategyId { get; set; }

    public DateTime TradeTime { get; set; }

    public string SymbolName { get; set; }

    public string FutureSymbolName { get; set; }

    public decimal SavedSpotPrice { get; set; }

    public decimal SavedFuturePrice { get; set; }

    public decimal? LastQuotePrice { get; set; }

    public decimal? LastQuoteFutPrice { get; set; }

    public string SpotSymbolName { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool Deleted { get; set; }

    public string Note { get; set; }

    public virtual ICollection<BuilderStrategySubComponent> BuilderStrategySubComponents { get; set; } = new List<BuilderStrategySubComponent>();

    public virtual BuilderStrategy Strategy { get; set; }
}
