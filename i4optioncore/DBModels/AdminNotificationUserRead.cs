using System;
using System.Collections.Generic;

#nullable disable

namespace i4optioncore.DBModels
{
    public partial class AdminNotificationUserRead
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
