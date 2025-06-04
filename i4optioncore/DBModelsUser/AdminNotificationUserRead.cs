using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class AdminNotificationUserRead
{
    public int Id { get; set; }

    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; }
}
