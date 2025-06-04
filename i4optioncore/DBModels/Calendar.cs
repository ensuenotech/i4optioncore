using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class Calendar
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool Active { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public virtual ICollection<CalendarDate> CalendarDates { get; set; } = new List<CalendarDate>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
