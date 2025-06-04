using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Notification
{
    public int Id { get; set; }

    public string Campaign { get; set; }

    public string Condition { get; set; }

    public string Template { get; set; }

    public string Schedule { get; set; }

    public string Days { get; set; }

    public bool Status { get; set; }

    public DateTime Date { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
