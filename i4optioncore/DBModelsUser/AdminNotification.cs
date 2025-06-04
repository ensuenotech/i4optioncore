using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class AdminNotification
{
    public int Id { get; set; }

    public string Subject { get; set; }

    public string Notification { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool Deleted { get; set; }
}
