using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels
{
    public partial class BuilderStrategySubComponent
    {
        public int Id { get; set; }
        public int StrategyComponentId { get; set; }
        public string TradeType { get; set; }
        public decimal Strike { get; set; }
        public string OptionType { get; set; }
        public decimal LotQty { get; set; }
        public decimal LotSize { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal? ExitPrice { get; set; }
        public DateTime Expiry { get; set; }
        public decimal? Iv { get; set; }
        public decimal? Delta { get; set; }
        public decimal? Theta { get; set; }
        public decimal? Vega { get; set; }
        public string StrikeSymbolName { get; set; }
        public decimal? Pnl { get; set; }
        public int? SymbolId { get; set; }
        public decimal? LastQuoteLtp { get; set; }

        public virtual BuilderStrategyComponent StrategyComponent { get; set; }
    }
}
