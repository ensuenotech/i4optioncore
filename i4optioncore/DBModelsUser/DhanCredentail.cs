using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser
{
    public partial class DhanCredentials
    {
        public int Id { get; set; }
        public string DhanUcc { get; set; }
        public string DhanClientId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }

        public virtual User User { get; set; }
    }
}
