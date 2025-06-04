using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class User
{
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

    public bool Deleted { get; set; }

    public string Mobile { get; set; }

    public decimal? WalletBalance { get; set; }

    public DateTime? NiftyPlanExpireDate { get; set; }

    public bool ShowOinumbers { get; set; }

    public string SocialProfileName { get; set; }

    public string AppToken { get; set; }

    public DateTime? AppPlanExpireDate { get; set; }

    public DateTime? BtexpiryDate { get; set; }

    public DateTime? ExcelExpiryDate { get; set; }

    public string ReferalCode { get; set; }

    public string AffiliateCode { get; set; }

    public virtual ICollection<AdminNotificationUserRead> AdminNotificationUserReads { get; set; } = new List<AdminNotificationUserRead>();

    public virtual ICollection<BasketOrder> BasketOrders { get; set; } = new List<BasketOrder>();

    public virtual ICollection<BrokeragePlanUserMapping> BrokeragePlanUserMappings { get; set; } = new List<BrokeragePlanUserMapping>();

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<DhanCredential> DhanCredentials { get; set; } = new List<DhanCredential>();

    public virtual ICollection<Otp> Otps { get; set; } = new List<Otp>();

    public virtual ICollection<TradeOrder> TradeOrders { get; set; } = new List<TradeOrder>();

    public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
}
