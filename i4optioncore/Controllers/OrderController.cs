using i4optioncore.Models;
using i4optioncore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace i4optioncore.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class OrderController : Controller
    {
        IOrderBL orderBL;

        public OrderController(IOrderBL _orderBL)
        {
            orderBL = _orderBL;
        }
        //[Route("Payment"), HttpPost]
        //public async Task<IActionResult> Payment([FromBody] OrderBL.RazorpayResponse data)
        //{
        //    try
        //    {
        //        var _result = await orderBL.Payment(data);
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}

        //[Route("PaymentCheck/{paymentId}"), HttpGet]
        //public IActionResult PaymentCheck(string paymentId)
        //{
        //    try
        //    {
        //        var _result = orderBL.ValidatePayment(paymentId);
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel.OrderRequest orderRequest)
        {
            var orderId = await orderBL.CreateOrder(orderRequest);
            return Ok(new { orderId });
        }

        [HttpPost("capture-payment")]
        public async Task<IActionResult> CapturePayment([FromBody] OrderModel.CapturePaymentDetails data)
        {
            await orderBL.CapturePayment(data);
            return Ok("SUCCESS");
        }
        [HttpPost("coupon")]
        public async Task<IActionResult> SaveCoupon([FromBody] OrderModel.CouponRequest data)
        {
            await orderBL.CreateCoupon(data);
            return Ok("SUCCESS");
        }
        [HttpPost("coupon-get")]
        public async Task<IActionResult> GetCoupon()
        {
            return Ok(await orderBL.GetCoupons());
        }
        [HttpPost("coupon-d")]
        public async Task<IActionResult> DeleteCoupon([FromBody] int id)
        {
            await orderBL.DeleteCoupon(id);
            return Ok("SUCCESS");
        } 
        
        [HttpPost("coupon-details")]
        public IActionResult GetCouponDetails([FromBody] string couponCode)
        {
            
            return Ok(orderBL.GetCouponDetails(couponCode));
        }


        [Route("Filter"), HttpPost]
        public async Task<IActionResult> FilterOrder([FromBody] OrderModel.PurchaseHistoryRequest data)
        {
            try
            {
                var _result = await orderBL.GetOrders(data);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
