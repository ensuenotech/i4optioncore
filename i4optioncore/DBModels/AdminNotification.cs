using System;
using System.Collections.Generic;

#nullable disable

namespace i4optioncore.DBModels
{
    public partial class AdminNotification
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Notification { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Deleted { get; set; }
    }
}
