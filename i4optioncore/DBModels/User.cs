using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels
{
    public partial class User
    {
        public User()
        {
            CustomerAddresses = new HashSet<CustomerAddress>();
            Otps = new HashSet<Otp>();
        }

        public int Id { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public DateTime? PlanExpireDate { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string Bio { get; set; }
        public bool Deleted { get; set; }
        public string Mobile { get; set; }

        public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }
        public virtual ICollection<Otp> Otps { get; set; }
    }
}
