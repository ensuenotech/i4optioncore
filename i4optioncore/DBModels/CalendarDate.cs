using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class CalendarDate
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool Active { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public int CalendarId { get; set; }

    public virtual Calendar Calendar { get; set; }
}
