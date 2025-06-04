using Azure.Core;
using DocumentFormat.OpenXml.Vml.Office;

using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using i4optioncore.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Razorpay.Api;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TwelveDataSharp.Library.ResponseModels;
using static i4optioncore.Models.CommonModel;

namespace i4optioncore.Repositories
{
    public class OrderBL : IOrderBL
    {
        I4optionUserDbContext db;
        private readonly IWebHostEnvironment env;
        //private string key = "rzp_test_kz2zj299UxrCD6";
        //private string secret = "mlutpZkMoV0JIDX8nkyVTFw9";
        string key = "rzp_live_nPmkRtYTIg50Ci";
        string secret = "etl6DKZ3J38uE6bTBb8S93TP";
        readonly ICommonBL commonBL;
        public OrderBL(I4optionUserDbContext _db, IWebHostEnvironment _env, ICommonBL _commonBL)
        {
            db = _db;
            env = _env;
            commonBL = _commonBL;
        }
        public string ValidatePayment(string PaymentId)
        {
            RazorpayClient client = new(key, secret);
            var payment = client.Payment.Fetch(PaymentId);
            var details = JsonConvert.DeserializeObject<RazorpayPaymentDetails>(payment.Attributes.ToString());
            return details.status;

        }
        public async Task<bool> Payment(RazorpayResponse data)
        {
            if (data.Event == "payment.captured")
            {
                var entity = data.payload.payment.entity;
                if (entity.status != "captured") return false;

                if (ValidatePayment(entity.id).ToLower() != "captured") return false;

                decimal amount = decimal.Parse((entity.amount / 100).ToString());
                var plan = db.Plans.FirstOrDefault(x => x.Amount == amount);
                if (plan != null)
                {
                    var remarks = string.Empty;
                    var user = new User();
                    var userid = "";
                    try
                    {
                        userid = entity.notes.userid;
                    }
                    catch
                    {
                        userid = null;
                    }
                    if (!string.IsNullOrEmpty(userid))
                    {
                        user = db.Users.FirstOrDefault(x => x.Id == int.Parse(userid));
                    }
                    else
                    {
                        user = db.Users.FirstOrDefault(x => x.Email.ToLower() == entity.email.ToLower());
                    }

                    if (user != null)
                    {
                        DateTime? expiryDate = null;
                        if (plan.Domain == "niftyaction")
                        {
                            user.NiftyPlanExpireDate = user.NiftyPlanExpireDate > DateTime.Now ? user.NiftyPlanExpireDate.Value.AddDays(plan.Days) : DateTime.Now.AddDays(plan.Days);
                            expiryDate = user.NiftyPlanExpireDate;
                        }
                        else if (plan.Domain == "backtesting")
                        {
                            user.BtexpiryDate = user.BtexpiryDate > DateTime.Now ? user.BtexpiryDate.Value.AddDays(plan.Days) : DateTime.Now.AddDays(plan.Days);
                            expiryDate = user.BtexpiryDate;
                        }
                        else if (plan.Domain == "excel")
                        {
                            user.ExcelExpiryDate = user.ExcelExpiryDate > DateTime.Now ? user.ExcelExpiryDate.Value.AddDays(plan.Days) : DateTime.Now.AddDays(plan.Days);
                            expiryDate = user.ExcelExpiryDate;

                        }
                        else
                        {
                            user.PlanExpireDate = user.PlanExpireDate > DateTime.Now ? user.PlanExpireDate.Value.AddDays(plan.Days) : DateTime.Now.AddDays(plan.Days);
                            expiryDate = user.PlanExpireDate;
                        }
                        db.UserTokens.RemoveRange(db.UserTokens.Where(x => x.Id == user.Id));

                        await db.SaveChangesAsync();

                        string contentRootPath = env.ContentRootPath;

                        string body = string.Empty;
                        //                         using (StreamReader reader = new StreamReader(contentRootPath + "/emails/order-email.html", Encoding.Default)) // Path to your 
                        //                         {
                        //                            body = reader.ReadToEnd();
                        //                            body = body.Replace("[[--NAME--]]", string.Format("{0} {1}", user.FirstName, user.LastName));
                        //                            body = body.Replace("[[--ORDERID--]]", string.Format("{0}", entity.order_id));
                        //                         //    body = body.Replace("[[--CONTENT--]]", string.Format("<p>Dear {0} {1}</span></p><p>Your account plan has been renewed and the plan expiry date is {2}</strong></p><p>You can check the details online&nbsp;under the profile section:&nbsp;<a href='https://dashboard.i4option.com/dashboard/profile' rel='noopener noreferrer' target='_blank' style='color: rgb(17, 85, 204);'>https://dashboard.i4option.com/dashboard/profile</a><p><span style=';'>for any help &amp; query Whatsapp 9330000029&nbsp;or write us on this mail.&nbsp;</span></p><p><br></p><p>Thanks!</span></p>", user.FirstName, user.LastName, user.PlanExpireDate.Value));
                        //                         }

                        // body = @$"Dear {user.FirstName} {user.LastName},<br/>
                        //                         Your account plan has been renewed and the plan expiry date is {expiryDate}<br/>
                        //                         You can check the details online under the profile section: https://dashboard.i4option.com/dashboard/profile<br/>
                        //                         <br/>
                        //                         For customer care support and help:-<br/>
                        //                         Whatsapp:- +91 9330000029<br/>
                        //                         Mail:- support@ifil.co.in<br/>
                        //                         <br/>
                        //                         <br/>
                        //                         <br/>
                        //                         Do not reply to this mail. <br/>
                        //                         <br/>
                        //                         Thanks and regards,<br/>
                        //                         i4option <br/>
                        //                         ";
                        //                                                var mail = new SendMailDetails
                        //                                                {
                        //                                                    To = user.Email,
                        //                                                    Subject = string.Format("Plan Renewed"),
                        //                                                    Body = body,
                        //                                                    BCC = new List<string> { "lvkeshb@gmail.com" }
                        //                                                };

                        //                                                await commonBL.SendViaSendGridAsync(mail.Subject, mail.To, "i4option User", mail.Body, mail.BCC);
                        if (expiryDate.HasValue)
                        {
                            await commonBL.SendMsg91Email("Planupdated_i4", $"{user.FirstName} {user.LastName}", $"{expiryDate.Value:dd-MM-yyyy}", user.Email, $"{user.FirstName} {user.LastName}");
                            await commonBL.SendMsg91Email("Planupdated_i4", $"{user.FirstName} {user.LastName}", $"{expiryDate.Value:dd-MM-yyyy}", "lvkeshb@gmail.com", $"{user.FirstName} {user.LastName}");
                            await commonBL.SendWhatsapp(user.Mobile, "payment_i4", $"{user.FirstName} {user.LastName}", $"{expiryDate.Value:dd-MM-yyyy}", amount.ToString(), null, null);
                        }

                    }
                    else
                    {
                        remarks = "user/plan data not found for same amount";

                        string contentRootPath = env.ContentRootPath;

                        //                        string body = string.Empty;
                        //                        body = string.Format(@"Dear User,<br/>
                        //We confirm receipt of your payment of {0} for a subscription for i4option.com, but we could not find your registered mail<br/>
                        //Have you registered with another mail id? please let us know<br/>
                        //If you still not registered please complete registration, it is an easy and one-time process.to register click here: https://www.i4option.com/#/register <br/>
                        // <br/>
                        //For customer care support and help:-<br/>
                        //Whatsapp:- +91 9330000029<br/>
                        //Mail:- support@ifil.co.in<br/>
                        //<br/>
                        //<br/>
                        //Do not reply to this mail.<br/>
                        //<br/>
                        //Thanks and regards,<br/>
                        //i4option <br/>
                        //", amount);

                        //using (StreamReader reader = new StreamReader(contentRootPath + "/emails/order-email.html", Encoding.Default)) // Path to your 
                        //{
                        //    body = reader.ReadToEnd();
                        //    body = body.Replace("[[--NAME--]]", string.Format("{0} {1}", firstname, lastname));
                        //    body = body.Replace("[[--ORDERID--]]", string.Format("{0}", entity.order_id));
                        //    body = body.Replace("[[--CONTENT--]]", string.Format(@"<p>Dear {0} {1},</span></p><p>We confirm receipt of your payment of ₹{2} for a subscription for i4option.com, but we could not find your registered mail</strong></p><p>Have you registered with another mail id? please let us know </p><p>If you still not registered please complete registration, it is an easy and one-time process.to register click here:&nbsp;<a href='https://www.i4option.com/#/register' rel='noopener noreferrer' target='_blank' style='color: rgb(17, 85, 204);'>https://www.i4option.com/#/register</a></p><p>or any help &amp; query Whatsapp 9330000029&nbsp;or write us on this mail.&nbsp;</span></p><p><br></p><p>Thanks!</span></p>", firstname, lastname, amount));
                        //}
                        //var mail = new CommonModel.SendMailDetails
                        //{
                        //    To = entity.email,
                        //    Subject = string.Format("Registered mail id not found"),
                        //    Body = body
                        //};

                        //await commonBL.SendViaSendGridAsync(mail.Subject, mail.To, "i4option User", mail.Body, null);
                        //await commonBL.SendMsg91Email("Planupdated_i4", $"{user.FirstName} {user.LastName}", "", user.Email, $"{user.FirstName} {user.LastName}");
                    }

                    if (!db.PurchaseHistories.Any(x => x.PaymentId == entity.id && x.OrderId == entity.order_id))
                    {

                        db.PurchaseHistories.Add(new PurchaseHistory
                        {
                            Amount = amount,
                            CreatedOnUtc = DateTime.Now,
                            Email = entity.email,
                            Mobile = entity.contact,
                            OrderId = entity.order_id,
                            PaymentId = entity.id,
                            UserId = user?.Id,
                            PlanId = plan.Id,
                            UpdatedOnUtc = DateTime.Now,
                            Status = entity.status,
                            Remarks = remarks,
                        });
                        await db.SaveChangesAsync();
                    }

                    return true;
                }
                return false;
            }


            return false;
        }
        public async Task<List<OrderModel.PurchaseHistoryDetails>> GetOrders(OrderModel.PurchaseHistoryRequest request)
        {

            if (request.Year.HasValue)
            {
                if (request.Month.HasValue)
                {
                    return await (from ph in db.PurchaseHistories
                                  join user in db.Users on ph.UserId equals user.Id
                                  where ph.CreatedOnUtc.Date.Month == request.Month && ph.CreatedOnUtc.Date.Year == request.Year
                                  && (!request.StateId.HasValue || user.CustomerAddresses.Any(x => x.Address.StateId == request.StateId))
                                  && !string.IsNullOrEmpty(ph.PaymentId)
                                  select new OrderModel.PurchaseHistoryDetails
                                  {
                                      Date = ph.CreatedOnUtc,
                                      Amount = ph.Amount,
                                      Email = user.Email,
                                      Mobile = user.Mobile,
                                      OrderId = ph.OrderId,
                                      Status = ph.Status,
                                      Remarks = ph.Remarks,
                                      UserId = user.Id,
                                      State = user.CustomerAddresses.FirstOrDefault().Address.State.Name,
                                      couponUsed = db.Coupons.FirstOrDefault(x => x.Id == ph.CouponId).Name,
                                      AffiliateCode = user.AffiliateCode,
                                      AffiliateId = db.Users.Where(x => x.ReferalCode == user.AffiliateCode).FirstOrDefault().Id,
                                  }).ToListAsync();
                }
                else
                {
                    return await (from ph in db.PurchaseHistories
                                  join user in db.Users on ph.UserId equals user.Id
                                  where ph.CreatedOnUtc.Date.Year == request.Year
                                  && (!request.StateId.HasValue || user.CustomerAddresses.Any(x => x.Address.StateId == request.StateId))
                                  && !string.IsNullOrEmpty(ph.PaymentId)
                                  select new OrderModel.PurchaseHistoryDetails
                                  {
                                      Date = ph.CreatedOnUtc,
                                      Amount = ph.Amount,
                                      Email = user.Email,
                                      Mobile = user.Mobile,
                                      OrderId = ph.OrderId,
                                      Status = ph.Status,
                                      Remarks = ph.Remarks,
                                      UserId = user.Id,
                                      State = user.CustomerAddresses.FirstOrDefault().Address.State.Name,
                                      couponUsed = db.Coupons.FirstOrDefault(x => x.Id == ph.CouponId).Name,
                                      AffiliateCode = user.AffiliateCode,
                                      AffiliateId = db.Users.Where(x => x.ReferalCode == user.AffiliateCode).FirstOrDefault().Id,
                                  }).ToListAsync();
                }
            }
            else if (request.Month.HasValue && !request.Year.HasValue)
            {
                return await (from ph in db.PurchaseHistories
                              join user in db.Users on ph.UserId equals user.Id
                              where ph.CreatedOnUtc.Date.Month == request.Month
                              && (!request.StateId.HasValue || user.CustomerAddresses.Any(x => x.Address.StateId == request.StateId))
                              && !string.IsNullOrEmpty(ph.PaymentId)
                              select new OrderModel.PurchaseHistoryDetails
                              {
                                  Date = ph.CreatedOnUtc,
                                  Amount = ph.Amount,
                                  Email = user.Email,
                                  Mobile = user.Mobile,
                                  OrderId = ph.OrderId,
                                  Status = ph.Status,
                                  Remarks = ph.Remarks,
                                  UserId = user.Id,
                                  State = user.CustomerAddresses.FirstOrDefault().Address.State.Name,
                                  couponUsed = db.Coupons.FirstOrDefault(x => x.Id == ph.CouponId).Name,
                                  AffiliateCode = user.AffiliateCode,
                                  AffiliateId = db.Users.Where(x => x.ReferalCode == user.AffiliateCode).FirstOrDefault().Id,
                              }).ToListAsync();
            }
            else if (request.Date.HasValue)
            {
                return await (from ph in db.PurchaseHistories
                              join user in db.Users on ph.UserId equals user.Id
                              where ph.CreatedOnUtc.Date == request.Date.Value.Date
                              && (!request.StateId.HasValue || user.CustomerAddresses.Any(x => x.Address.StateId == request.StateId))
                              && !string.IsNullOrEmpty(ph.PaymentId)
                              select new OrderModel.PurchaseHistoryDetails
                              {
                                  Date = ph.CreatedOnUtc,
                                  Amount = ph.Amount,
                                  Email = user.Email,
                                  Mobile = user.Mobile,
                                  OrderId = ph.OrderId,
                                  Status = ph.Status,
                                  Remarks = ph.Remarks,
                                  UserId = user.Id,
                                  State = user.CustomerAddresses.FirstOrDefault().Address.State.Name,
                                  couponUsed = db.Coupons.FirstOrDefault(x => x.Id == ph.CouponId).Name,
                                  AffiliateCode = user.AffiliateCode,
                                  AffiliateId = db.Users.Where(x => x.ReferalCode == user.AffiliateCode).FirstOrDefault().Id,
                              }).ToListAsync();
            }

            if (request.StateId.HasValue && !(request.Year.HasValue && request.Month.HasValue && request.Date.HasValue))
            {
                return await (from ph in db.PurchaseHistories
                              join user in db.Users on ph.UserId equals user.Id
                              where user.CustomerAddresses.Any(x => x.Address.StateId == request.StateId)
                              && !string.IsNullOrEmpty(ph.PaymentId)
                              select new OrderModel.PurchaseHistoryDetails
                              {
                                  Date = ph.CreatedOnUtc,
                                  Amount = ph.Amount,
                                  Email = user.Email,
                                  Mobile = user.Mobile,
                                  OrderId = ph.OrderId,
                                  Status = ph.Status,
                                  Remarks = ph.Remarks,
                                  UserId = user.Id,
                                  State = user.CustomerAddresses.FirstOrDefault().Address.State.Name,
                                  couponUsed = db.Coupons.FirstOrDefault(x => x.Id == ph.CouponId).Name,
                                  AffiliateCode = user.AffiliateCode,
                                  AffiliateId = db.Users.Where(x => x.ReferalCode == user.AffiliateCode).FirstOrDefault().Id,
                              }
                ).ToListAsync();
            }
            throw new NotFoundException("INVALID");
        }

