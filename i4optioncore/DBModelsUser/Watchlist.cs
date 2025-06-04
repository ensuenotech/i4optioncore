using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Watchlist
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<SubWatchlist> SubWatchlists { get; set; } = new List<SubWatchlist>();

    public virtual User User { get; set; }
}
