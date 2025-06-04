using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class PurchaseHistory
{
    public int Id { get; set; }

    public string OrderId { get; set; }

    public string PaymentId { get; set; }

    public string Status { get; set; }

    public decimal Amount { get; set; }

    public string Email { get; set; }

    public int? UserId { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public string Mobile { get; set; }

    public int? PlanId { get; set; }

    public string Remarks { get; set; }

    public int? CouponId { get; set; }
}