        public async Task<string> CreateOrder(OrderModel.OrderRequest request)
        {
            var planId = long.Parse(request.P) / long.Parse(request.T);
            var plan = db.Plans.Where(x => x.Id == planId).FirstOrDefault();
            var user = db.Users.Where(x => x.Id == request.UserId).FirstOrDefault();
            var payableAmount = plan.Amount;
            if (request.CouponId.HasValue)
            {
                var coupon = db.Coupons.Where(x => x.Id == request.CouponId && x.Active).FirstOrDefault();
                if (coupon != null)
                {
                    var discount = coupon.DiscountValue;
                    if (coupon.DiscountType == "percentage")
                    {
                        discount = plan.Amount * coupon.DiscountValue / 100;
                    }

                    payableAmount = plan.Amount - discount;

                }
            }
            payableAmount = Math.Round(payableAmount * 1.18m, 2);
            // Initialize Razorpay client with your API key and secret key
            RazorpayClient razorpayClient = new RazorpayClient(key, secret);

            // Create order
            Dictionary<string, object> options = new()
            {
                { "amount", payableAmount*100 }, // amount in paise
                { "currency", "INR" },
                { "receipt", $"PAYMENT_{user.Id}_{plan.Id}" }
            };
            Order order = razorpayClient.Order.Create(options);


            db.PurchaseHistories.Add(new PurchaseHistory
            {
                Amount = payableAmount,
                CreatedOnUtc = DateTime.Now,
                Email = user.Email,
                Mobile = user.Mobile,
                OrderId = order["id"],
                PaymentId = null,
                UserId = user?.Id,
                PlanId = plan.Id,
                UpdatedOnUtc = DateTime.Now,
                Status = "pending",
                CouponId = request.CouponId
            });
            await db.SaveChangesAsync();

            return order["id"];

        }
        public async Task CapturePayment(OrderModel.CapturePaymentDetails request)
        {

            var order = db.PurchaseHistories.Where(x => x.OrderId == request.OrderId).FirstOrDefault();


            var planId = order.PlanId;
            var plan = db.Plans.Where(x => x.Id == planId).FirstOrDefault();
            var user = db.Users.Where(x => x.Id == order.UserId).FirstOrDefault();


            if (user != null)
            {
                if (plan.Domain == "niftyaction")
                    user.NiftyPlanExpireDate = user.NiftyPlanExpireDate > DateTime.Now ? user.NiftyPlanExpireDate.Value.AddDays(plan.Days) : DateTime.Now.AddDays(plan.Days);
                else if (plan.Domain == "backtesting")
                    user.BtexpiryDate = user.BtexpiryDate > DateTime.Now ? user.BtexpiryDate.Value.AddDays(plan.Days) : DateTime.Now.AddDays(plan.Days);
                else if (plan.Domain == "excel")
                    user.ExcelExpiryDate = user.ExcelExpiryDate > DateTime.Now ? user.ExcelExpiryDate.Value.AddDays(plan.Days) : DateTime.Now.AddDays(plan.Days);
                else
                    user.PlanExpireDate = user.PlanExpireDate > DateTime.Now ? user.PlanExpireDate.Value.AddDays(plan.Days) : DateTime.Now.AddDays(plan.Days);

                db.UserTokens.RemoveRange(db.UserTokens.Where(x => x.Id == user.Id));
                await db.SaveChangesAsync();

                await db.PurchaseHistories.Where(x => x.OrderId == request.OrderId).ForEachAsync(x =>
                {
                    x.Status = "paid"; x.PaymentId = request.PaymentId;
                });
                await db.SaveChangesAsync();
                //if (!db.PurchaseHistories.Any(x => x.PaymentId == request.PaymentId))
                //{

                //    db.PurchaseHistories.Add(new PurchaseHistory
                //    {
                //        Amount = plan.Amount,
                //        CreatedOnUtc = DateTime.Now,
                //        Email = user.Email,
                //        Mobile = user.Mobile,
                //        OrderId = request.OrderId,
                //        PaymentId = request.PaymentId,
                //        UserId = user?.Id,
                //        PlanId = plan.Id,
                //        UpdatedOnUtc = DateTime.Now,
                //        Status = "captured",

                //    });
                //    await db.SaveChangesAsync();
                //}

                //string contentRootPath = env.ContentRootPath;

                //string body = string.Empty;

                //await commonBL.SendMsg91Email("Planupdated_i4", $"{user.FirstName} {user.LastName}", $"{user.PlanExpireDate.Value:dd-MM-yyyy}", user.Email, $"{user.FirstName} {user.LastName}");
                //await commonBL.SendMsg91Email("Planupdated_i4", $"{user.FirstName} {user.LastName}", $"{user.PlanExpireDate.Value:dd-MM-yyyy}", "lvkeshb@gmail.com", $"{user.FirstName} {user.LastName}");


            }
        }

