using System;

namespace i4optioncore.Models
{
    public class NotificationModel
    {
        public class NotificationRequest
        {
            public int? Id { get; set; }

            public string Campaign { get; set; }

            public string Condition { get; set; }

            public string Template { get; set; }

            public string Schedule { get; set; }

            public string Days { get; set; }

            public bool Status { get; set; }

            public DateTime Date { get; set; }

        }
    }
}
