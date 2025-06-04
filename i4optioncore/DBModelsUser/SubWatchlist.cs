using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class SubWatchlist
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public int SymbolId { get; set; }

    public DateTime LastUpdatedTime { get; set; }

    public decimal Ltp { get; set; }

    public long TickVolume { get; set; }

    public decimal Atp { get; set; }

    public long TotalVolume { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal PreviousClose { get; set; }

    public decimal TodayOi { get; set; }

    public decimal PreviousOiclose { get; set; }

    public long TurnOver { get; set; }

    public decimal Change { get; set; }

    public decimal ChangePercentage { get; set; }

    public decimal Oichange { get; set; }

    public decimal OiChangePercentage { get; set; }

    public int WatchListId { get; set; }

    public int DisplayOrder { get; set; }

    public virtual Watchlist WatchList { get; set; }
}
