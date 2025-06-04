using i4optioncore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static i4optioncore.Repositories.OrderBL;

namespace i4optioncore.Repositories
{
    public interface IOrderBL
    {
        Task CapturePayment(OrderModel.CapturePaymentDetails request);
        Task CreateCoupon(OrderModel.CouponRequest request);
        Task<string> CreateOrder(OrderModel.OrderRequest request);
        Task DeleteCoupon(int CouponId);
        object GetCouponDetails(string CouponCode);
        Task<object> GetCoupons();
        Task<List<OrderModel.PurchaseHistoryDetails>> GetOrders(OrderModel.PurchaseHistoryRequest request);
        Task<bool> Payment(RazorpayResponse data);
        string ValidatePayment(string PaymentId);
    }
}
