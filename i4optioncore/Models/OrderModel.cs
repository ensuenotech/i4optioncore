using System;

namespace i4optioncore.Models
{
    public class OrderModel
    {
        public class PurchaseHistoryDetails
        {
            public int UserId { get; set; }
            public string OrderId { get; set; }
            public DateTime Date { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }
            public string State { get; set; }
            public string AffiliateCode { get; set; }
            public int? AffiliateId { get; set; }
            public string couponUsed { get; set; }
        }
        public class PurchaseHistoryRequest
        {
            public int? StateId { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
            public DateTime? Date { get; set; }
        }
        public class CapturePaymentDetails
        {
            public string PaymentId { get; set; }
            public string OrderId{ get; set; }
        }
        public class OrderRequest
        {
            public string P{ get; set; }
            public int UserId { get; set; }
            public string T { get; set; }
            public int? CouponId { get; set; }
        }
        public class CouponRequest
        {
            public int? CouponId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string DiscountType { get; set; }
            public decimal  DiscountValue { get; set; }
            public bool Active{ get; set; }
        }
    }
}