        #region Coupon
        public async Task CreateCoupon(OrderModel.CouponRequest request)
        {
            var coupon = db.Coupons.FirstOrDefault(x => x.Id == request.CouponId) ?? new Coupon() { CreatedOn = DateTime.Now, Active = true, Deleted = false };

            coupon.Name = request.Name;
            coupon.Description = request.Description;
            coupon.UpdatedOn = DateTime.Now;
            coupon.DiscountType = request.DiscountType.ToLower();
            coupon.DiscountValue = request.DiscountValue;
            coupon.Active = request.Active;
            if (coupon.Id == 0)
                db.Coupons.Add(coupon);
            await db.SaveChangesAsync();
        }
        public async Task<object> GetCoupons() => await db.Coupons.Where(x => !x.Deleted).ToListAsync();
        public object GetCouponDetails(string CouponCode) => db.Coupons.Where(x => x.Name == CouponCode.Trim()).FirstOrDefault();
        public async Task DeleteCoupon(int CouponId)
        {
            db.Coupons.FirstOrDefault(x => x.Id == CouponId).Deleted = true;
            await db.SaveChangesAsync();
        }
        #endregion
        public class RazorpayResponse
        {
            public string Event { get; set; }
            public RazorpayPayload payload { get; set; }
        }
        public class RazorpayPayload
        {
            public RazorpayPayment payment { get; set; }
        }
        public class RazorpayPayment
        {
            public RazorpayEntity entity { get; set; }
        }
        public class RazorpayEntity
        {
            public string id { get; set; }
            public string entity { get; set; }
            public double amount { get; set; }
            public string currency { get; set; }
            public string status { get; set; }
            public string order_id { get; set; }
            public string email { get; set; }
            public string contact { get; set; }
            public int created_at { get; set; }
            public Notes notes { get; set; }
        }
        public class Notes
        {
            public string userid { get; set; }
        }
        public class RazorpayPaymentDetails
        {
            public string email { get; set; }
            public string status { get; set; }
        }

    }
}
