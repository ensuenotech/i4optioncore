using DocumentFormat.OpenXml.Presentation;
using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using i4optioncore.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static i4optioncore.Models.UserModel;
using Position = i4optioncore.DBModelsUser.Position;
//using Twilio.Http;

namespace i4optioncore.Repositories
{
    public class UserBL : IUserBL
    {
        I4optionUserDbContext dbuser;
        DBModels.i4option_dbContext db;
        IConfiguration _config;
        IAzureBL azureBL;
        IRedisBL redisBL;
        readonly ICommonBL commonBL;
        private readonly ICacheBL cacheBL;
        private readonly IWebHostEnvironment env;

        public UserBL(I4optionUserDbContext _db, DBModels.i4option_dbContext __db, ICommonBL _commonBL, IConfiguration config, IWebHostEnvironment _env,  IAzureBL _azureBL,
            IRedisBL _redisBL, ICacheBL cacheBL)
        {
            dbuser = _db;
            commonBL = _commonBL;
            db = __db;
            _config = config;
            env = _env;
            redisBL = _redisBL;
            azureBL = _azureBL;
            this.cacheBL = cacheBL;
        }
        public async Task<ValidateUserResponse> ValidateOtp(string Otp, int UserId)
        {
            var res = await dbuser.Otps.Where(x => x.UserId == UserId).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();
            var c = await dbuser.Users.FirstOrDefaultAsync(x => x.Id == UserId && x.Deleted == false);

            if (res != null && !string.IsNullOrEmpty(res.Otp1))
            {
                if (res.Otp1 == Otp)
                {
                    if (c.Status == "ACTIVE" || c.Status == "PENDING")
                    {

                        // dbuser.Users.FirstOrDefault(x => x.Id == UserId).Status = "EMAIL_PENDING";
                        // dbuser.SaveChanges();

                        var user = new UserModel.JwtUser { Email = c.Email, UserId = c.Id.ToString(), UserType = c.UserType, PlanExpireDate = c.PlanExpireDate, EODExpireDate = c.NiftyPlanExpireDate };
                        var token = GenerateJSONWebToken(user);
                        var userToken = dbuser.UserTokens.FirstOrDefault(x => x.UserId == c.Id && x.Remarks == "i4option") ?? new UserToken();
                        userToken.Token = token;
                        userToken.UpdatedOnUtc = DateTime.Now;
                        if (userToken.Id == 0)
                        {
                            userToken.UserId = c.Id;
                            userToken.Remarks = "i4option";
                            dbuser.UserTokens.Add(userToken);
                        }
                        await dbuser.SaveChangesAsync();

                        if (c.PlanExpireDate <= DateTime.Now && c.NiftyPlanExpireDate <= DateTime.Now && c.UserType != "ADMIN")
                            return new ValidateUserResponse { Result = true, Message = "plan_expired", Token = token };
                        //c.LastLoginDateUtc = DateTime.Now;
                        //c.LastActivityDateUtc = DateTime.Now;
                        //await dbuser.SaveChangesAsync();

                        return new ValidateUserResponse { Result = true, Message = c.Id.ToString(), Token = token, UserType = c.UserType };
                    }
                    else if (c.Status == "EMAIL_PENDING")
                    {
                        return new ValidateUserResponse { Result = false, Message = "email_not_verified" };

                    }

                }
                else
                {
                    return new ValidateUserResponse { Result = false, Message = "incorrect_password" };

                }
            }
            return null;
        }
        public async Task<int> SendOTP(int? userId, string type, string Mobile)
        {
            var user = await dbuser.Users.FirstOrDefaultAsync(x => x.Mobile == Mobile) ?? throw new Exception("Mobile number not registered. Please contact admin");
            type = type.ToLower();
            string contentRootPath = env.ContentRootPath;

            var otp = GenerateRandomOTP();
            dbuser.Otps.Add(new Otp
            {
                CreatedOn = DateTime.Now,
                Otp1 = otp,
                UserId = user.Id,
            });
            await dbuser.SaveChangesAsync();

            if (Mobile != null)
            {
                user.Mobile = Mobile;
                await dbuser.SaveChangesAsync();
            }
            if (user != null)
            {
                if (type == "mail")
                {
                    //                    string body = string.Empty;

                    //                    body = string.Format(@" Hi {0} {1},<br/>
                    //<br/>
                    //{2} is your verification code. This code is valid for 5 minutes. <br/>
                    //<br/>
                    //<br/>
                    //<br/>
                    //For customer care support and help:-<br/>
                    //Whatsapp:- +91 9330000029<br/>
                    //Mail:- support@ifil.co.in<br/>
                    //<br/>
                    //<br/>
                    //Do not reply to this mail.<br/>
                    //<br/>
                    //Thanks and regards,<br/>
                    //i4option", user.FirstName, user.LastName, otp);
                    //using (StreamReader reader = new StreamReader(contentRootPath + "/emails/email.html", Encoding.Default)) // Path to your 
                    //{
                    //    body = reader.ReadToEnd();
                    //    body = body.Replace("[[--NAME--]]", string.Format("{0} {1}", user.FirstName, user.LastName));
                    //    body = body.Replace("[[--CONTENT--]]", string.Format("{0} is your verification code.", otp));
                    //}
                    //var mail = new CommonModel.SendMailDetails
                    //{
                    //    To = user.Email,
                    //    Subject = string.Format("OTP to reset Password"),
                    //    Body = body
                    //};

                    //await commonBL.SendViaSendGridAsync(mail.Subject, mail.To, "i4option User", mail.Body, null);
                    //await commonBL.SendMail(mail);

                    await commonBL.SendSms(user.Mobile, otp);
                    //await commonBL.SendWhatsapp(user.Mobile,,$"{user.FirstName}{user.LastName}")

                    //await commonBL.SendMsg91Email("Password_reset_i4", $"{user.FirstName} {user.LastName}", otp, user.Email, $"{user.FirstName} {user.LastName}");
                }
                else if (type == "whatsapp")
                {
                    //await commonBL.SendSms(user.Mobile, otp);
                    await commonBL.SendWhatsapp($"+91{user.Mobile}", "otp2", user.FirstName, "", "", "", otp);
                    //twilioBL.SendSMS("Your OTP for validation on i4option.com: " + otp, user.Mobile);
                }
                else if (type == "sms")
                {
                    await commonBL.SendSms(user.Mobile, otp);
                    //await commonBL.SendWhatsapp($"+91{user.Mobile}", "code", user.FirstName, "", "", "", otp);
                    //twilioBL.SendSMS("Your OTP for validation on i4option.com: " + otp, user.Mobile);
                }
            }

            return user.Id;
        }

        public async Task<ValidateUserResponse> ValidateUserByToken(string token)
        {
            if (dbuser != null)
            {
                var userToken = dbuser.UserTokens.FirstOrDefault(x => x.Token == token);
                if (userToken == null) return null;
                var c = await dbuser.Users.FirstOrDefaultAsync(x => x.Id == userToken.UserId && x.Deleted == false);
                if (c == null)
                {
                    return new ValidateUserResponse { Result = false, Message = "no_user" };
                }
                else
                {
                    if (c.Status == "ACTIVE")
                    {

                        var user = new UserModel.JwtUser { Email = c.Email, UserId = c.Id.ToString(), UserType = c.UserType };
                        //var _token = GenerateJSONWebToken(user);
                        //var _userToken = dbuser.UserTokens.FirstOrDefault(x => x.UserId == c.Id && x.Remarks == "i4option") ?? new UserToken();
                        //_userToken.Token = _token;
                        //_userToken.UpdatedOnUtc = DateTime.Now;
                        //if (_userToken.Id == 0)
                        //{
                        //    _userToken.UserId = c.Id;
                        //    _userToken.Remarks = "i4option";
                        //    dbuser.UserTokens.Add(_userToken);
                        //}
                        //await dbuser.SaveChangesAsync();

                        if (c.PlanExpireDate < DateTime.Now && c.UserType != "ADMIN")
                            //return new ValidateUserResponse { Result = true, Message = "plan_expired", Token = _token };
                            return new ValidateUserResponse { Result = true, Message = "plan_expired", Token = userToken.Token };


                        //return new ValidateUserResponse { Result = true, Message = c.Id.ToString(), Token = _token, UserType = c.UserType };
                        return new ValidateUserResponse { Result = true, Message = c.Id.ToString(), Token = userToken.Token, UserType = c.UserType };

                    }
                    else if (c.Status == "EMAIL_PENDING")
                    {
                        return new ValidateUserResponse { Result = false, Message = "email_not_verified" };

                    }
                    else if (c.Status == "PENDING")
                    {
                        return new ValidateUserResponse { Result = false, Message = "mobile_not_verified" };

                    }

                }
            }
            return null;
        }
        public async Task<bool> SendWelcomeEmail(int userId)
        {
            var user = dbuser.Users.FirstOrDefault(x => x.Id == userId);
            user.Status = "ACTIVE";
            await dbuser.SaveChangesAsync();
            string contentRootPath = env.ContentRootPath;

            //            string body = string.Empty;

            //            body = string.Format(@"Hi {0} {1},<br/>
            //Thanks for joining us at i4option.com<br/>
            //<br/>
            //Your account is updated for lifetime access to the free plan and  3 days free access to all premium features.<br/>
            //<br/>
            //For better experience we recommend Chrome latest version and with setting Hardware acceleration off.  (https://www.howtogeek.com/412738/how-to-turn-hardware-acceleration-on-and-off-in-chrome.)
            //<br/>
            //We built this web application for retail equity derivative traders so that they can get real-time data information which is simplified and easy to understand. <br/>
            //Your feedback & suggestions help us to make sure that we’re delivering exactly what traders want. Write to us and let us know.<br/>
            //Over the next few weeks, we’ll be sending you a few more emails to help you gain maximum value from this web application. We’ll share our favorite tips and provide some exciting updates in the near time.<br/>
            //<br/><br/>
            //For customer care support and help:-<br/>
            //Whatsapp:- +91 9330000029<br/>
            //Mail:- support@ifil.co.in<br/>
            //<br/>
            //<br/>
            //<br/>
            //<br/>
            //<br/>
            //Do not reply to this mail.<br/>
            //<br/>
            //Thanks and regards,<br/>
            //i4option <br/>
            //", user.FirstName, user.LastName);
            //using (StreamReader reader = new StreamReader(contentRootPath + "/emails/welcome-email.html", Encoding.Default)) // Path to your 
            //{
            //    body = reader.ReadToEnd();
            //    body = body.Replace("[[--NAME--]]", string.Format("{0} {1}", user.FirstName, user.LastName));
            //}
            //var mail = new CommonModel.SendMailDetails
            //{
            //    To = user.Email,
            //    Subject = string.Format("Welcome mail"),
            //    Body = body
            //};


            //await commonBL.SendViaSendGridAsync(mail.Subject, mail.To, "i4option User", mail.Body, null);

            //await commonBL.SendMsg91Email("wlecome_1", $"{user.FirstName} {user.LastName}", $"{user.FirstName} {user.LastName}", user.Email, $"{user.FirstName} {user.LastName}");

            return true;
        }
        public async Task<bool> DeleteUser(int id)
        {
            var user = await dbuser.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                user.Deleted = true;
                await dbuser.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await dbuser.Countries.Where(x => x.Active && !x.Deleted).ToListAsync();
        }
        public async Task<List<State>> GetAllStates(int CountryId)
        {
            return await dbuser.States.Where(x => x.Active && !x.Deleted && x.CountryId == CountryId).ToListAsync();
        }
        public async Task<bool> ChangePassword(UserModel.ChangePasswordForm _user)
        {
            var user = await dbuser.Users.FirstOrDefaultAsync(x => x.Id == _user.UserId);
            if (user != null)
            {
                user.Password = _user.Password;
                await dbuser.SaveChangesAsync();
                return true;
            }
            return false;

        }
        public async Task<ValidateUserResponse> ValidateUser(UserModel.ValidateUser data)
        {
            if (dbuser != null)
            {
                var c = await dbuser.Users.FirstOrDefaultAsync(x => x.Email != null && x.Email == data.Email.ToLower().Trim() && x.Deleted == false);
                if (c == null)
                {
                    return new ValidateUserResponse { Result = false, Message = "no_user" };
                }
                else
                {
                    if (c.Status == "ACTIVE")
                    {
                        if (c.Password == null)
                        {
                            return new ValidateUserResponse { Result = false, Message = "reset_password" };
                        }
                        if (c.Password == data.Password)
                        {
                            var user = new UserModel.JwtUser { Email = c.Email, UserId = c.Id.ToString(), UserType = c.UserType, PlanExpireDate = c.PlanExpireDate, EODExpireDate = c.NiftyPlanExpireDate };
                            var token = GenerateJSONWebToken(user);
                            var userToken = dbuser.UserTokens.FirstOrDefault(x => x.UserId == c.Id && x.Remarks == "i4option") ?? new UserToken();
                            userToken.Token = token;
                            userToken.UpdatedOnUtc = DateTime.Now;
                            if (userToken.Id == 0)
                            {
                                userToken.UserId = c.Id;
                                userToken.Remarks = "i4option";
                                dbuser.UserTokens.Add(userToken);
                            }
                            await dbuser.SaveChangesAsync();

                            if (c.PlanExpireDate <= DateTime.Now && c.NiftyPlanExpireDate <= DateTime.Now && c.UserType != "ADMIN")
                                return new ValidateUserResponse { Result = true, Message = "plan_expired", Token = token };
                            //c.LastLoginDateUtc = DateTime.Now;
                            //c.LastActivityDateUtc = DateTime.Now;
                            //await dbuser.SaveChangesAsync();

                            return new ValidateUserResponse { Result = true, Message = c.Id.ToString(), Token = token, UserType = c.UserType };
                        }
                        else
                        {
                            return new ValidateUserResponse { Result = false, Message = "incorrect_password" };
                        }
                    }
                    else if (c.Status == "EMAIL_PENDING")
                    {
                        return new ValidateUserResponse { Result = false, Message = "email_not_verified" };

                    }
                    else if (c.Status == "PENDING")
                    {
                        return new ValidateUserResponse { Result = false, Message = "mobile_not_verified" };

                    }

                }
            }
            return null;
        }
        public async Task<RefreshTokenResponse> RefreshToken(string Token)
        {
            var token = dbuser.UserTokens.FirstOrDefault(x => x.Token == Token && x.Remarks == "i4option");
            if (token != null)
            {
                //var c = await dbuser.Users.FirstOrDefaultAsync(x => x.Id==token.UserId);
                //if (c.PlanExpireDate <= DateTime.Now && c.UserType != "ADMIN")
                //    return new RefreshTokenResponse { Success = false, Message = "TOKEN_EXPIRED" };

                if (token.UpdatedOnUtc.Date == DateTime.Now.Date)
                {
                    token.UpdatedOnUtc = DateTime.Now;
                    await dbuser.SaveChangesAsync();
                    return new RefreshTokenResponse { Success = true, Message = token.Token };
                }
                else
                {
                    return new RefreshTokenResponse { Success = false, Message = "TOKEN_EXPIRED" };
                }

            }
            else
            {
                return new RefreshTokenResponse { Success = false, Message = "TOKEN_NOT_FOUND" };
            }
        }
        private string GenerateJSONWebToken(UserModel.JwtUser userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserType),

        new Claim(JwtRegisteredClaimNames.NameId, userInfo.UserId),
        new Claim( "LIVE_PLAN_EXPIRE_DATE",userInfo.PlanExpireDate.ToString()),
        new Claim( "EOD_PLAN_EXPIRE_DATE",userInfo.EODExpireDate.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, userInfo.Email) };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"], claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<List<UserModel.UserDetails>> GetAllUsers()
        {
            return await dbuser.Users.Where(x => !x.Deleted)
                .Include(x => x.CustomerAddresses)
                .ThenInclude(x => x.Address)
                .ThenInclude(x => x.State).Select(x => new UserModel.UserDetails
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    CreatedOn = x.CreatedOnUtc,
                    Password = x.Password,
                    PlanExpireDate = x.PlanExpireDate != null ? x.PlanExpireDate.Value : null,
                    AppPlanExpireDate = x.AppPlanExpireDate != null ? x.AppPlanExpireDate.Value : null,
                    BTPlanExpireDate = x.BtexpiryDate != null ? x.BtexpiryDate.Value : null,
                    ExcelPlanExpireDate = x.ExcelExpiryDate != null ? x.ExcelExpiryDate.Value : null,
                    Id = x.Id,
                    UpdatedOn = x.UpdatedOnUtc,
                    NiftyPlanExpireDate = x.NiftyPlanExpireDate,
                    UserType = x.UserType,
                    Mobile = x.Mobile,
                    SocialProfileName = x.SocialProfileName,
                    Status = (x.PlanExpireDate >= DateTime.Now) ? "ACTIVE" : (x.PlanExpireDate < DateTime.Now) ? "EXPIRED" : "PENDING",
                    ReferalCode = x.ReferalCode ?? EncryptNumber(x.Id),
                    AffiliateCode = x.AffiliateCode,
                    Address = x.CustomerAddresses.Select(address => new UserModel.AddressDetails
                    {
                        Id = address.Id,
                        Address = address.Address.Address1,
                        City = address.Address.City,
                        Country = address.Address.Country.Name,
                        FirstName = address.Address.FirstName,
                        LastName = address.Address.LastName,
                        CountryId = address.Address.CountryId,
                        PinCode = address.Address.PinCode,
                        StateId = address.Address.StateId,
                        State = address.Address.State.Name,
                    }).FirstOrDefault(),
                    WalletBalance = dbuser.Wallets.Where(w => w.UserId == x.Id).Sum(x => x.Amount) + dbuser.Positions.Where(w => w.UserId == x.Id).Sum(x => x.PandL)
                }).ToListAsync();
        }
        public async Task<UserModel.UserDetails> GetUserDetails(int Id)
        {
            var x = await dbuser.Users.Where(u => u.Id == Id).Include(x => x.DhanCredentials).FirstOrDefaultAsync();
            var ReferalCode = EncryptNumber(Id);

            var affiliates = await (from u in dbuser.Users

                                    join ph in dbuser.PurchaseHistories on u.Id equals ph.UserId into ju
                                    from p in ju.DefaultIfEmpty()
                                    where u.AffiliateCode == ReferalCode
                                    //&& !string.IsNullOrEmpty(p.PaymentId)
                                    select new { u, p }).ToListAsync();



            return new UserModel.UserDetails
            {
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                CreatedOn = x.CreatedOnUtc,
                Password = x.Password,
                PlanExpireDate = x.PlanExpireDate,
                NiftyPlanExpireDate = x.NiftyPlanExpireDate,
                ExcelPlanExpireDate = x.ExcelExpiryDate,
                BTPlanExpireDate = x.BtexpiryDate,
                Id = x.Id,
                UpdatedOn = x.UpdatedOnUtc,
                UserType = x.UserType,
                Address = GetAddress(x.Id),
                Status = x.Status,
                SocialProfileName = x.SocialProfileName,
                Mobile = x.Mobile,
                AppToken = x.AppToken,
                AppPlanExpireDate = x.AppPlanExpireDate,
                UserPayments = dbuser.PurchaseHistories.Where(p => !string.IsNullOrEmpty(p.PaymentId) && p.Email.ToLower().Equals(x.Email.ToLower())).Select(p => new UserModel.UserPaymentDetails
                {
                    Amount = p.Amount,
                    OrderId = p.OrderId,
                    PaymentDate = p.UpdatedOnUtc,
                    PaymentId = p.PaymentId,
                    Status = p.Status
                }).ToList(),
                Affiliates = (from a in affiliates
                              group a by a.u into ug
                              select
                                               new UserModel.AffiliateDetails
                                               {
                                                   Email = ug.Key.Email,
                                                   FirstName = ug.Key.FirstName,
                                                   LastName = ug.Key.LastName,
                                                   RegistrationDate = ug.Key.CreatedOnUtc,
                                                   UserId = ug.Key.Id,
                                                   Payments = ug.Any(x => x.p != null) ? ug.Select(p => new
                                                   {
                                                       p.p.Amount,
                                                       p.p.OrderId,
                                                       Date = p.p.CreatedOnUtc
                                                   }).ToList() : null
                                               }
                               ).ToList(),

                OINumbers = x.ShowOinumbers,
                Dhan = new UserModel.IDhanTokenDetails
                {
                    ClientId = x.DhanCredentials.FirstOrDefault()?.DhanClientId,
                    ClientUcc = x.DhanCredentials.FirstOrDefault()?.DhanUcc,
                    Token = x.DhanCredentials.FirstOrDefault()?.AccessToken,
                },
                ReferalCode = x.ReferalCode ?? EncryptNumber(x.Id),
                AffiliateCode = x.AffiliateCode,
                WalletBalance = x.WalletBalance ?? 0,
            };
        }
        public UserModel.AddressDetails GetAddress(int UserId)
        {
            var a = dbuser.CustomerAddresses.FirstOrDefault(a => a.UserId == UserId);
            if (a != null)
            {
                var address = dbuser.Addresses.FirstOrDefault(x => x.Id == a.AddressId);
                return new UserModel.AddressDetails
                {
                    Id = address.Id,
                    Address = address.Address1,
                    City = address.City,
                    Country = dbuser.Countries.FirstOrDefault(c => c.Id == address.CountryId).Name,
                    FirstName = address.FirstName,
                    LastName = address.LastName,
                    CountryId = address.CountryId,
                    PinCode = address.PinCode,
                    StateId = address.StateId,
                    State = dbuser.States.FirstOrDefault(c => c.Id == address.StateId).Name,
                };
            }
            return null;
        }
        public async Task<int> Register(UserModel.UserDetailsForm _user)
        {
            var userid = await SaveUserDetails(_user);
            string contentRootPath = env.ContentRootPath;

            var user = dbuser.Users.FirstOrDefault(u => u.Id == userid);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
        new Claim("userId", userid.ToString())};
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"], claims
              ,
              expires: DateTime.Now.AddHours(24),
              signingCredentials: credentials);

            var _token = new JwtSecurityTokenHandler().WriteToken(token);


            if (user != null)
            {
                //string body = string.Empty;

                string linkText = string.Format("<a href='https://www.i4option.com/#/register/validate-otp?id={0}'>Click here </a>", _token);

                //                body = string.Format(@"Hi {0} {1},<br/>
                //To activate your i4option Account, please verify your email address.<br/>
                //<br/>
                //{2}<br/>
                //<br/>
                //This link is valid for the next 24 hours only.<br/>
                //<br/>
                //<br/>
                //<br/>
                //For Non-Indian Mobile Verification, Please contact the admin:-<br/>
                //Whatsapp:- +91 9330000029<br/>
                //Mail:- support@ifil.co.in<br/>
                //<br/>
                //<br/>
                //<br/>
                //Do not reply to this mail.<br/>
                //<br/>
                //Thanks!", user.FirstName, user.LastName, linkText);
                //using (StreamReader reader = new StreamReader(contentRootPath + "/emails/email.html", Encoding.Default)) // Path to your 
                //{
                //    body = reader.ReadToEnd();
                //    body = body.Replace("[[--NAME--]]", string.Format("{0} {1}", user.FirstName, user.LastName));
                //    body = body.Replace("[[--CONTENT--]]", string.Format("Plese click on the link below:<br>" +
                //        "<a href='https://www.i4option.com/#/register/validate-otp?id={0}'>Click here </a>.<br/>This link is valid for next 24 hours only.", _token));
                //}


                //var mail = new CommonModel.SendMailDetails
                //{
                //    To = user.Email,
                //    Subject = string.Format("Verify your email"),
                //    Body = body
                //};

                //await commonBL.SendViaSendGridAsync(mail.Subject, mail.To, "i4option User", mail.Body, null);

                //await commonBL.SendMail(mail);

                //await commonBL.SendMsg91Email("verify_i4", $"{user.FirstName} {user.LastName}", linkText, user.Email, $"{user.FirstName} {user.LastName}");

            }

            return userid;

            //await SendOTP(userid);

        }
        public async Task<RegisterViaMobileResponse> RegisterViaMobile(SendOtpRequest request)
        {
            var checkuser = dbuser.Users.FirstOrDefault(x => x.Mobile == request.Mobile && (x.Status == "PENDING" || x.Status == "EMAIL_PENDING"));
            if (checkuser != null)
            {
                if (checkuser.Status == "PENDING")
                {
                    var otp = await SendOTP(checkuser.Id, request.Type, request.Mobile);
                    return new RegisterViaMobileResponse { Id = checkuser.Id, Success = true, Status = checkuser.Status };
                }
                else
                {
                    var _user = new UserModel.UserDetailsForm()
                    {
                        Id = checkuser.Id,
                        Email = checkuser.Email,
                        FirstName = checkuser.FirstName,
                        LastName = checkuser.LastName,
                        Password = checkuser.Password,
                        UserType = "USER",
                        //Status = "EMAIL_PENDING",
                        Status = "ACTIVE",
                    };

                    await Register(_user);
                    return new RegisterViaMobileResponse
                    {
                        Id = checkuser.Id,
                        Success = true,
                        Status = checkuser.Status,
                        User = new UserModel.UserDetails
                        {
                            FirstName = checkuser.FirstName,
                            LastName = checkuser.LastName,
                            Email = checkuser.Email,
                            Mobile = checkuser.Mobile
                        }
                    };
                }
            }
            else
            {
                if (dbuser.Users.Any(x => x.Mobile == request.Mobile && x.Status == "ACTIVE"))
                {
                    return new RegisterViaMobileResponse { Success = false, Status = "USER_ALREADY_EXISTS" };
                }
                else
                {
                    var user = new User
                    {
                        Mobile = request.Mobile,
                        Email = "",
                        FirstName = "",
                        LastName = "",
                        UserType = "USER",
                        CreatedOnUtc = DateTime.Now,
                        UpdatedOnUtc = DateTime.Now,
                        Status = "PENDING",
                        WalletBalance = 500000
                    };
                    dbuser.Users.Add(user);

                    dbuser.Wallets.Add(new Wallet { Amount = 50000, CreatedOn = DateTime.Now, UserId = user.Id });

                    await dbuser.SaveChangesAsync();
                    var otp = await SendOTP(user.Id, request.Type, request.Mobile);
                    return new RegisterViaMobileResponse { Id = user.Id, Success = true, Status = user.Status };
                }
            }
        }

        public async Task UpdateAppToken(int UserId, string AppToken)
        {
            var user = dbuser.Users.FirstOrDefault(x => x.Id == UserId);
            if (user != null)
            {
                user.AppToken = AppToken;
                await dbuser.SaveChangesAsync();
            }


        }
        public async Task<int> SaveUserDetails(UserModel.UserDetailsForm _user)
        {

            var user = dbuser.Users.FirstOrDefault(x => x.Id == _user.Id) ?? new User()
            {
                CreatedOnUtc = DateTime.Now,
                WalletBalance = 500000
            };

            user.UpdatedOnUtc = DateTime.Now;
            user.LastName = _user.LastName;
            user.FirstName = _user.FirstName;
            if (_user.Email != null)
                user.Email = _user.Email.ToLower();
            if (_user.Password != null)
                user.Password = _user.Password;
            if (_user.PlanExpireDate != null)
                user.PlanExpireDate = _user.PlanExpireDate;
            if (_user.NiftyPlanExpireDate != null)
                user.NiftyPlanExpireDate = _user.NiftyPlanExpireDate;
            if (_user.AppPlanExpireDate != null)
                user.AppPlanExpireDate = _user.AppPlanExpireDate;
            if (_user.ExcelPlanExpireDate != null)
                user.ExcelExpiryDate = _user.ExcelPlanExpireDate;
            if (_user.BTPlanExpireDate != null)
                user.BtexpiryDate = _user.BTPlanExpireDate;
            if (_user.UserType != null)
                user.UserType = _user.UserType;
            if (_user.Mobile != null)
                user.Mobile = _user.Mobile;

            if (_user.Status != null)
                user.Status = _user.Status;
            if (_user.SocialProfileName != null)
                user.SocialProfileName = _user.SocialProfileName;
            if (_user.AffiliateCode != null)
                user.AffiliateCode = _user.AffiliateCode;

            user.ShowOinumbers = _user.ShowOINumbers ?? true;
            if (user.Id == 0)
            {
                dbuser.Users.Add(user);
            };
            await dbuser.SaveChangesAsync();

            if (_user.StateId.HasValue && _user.CountryId.HasValue)
                await SaveAddress(_user.FirstName, _user.LastName, _user.Address, _user.StateId.Value, _user.CountryId.Value, _user.Pincode, _user.City, user.Id, _user.GSTIN);


            return user.Id;

        }
        public async Task<int> SaveAddress(string FirstName, string LastName, string Address, int StateId, int CountryId, string Pincode, string City, int UserId, string GSTIN)
        {
            var address = new Address
            {
                FirstName = FirstName,
                LastName = LastName,
                Address1 = Address,
                StateId = StateId,
                PinCode = Pincode,
                City = City,
                CreatedOnUtc = DateTime.Now,
                UpdatedOnUtc = DateTime.Now,
                CountryId = CountryId,
                Gstin = GSTIN
            };
            dbuser.Addresses.Add(address);
            await dbuser.SaveChangesAsync();

            dbuser.CustomerAddresses.RemoveRange(dbuser.CustomerAddresses.Where(x => x.UserId == UserId));
            dbuser.CustomerAddresses.Add(new CustomerAddress
            {
                AddressId = address.Id,
                UserId = UserId
            });
            await dbuser.SaveChangesAsync();

            return address.Id;

        }


        public async Task<int?> SearchUser(string query, string type)
        {
            var user = new User();
            if (type == "mail")
            {
                user = await dbuser.Users.FirstOrDefaultAsync(x => x.Email == query.ToLower());

            }
            else if (type == "mobile")
            {
                user = await dbuser.Users.FirstOrDefaultAsync(x => x.Mobile == query);

            }
            if (user == null)
                return null;
            else
                return user.Id;
        }

        private string GenerateRandomOTP()

        {
            var iOTPLength = 6;
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string sOTP = String.Empty;

            string sTempChars = String.Empty;

            Random rand = new Random();

            for (int i = 0; i < iOTPLength; i++)

            {

                int p = rand.Next(0, saAllowedCharacters.Length);

                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];

                sOTP += sTempChars;

            }

            return sOTP;

        }
        public ValidateUserResponse ValidateInternal(string username, string password)
        {
            if (password == "iampassword")
            {
                var user = new UserModel.JwtUser { Email = username, UserId = "0", UserType = "INTERNAL", PlanExpireDate = DateTime.MinValue };
                var token = GenerateJSONWebToken(user);
                //var userToken = new UserToken();
                //userToken.Token = token;
                //userToken.UpdatedOnUtc = DateTime.Now;
                //if (userToken.Id == 0)
                //{
                //    userToken.UserId = 0;
                //    userToken.Remarks = "i4option";
                //    dbuser.UserTokens.Add(userToken);
                //}
                //await dbuser.SaveChangesAsync();
                return new ValidateUserResponse { Result = true, Message = "NA", Token = token, UserType = "INTERNAL" };
            }
            return new ValidateUserResponse { Result = false };
        }
        #region Builder Strategy
        public async Task<List<BuilderStrategy>> GetBuilderStrategies(int UserId)
        {
            return await dbuser.BuilderStrategies.Where(x => x.UserId == UserId && !x.Deleted).Select(x => new BuilderStrategy
            {
                CreatedOn = x.CreatedOn,
                Id = x.Id,
                StrategyName = x.StrategyName,
                UpdatedOn = x.UpdatedOn,
                UserId = x.UserId,
                BuilderStrategyComponents = dbuser.BuilderStrategyComponents.Where(bs => bs.StrategyId == x.Id && !bs.Deleted).Select(bs => new BuilderStrategyComponent
                {
                    Id = bs.Id,
                    CreatedOn = bs.CreatedOn,
                    FutureSymbolName = bs.FutureSymbolName,
                    LastQuoteFutPrice = bs.LastQuoteFutPrice,
                    LastQuotePrice = bs.LastQuotePrice,
                    Note = bs.Note,
                    SavedFuturePrice = bs.SavedFuturePrice,
                    SavedSpotPrice = bs.SavedSpotPrice,
                    SpotSymbolName = bs.SpotSymbolName,
                    Strategy = bs.Strategy,
                    StrategyId = bs.StrategyId,
                    SymbolName = bs.SymbolName,
                    TradeTime = bs.TradeTime,
                    UpdatedOn = bs.UpdatedOn,
                    BuilderStrategySubComponents = dbuser.BuilderStrategySubComponents.Where(x => x.StrategyComponentId == bs.Id).ToList()
                }).ToList()
            }).ToListAsync();
        }
        public async Task<List<BuilderStrategyComponent>> GetBuilderStrategyComponents(int BuilderStrategyId)
        {
            return await dbuser.BuilderStrategyComponents.Where(x => x.StrategyId == BuilderStrategyId && !x.Deleted).ToListAsync();
        }
        public async Task<List<BuilderStrategySubComponent>> GetBuilderStrategySubComponents(int BuilderStrategySubId)
        {
            return await dbuser.BuilderStrategySubComponents.Where(x => x.StrategyComponentId == BuilderStrategySubId).ToListAsync();
        }
        public async Task<int> SaveBuilderStrategy(int BuilderStrategyId, string StrategyName, int UserId)
        {
            var builderStrategy = dbuser.BuilderStrategies.FirstOrDefault(x => x.Id == BuilderStrategyId) ?? new BuilderStrategy();
            builderStrategy.StrategyName = StrategyName;
            builderStrategy.UpdatedOn = DateTime.Now;
            if (builderStrategy.Id == 0)
            {
                builderStrategy.CreatedOn = DateTime.Now;
                builderStrategy.UserId = UserId;
                builderStrategy.Deleted = false;
                dbuser.BuilderStrategies.Add(builderStrategy);
            }
            await dbuser.SaveChangesAsync();

            return builderStrategy.Id;
        }
        public async Task<int> SaveBuilderStrategyComponent(int BuilderStrategyComponentId, int BuilderStrategyId, string FutureSymbolName,
            decimal? LastQuoteFutPrice, decimal? LastQuotePrice, decimal SavedFuturePrice, decimal SavedSpotPrice, string SpotSymbolName, string SymbolName,
            DateTime TradeTime, string Note)
        {
            var builderStrategyComponent = dbuser.BuilderStrategyComponents.FirstOrDefault(x => x.Id == BuilderStrategyComponentId) ?? new BuilderStrategyComponent();
            builderStrategyComponent.FutureSymbolName = FutureSymbolName;
            builderStrategyComponent.LastQuoteFutPrice = LastQuoteFutPrice;
            builderStrategyComponent.LastQuotePrice = LastQuotePrice;
            builderStrategyComponent.SavedFuturePrice = SavedFuturePrice;
            builderStrategyComponent.SavedSpotPrice = SavedSpotPrice;
            builderStrategyComponent.SpotSymbolName = SpotSymbolName;
            builderStrategyComponent.StrategyId = BuilderStrategyId;
            builderStrategyComponent.SymbolName = SymbolName;
            builderStrategyComponent.TradeTime = TradeTime;
            builderStrategyComponent.UpdatedOn = DateTime.Now;
            builderStrategyComponent.Note = Note;
            if (builderStrategyComponent.Id == 0)
            {
                builderStrategyComponent.CreatedOn = DateTime.Now;
                dbuser.BuilderStrategyComponents.Add(builderStrategyComponent);
            }
            await dbuser.SaveChangesAsync();
            return builderStrategyComponent.Id;
        }
        public async Task<int> SaveBuilderStrategySubComponent(int BuilderStrategyComponentId, decimal? Delta, decimal EntryPrice,
            decimal? ExitPrice, decimal? Iv, DateTime Expiry, decimal? LastQuoteLtp, decimal LotQty, int LotSize, string OptionType,
            decimal? Pnl, decimal Strike, decimal? Theta, decimal? Vega, string StrikeSymbolName, string TradeType, int? SymbolId, DateTime? updatedOn)
        {

            var builderStrategy = new BuilderStrategySubComponent
            {
                Delta = Delta,
                EntryPrice = EntryPrice,
                ExitPrice = ExitPrice,
                Expiry = Expiry,
                Iv = Iv,
                LastQuoteLtp = LastQuoteLtp,
                LotQty = LotQty,
                LotSize = LotSize,
                OptionType = OptionType,
                Pnl = Pnl,
                StrategyComponentId = BuilderStrategyComponentId,
                Strike = Strike,
                StrikeSymbolName = StrikeSymbolName,
                SymbolId = SymbolId,
                Theta = Theta,
                TradeType = TradeType,
                Vega = Vega,
                UpdatedOn = updatedOn ?? DateTime.Now
            };
            dbuser.BuilderStrategySubComponents.Add(builderStrategy);

            await dbuser.SaveChangesAsync();

            return builderStrategy.Id;


        }
        public async Task<bool> UpdateExitPrice(int Id, decimal ExitPrice)
        {

            var res = await dbuser.BuilderStrategySubComponents.FirstOrDefaultAsync(x => x.Id == Id);
            if (res != null)
            {
                res.ExitPrice = ExitPrice;
                await dbuser.SaveChangesAsync();

                return true;
            }
            return false;


        }

        public async Task<bool> DeleteBuilderStrategy(int StrategyId)
        {
            if (dbuser.BuilderStrategies.Any(x => x.Id == StrategyId))
            {
                dbuser.BuilderStrategies.FirstOrDefault(x => x.Id == StrategyId).Deleted = true;
                await dbuser.SaveChangesAsync();
                return true;
            }
            else return false;
        }
        public async Task<bool> DeleteBuilderStrategyComponent(int BuilderStrategyComponentId)
        {
            if (dbuser.BuilderStrategyComponents.Any(x => x.Id == BuilderStrategyComponentId))
            {
                dbuser.BuilderStrategyComponents.FirstOrDefault(x => x.Id == BuilderStrategyComponentId).Deleted = true;
                await dbuser.SaveChangesAsync();
                return true;
            }
            else return false;
        }
        public async Task<bool> DeleteBuilderStrategySubComponent(int BuilderStrategySubComponentId)
        {
            if (dbuser.BuilderStrategySubComponents.Any(x => x.Id == BuilderStrategySubComponentId))
            {
                dbuser.BuilderStrategySubComponents.RemoveRange(dbuser.BuilderStrategySubComponents.Where(x => x.Id == BuilderStrategySubComponentId));
                await dbuser.SaveChangesAsync();
                return true;
            }
            else return false;
        }
        #endregion

        #region AdminNotification 
        public async Task<bool> SaveAdminNotification(string Subject, string notification)
        {
            dbuser.AdminNotifications.Add(new AdminNotification
            {
                CreatedOn = DateTime.Now,
                Deleted = false,
                Notification = notification,
                Subject = Subject
            });
            await dbuser.SaveChangesAsync();
            var key = $"AdminNotifications";
            await redisBL.DeleteKey(key);
            return true;
        }
        public async Task<bool> DeleteAdminNotification(int NotificationId)
        {
            dbuser.AdminNotifications.FirstOrDefault(x => x.Id == NotificationId).Deleted = true;
            await dbuser.SaveChangesAsync();
            var key = $"AdminNotifications";
            await redisBL.DeleteKey(key);
            return true;
        }
        public async Task<List<AdminNotification>> GetAdminNotifications()
        {
            var key = $"AdminNotifications";
            var redisValue = await redisBL.GetValue(key);
            if (redisValue != null)
                return JsonConvert.DeserializeObject<List<AdminNotification>>(redisValue);
            var _result = await dbuser.AdminNotifications.Where(x => !x.Deleted).OrderByDescending(x => x.CreatedOn).ToListAsync();
            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
            return _result;
        }
        public async Task<bool> MarkNotificationRead(int NotificationId, int UserId, bool Read)
        {
            var key = $"ReadNotifications_{UserId}";
            if (Read == true)
                dbuser.AdminNotificationUserReads.Add(new AdminNotificationUserRead { NotificationId = NotificationId, UserId = UserId });
            else
            {
                dbuser.AdminNotificationUserReads.RemoveRange(dbuser.AdminNotificationUserReads.Where(n => n.UserId == UserId && n.NotificationId == NotificationId));
            }
            await dbuser.SaveChangesAsync();
            await redisBL.DeleteKey(key);
            return true;
        }
        public async Task<List<int>> GetReadNotifications(int UserId)
        {
            var key = $"ReadNotifications_{UserId}";
            var redisValue = await redisBL.GetValue(key);
            if (redisValue != null)
                return JsonConvert.DeserializeObject<List<int>>(redisValue);
            var _result = await dbuser.AdminNotificationUserReads.Where(x => x.UserId == UserId).Select(x => x.NotificationId).ToListAsync();
            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
            return _result;
        }
        #endregion

        public async Task<CommonModel.FileDetails> DownloadUsers()
        {
            var username = GetKeyValue("report.username");
            var password = GetKeyValue("report.password");
            var reportserverurl = GetKeyValue("report.url");
            var credentials = new NetworkCredential(username, password);

            string url = $"{reportserverurl}?/users&rs:Format=Excel";

            using var httpClientHandler = new HttpClientHandler { Credentials = credentials };
            bool disposeHandler = true; //Setting true or false does not fix the problem
            using (var client = new System.Net.Http.HttpClient(httpClientHandler, disposeHandler))
            {
                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {

                        var stream = await result.Content.ReadAsByteArrayAsync();
                        string fileName = $"Users_{DateTime.Now}.xls";
                        var fileblob = await azureBL.UploadBlob(stream, fileName, true);
                        return new CommonModel.FileDetails
                        {
                            Blob = fileblob.Blob,
                            Url = azureBL.AzureImageUrl() + azureBL.AzureContainerReference() + "/" + fileblob
                        };
                    }
                    else
                    {
                        throw new Exception(result.ReasonPhrase);
                    }

                }
            }
        }
        public async Task<CommonModel.FileDetails> DownloadPandLReports(int UserId, DateTime? Date)
        {
            var username = GetKeyValue("report.username");
            var password = GetKeyValue("report.password");
            var reportserverurl = GetKeyValue("report.url");
            var credentials = new NetworkCredential(username, password);

            string url = $"{reportserverurl}?/PANDLReports&rs:Format=Excel&UserId={UserId}";
            if (Date.HasValue)
                url = $"{reportserverurl}?/PANDLReportsDateWise&rs:Format=Excel&UserId={UserId}&Date={Date.Value}";
            using var httpClientHandler = new HttpClientHandler { Credentials = credentials };
            bool disposeHandler = true; //Setting true or false does not fix the problem
            using (var client = new System.Net.Http.HttpClient(httpClientHandler, disposeHandler))
            {
                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {

                        var stream = await result.Content.ReadAsByteArrayAsync();
                        string fileName = $"PANDLReports_{DateTime.Now}.xls";
                        if (Date.HasValue)
                            url += $"&Date={Date.Value}";
                        var fileblob = await azureBL.UploadBlob(stream, fileName, true);
                        return new CommonModel.FileDetails
                        {
                            Blob = fileblob.Blob,
                            Url = azureBL.AzureImageUrl() + azureBL.AzureContainerReference() + "/" + fileblob
                        };
                    }
                    else
                    {
                        throw new Exception(result.ReasonPhrase);
                    }

                }
            }
        }
        public string GetKeyValue(string Key)
        {
            return dbuser.Configurations.FirstOrDefault(x => x.Key == Key)?.Value;
        }


        #region Alerts
        public async Task<bool> SaveAlert(string symbolType, string symbol, string alertFor, string condition, decimal value, int userId)
        {
            dbuser.Alerts.Add(new Alert
            {
                AlertFor = alertFor,
                Condition = condition,
                Value = value,
                CreatedOn = DateTime.Now,
                Deleted = false,
                Status = "active",
                Symbol = symbol,
                SymbolType = symbolType,
                UserId = userId
            });

            await dbuser.SaveChangesAsync();
            return true;
        }
        public async Task<List<Alert>> GetAlert(int UserId)
        {
            return await dbuser.Alerts.Where(x => x.UserId == UserId && !x.Deleted).ToListAsync();
        }
        public async Task<bool> DeleteAlert(int AlertId)
        {
            if (!dbuser.Alerts.Any(x => x.Id == AlertId)) return false;
            dbuser.Alerts.FirstOrDefault(x => x.Id == AlertId).Deleted = true;
            await dbuser.SaveChangesAsync();
            return true;
        }
        #endregion

        #region Trade
        public async Task<List<TradeModel.WatchlistDetails>> GetWatchlists(int UserId)
        {

            return await (from w in dbuser.Watchlists
                          where w.UserId == UserId
                          select new TradeModel.WatchlistDetails
                          {
                              Name = w.Name,
                              WatchLists = dbuser.SubWatchlists.Where(x => x.WatchListId == w.Id).Select(sw => new TradeModel.SubWatchlistDetails
                              {
                                  ATP = sw.Atp,
                                  Change = sw.Change,
                                  Id = sw.Id,
                                  ChangePercentage = sw.ChangePercentage,
                                  High = sw.High,
                                  LastUpdatedTime = sw.LastUpdatedTime,
                                  Low = sw.Low,
                                  LTP = sw.Ltp,
                                  OIChange = sw.Oichange,
                                  OIChangePercentage = sw.OiChangePercentage,
                                  Open = sw.Open,
                                  PreviousClose = sw.PreviousClose,
                                  PreviousOIClose = sw.PreviousOiclose,
                                  Symbol = sw.Symbol,
                                  SymbolId = sw.SymbolId,
                                  TickVolume = sw.TickVolume,
                                  TodayOI = sw.TodayOi,
                                  TotalVolume = sw.TotalVolume,
                                  TurnOver = sw.TurnOver,
                                  DisplayOrder = sw.DisplayOrder
                              }).ToList()
                          }
             ).ToListAsync();


        }
        public async Task<bool> DeleteSubWatchList(int SubwatchlistId)
        {
            dbuser.SubWatchlists.RemoveRange((dbuser.SubWatchlists.Where(x => x.Id == SubwatchlistId)));
            await dbuser.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SaveWatchList(List<TradeModel.WatchlistDetails> watchlist, int userId)
        {
            try
            {
                dbuser.SubWatchlists.RemoveRange(dbuser.SubWatchlists.Where(s => dbuser.Watchlists.Where(c => c.UserId == userId).Select(w => w.Id).ToArray().Contains(s.WatchListId)));
                dbuser.Watchlists.RemoveRange(dbuser.Watchlists.Where(c => c.UserId == userId));

                foreach (var w in watchlist)
                {
                    var _watchlist = new Watchlist
                    {
                        Name = w.Name,
                        UserId = userId
                    };
                    dbuser.Watchlists.Add(_watchlist);
                    await dbuser.SaveChangesAsync();

                    var _subwatchlists = new List<SubWatchlist>();
                    w.WatchLists.ForEach(x =>
                    {
                        _subwatchlists.Add(new SubWatchlist
                        {
                            Atp = x.ATP,
                            Change = x.Change,
                            ChangePercentage = x.ChangePercentage,
                            High = x.High,
                            LastUpdatedTime = DateTime.Now,
                            Low = x.Low,
                            Ltp = x.LTP,
                            Oichange = x.OIChange,
                            OiChangePercentage = x.OIChangePercentage,
                            Open = x.Open,
                            PreviousClose = x.PreviousClose,
                            PreviousOiclose = x.PreviousOIClose,
                            Symbol = x.Symbol,
                            SymbolId = x.SymbolId,
                            TickVolume = x.TickVolume,
                            TodayOi = x.TodayOI,
                            TotalVolume = x.TotalVolume,
                            TurnOver = x.TurnOver,
                            WatchListId = _watchlist.Id,
                            DisplayOrder = x.DisplayOrder
                        });
                    });
                    dbuser.SubWatchlists.AddRange(_subwatchlists);
                    await dbuser.SaveChangesAsync();

                };

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> SaveBasketOrder(List<TradeModel.TradeOrderRequestDetails> request, int UserId)
        {
            var orders = new List<TradeOrder>();
            request.ForEach(x =>
            {
                var trade = dbuser.BasketOrders.FirstOrDefault(t => t.Id == x.Id) ?? new BasketOrder();
                trade.OperationType = x.OperationType;
                trade.OrderType = x.OrderType;
                trade.Price = x.Price;
                trade.Quantity = x.Quantity;
                trade.RateType = x.RateType;
                trade.Status = x.Status;
                trade.Symbol = x.Symbol;
                trade.TriggerPrice = x.TriggerPrice;
                trade.Time = DateTime.Now;
                trade.UserId = UserId;
                trade.BasketId = x.BasketId;
                trade.Status = "basket";
                trade.Expiry = x.Expiry;
                trade.Strategy = x.Strategy;
                trade.Strike = x.Strike;
                if (trade.Id == 0)
                {
                    dbuser.BasketOrders.Add(trade);
                }

            });
            await dbuser.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SaveTradeOrder(List<TradeModel.TradeOrderRequestDetails> request, int UserId)
        {
            var orders = new List<TradeOrder>();
            request.ForEach(x =>
            {
                var trade = dbuser.TradeOrders.FirstOrDefault(t => t.Id == x.Id) ?? new TradeOrder();
                trade.OperationType = x.OperationType;
                trade.OrderType = x.OrderType;
                trade.Price = x.Price;
                trade.Strategy = x.Strategy;
                trade.Quantity = x.Quantity;
                trade.RateType = x.RateType;
                trade.Status = x.RateType.ToLower() == "market" ? "executed" : x.Status;
                trade.Symbol = x.Symbol;
                trade.TriggerPrice = x.TriggerPrice;
                trade.Time = DateTime.Now;
                if (x.RateType.ToLower() == "market")
                    trade.ExecutionTime = DateTime.Now;
                trade.UserId = UserId;
                trade.Guid = x.guid;
                trade.BasketId = x.BasketId;
                trade.Expiry = x.Expiry;
                trade.Boguid = x.boguid;
                trade.TargetPrice = x.TargetPrice;
                trade.Strike = x.Strike;
                trade.StopLoss = x.StopLoss;
                if (!string.IsNullOrEmpty(trade.Exchange))
                    trade.Exchange = x.Exchange;
                if (!string.IsNullOrEmpty(trade.InstrumentType))
                    trade.InstrumentType = x.InstrumentType;
                if (trade.Id == 0)
                {
                    dbuser.TradeOrders.Add(trade);
                }

            });
            await dbuser.SaveChangesAsync();

            try
            {
                var requestUrl = $"https://market.i4option.com/add-trade";
                HttpClientHandler handler = new();
                System.Net.Http.HttpClient httpClient = new(handler);
                var trades = dbuser.TradeOrders.Where(x => x.UserId == UserId && x.Status != "executed" && x.Time.Date == DateTime.Now.Date)
                    .Select(trade => new
                    {
                        id = trade.Id,
                        strategy = trade.Strategy,
                        symbol = trade.Symbol,
                        status = trade.Status,
                        operationType = trade.OperationType,
                        orderType = trade.OrderType,
                        quantity = trade.Quantity,
                        price = trade.Price,
                        triggerPrice = trade.TriggerPrice,
                        userId = trade.UserId,
                        rateType = trade.RateType,
                        strike = trade.Strike
                    }).ToList();
                if (trades.Count > 0)
                {
                    var body = JsonConvert.SerializeObject(trades);
                    HttpContent c = new StringContent(body, Encoding.UTF8, "application/json");
                    var result = await httpClient.PostAsync(requestUrl, c);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }
        public async Task EditOrder(TradeModel.IEditOrderRequest request)
        {
            var trade = dbuser.TradeOrders.FirstOrDefault(x => x.Guid == request.Guid);
            trade.Price = request.Price;
            if (request.TriggerPrice.HasValue)
                trade.TriggerPrice = request.TriggerPrice.Value;
            await dbuser.SaveChangesAsync();

        }
        public async Task PlaceBOOrder(List<BOOrderRequest> request)
        {

            request.ForEach(order =>
            {
                var trade = dbuser.TradeOrders.FirstOrDefault(t => t.Guid == order.Guid);
                dbuser.TradeOrders.Add(new TradeOrder
                {
                    Guid = Guid.NewGuid().ToString(),
                    OperationType = trade.OperationType.ToLower().Equals("buy") ? "sell" : "buy",
                    OrderType = trade.OrderType,
                    Quantity = trade.Quantity,
                    Price = order.Price,
                    RateType = trade.RateType,
                    Symbol = trade.Symbol,
                    Expiry = trade.Expiry,
                    Time = DateTime.Now,
                    TriggerPrice = trade.TriggerPrice,
                    Status = "executed",
                    UserId = trade.UserId,
                    Boguid = trade.Boguid
                });
            });
            await db.SaveChangesAsync();
        }
        public async Task<List<TradeModel.TradeOrderDetails>> GetTradeOrders(int UserId)
        {
            return await dbuser.TradeOrders.Where(x => x.UserId == UserId && ((x.Time.Date == DateTime.Now.Date && (x.OrderType.ToLower().Equals("mis") || x.OrderType.ToLower().Equals("bo")))
            || (x.OrderType.ToLower().Equals("normal") && ((x.Expiry.HasValue && x.Expiry.Value.Date >= DateTime.Now.Date))))).Select(x => new TradeModel.TradeOrderDetails
            {
                Id = x.Id,
                OperationType = x.OperationType,
                OrderType = x.OrderType,
                Price = x.Price,
                Quantity = x.Quantity,
                RateType = x.RateType,
                Status = x.Status,
                Symbol = x.Symbol,
                guid = x.Guid,
                Time = x.Time,
                TriggerPrice = x.TriggerPrice,
                BasketId = x.BasketId,
                Expiry = x.Expiry,
                Strategy = x.Strategy,
                Strike = x.Strike,
                Exchange = x.Exchange,
                InstrumentType = x.InstrumentType,
                ExecutionTime = x.ExecutionTime,
            }).ToListAsync();
        }
        public async Task<List<TradeModel.TradeOrderDetails>> GetAllTradeOrders(int? UserId, DateTime? date)
        {
            return await dbuser.TradeOrders
                .Where(x => (UserId.HasValue ? x.UserId == UserId : true) && (!date.HasValue || x.Time.Date == date.Value.Date))
                .Include(x => x.User)
                .Select(x => new TradeModel.TradeOrderDetails
                {
                    Id = x.Id,
                    OperationType = x.OperationType,
                    OrderType = x.OrderType,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    RateType = x.RateType,
                    Status = x.Status,
                    Symbol = x.Symbol,
                    guid = x.Guid,
                    Time = x.Time,
                    TriggerPrice = x.TriggerPrice,
                    Expiry = x.Expiry,
                    Strategy = x.Strategy,
                    Strike = x.Strike,
                    UserId = x.UserId,
                    WalletBalance = x.User.WalletBalance,
                    Exchange = x.Exchange,
                    InstrumentType = x.InstrumentType,
                    ExecutionTime = x.ExecutionTime,
                }).ToListAsync();
        }
        public async Task<List<TradeModel.TradeOrderDetails>> GetTodayTrades()
        {

            var _result = new List<CommonModel.SymbolDetails>();
            var key = "AllSymbols";
            var redisValue = await redisBL.GetValue(key);
            if (redisValue != null)
            { _result = JsonConvert.DeserializeObject<List<CommonModel.SymbolDetails>>(redisValue); }
            else
            {
                _result = await commonBL.GetSymbols();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
            }

            //return await (from x in dbuser.TradeOrders join
            //  s in db.Symbols on x.Symbol equals s.Symbol1
            // where x.Time.Date == DateTime.Now.Date && x.Status == "open"
            //  select new TradeModel.TradeOrderDetails
            //  {
            //      Id = x.Id,
            //      OperationType = x.OperationType,
            //      OrderType = x.OrderType,
            //      Price = x.Price,
            //      Quantity = x.Quantity,
            //      RateType = x.RateType,
            //      Status = x.Status,
            //      Symbol = x.Symbol,
            //      SymbolId = s.SymbolId,
            //      guid = x.Guid,
            //      Time = x.Time,
            //      TriggerPrice = x.TriggerPrice
            //  }).ToListAsync();

            return await dbuser.TradeOrders.Where(x => x.Time.Date == DateTime.Now.Date && x.Status == "open").Select(x => new TradeModel.TradeOrderDetails
            {
                Id = x.Id,
                OperationType = x.OperationType,
                OrderType = x.OrderType,
                Price = x.Price,
                Quantity = x.Quantity,
                RateType = x.RateType,
                Status = x.Status,
                Symbol = x.Symbol,
                SymbolId = redisBL.GetValue(x.Symbol).Result,
                guid = x.Guid,
                UserId = x.UserId,
                Time = x.Time,
                TriggerPrice = x.TriggerPrice,
                Expiry = x.Expiry,
                Strategy = x.Strategy,
                Strike = x.Strike,
                Exchange = x.Exchange,
                InstrumentType = x.InstrumentType,
                ExecutionTime = x.ExecutionTime,
            }).ToListAsync();
        }
        public async Task<bool> DeleteTrade(int TradeId)
        {
            //dbuser.TradeOrders.RemoveRange((dbuser.TradeOrders.Where(x => x.Id == TradeId)));
            foreach (var item in dbuser.TradeOrders.Where(x => x.Id == TradeId))
            {
                item.Status = "cancelled";
            }
            await dbuser.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteBasketOrder(int id)
        {
            dbuser.BasketOrders.RemoveRange((dbuser.BasketOrders.Where(x => x.Id == id)));
            await dbuser.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SavePositions(TradeModel.PositionsRequest request, bool clearPrevious)
        {
            if (clearPrevious)
                dbuser.Positions.RemoveRange(dbuser.Positions.Where(x => x.UserId == request.UserId && x.UpdatedOn.Date == DateTime.Now.Date));
            dbuser.Positions.AddRange(request.Positions.Select(x => new Position
            {
                SellAvg = x.SellAvg,
                BuyQuantity = x.BuyQuantity,
                BuyAvg = x.BuyAvg,
                SellQuantity = x.SellQuantity,
                Ltp = x.Ltp,
                OrderType = x.OrderType,
                PandL = x.PandL,
                Symbol = x.Symbol,
                UpdatedOn = DateTime.Now,
                UserId = x.UserId,
                Exchange = x.Exchange,
                InstrumentType = x.InstrumentType
            }));
            //var orders = new List<Position>();
            //request.Positions.ForEach(x =>
            //{
            //    var position = dbuser.Positions.FirstOrDefault(t => t.Id == x.Id) ?? new Position();
            //    position.Ltp = x.Ltp;
            //    position.Quantity = x.Quantity;
            //    position.Avg = x.Avg;
            //    position.PandL = x.PandL;
            //    position.OrderType = x.OrderType;
            //    position.Symbol = x.Symbol;
            //    position.UserId = request.UserId;
            //    position.UpdatedOn = DateTime.Now;
            //    if (position.Id == 0)
            //    {
            //        dbuser.Positions.Add(position);
            //    }

            //});
            await dbuser.SaveChangesAsync();
            return true;
        }

        public async Task<List<Position>> GetPositions(int UserId, DateTime? date)
        {
            return await dbuser.Positions.Where(x => x.UserId == UserId && (!date.HasValue || x.UpdatedOn.Date == date.Value.Date))
                .OrderBy(x => x.UpdatedOn).ToListAsync();
        }
        public decimal GetWalletBalance(int UserId)
        {
            try
            {

                var totalCredits = dbuser.Wallets.Where(x => x.UserId == UserId).Sum(x => x.Amount);
                var totalDebits = dbuser.Positions.Where(x => x.UserId == UserId).Sum(x => x.PandL);

                return totalCredits + totalDebits;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddFundsToWallet(int UserId, decimal Amount)
        {
            dbuser.Wallets.Add(new Wallet
            {
                Amount = Amount,
                CreatedOn = DateTime.Now,
                UserId = UserId
            });
            await dbuser.SaveChangesAsync();
            dbuser.Database.ExecuteSqlInterpolated($"EXEC sProc_UpdateWallet");

        }

        public async Task<List<SocialTradingUserDetails>> GetSocialTradingToppers(string sortBy, string userSearch, int? userId)
        {
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);
            userSearch = userSearch?.ToLower();
            var results = from p in dbuser.Positions
                          where p.UpdatedOn.Month == DateTime.Now.Month && p.UpdatedOn.Year == DateTime.Now.Year
                          group p.PandL by p.UserId into g
                          select new { UserId = g.Key, PandL = g.Sum() };
            if (!string.IsNullOrEmpty(userSearch))
            {

                results = from p in dbuser.Positions
                          join u in dbuser.Users on p.UserId equals u.Id
                          where u.FirstName.ToLower().Contains(userSearch) || u.LastName.ToLower().Contains(userSearch)
                          group p.PandL by p.UserId into g
                          select new { UserId = g.Key, PandL = g.Sum() };
            }
            else if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy == "last-day")
                    results = from p in dbuser.Positions
                              where p.UpdatedOn.Date == DateTime.Now.AddDays(-1).Date
                              group p.PandL by p.UserId into g
                              select new { UserId = g.Key, PandL = g.Sum() };
                else if (sortBy == "last-month")
                    results = from p in dbuser.Positions
                              where p.UpdatedOn.Date >= first.Date
                              && p.UpdatedOn.Date <= last
                              group p.PandL by p.UserId into g
                              select new { UserId = g.Key, PandL = g.Sum() };
                else if (sortBy == "my-group")
                {
                    var followers = await GetFollows(userId.Value);
                    results = from p in dbuser.Positions
                              where followers.Contains(p.UserId)
                              group p.PandL by p.UserId into g
                              select new { UserId = g.Key, PandL = g.Sum() };
                }

            }
            var res = (from u in dbuser.Users
                       join r in results on u.Id equals r.UserId
                       where u.Id != 10104 && u.Id != 49496
                       select new SocialTradingUserDetails
                       {
                           FirstName = u.FirstName,
                           LastName = u.LastName,
                           SocialProfileName = u.SocialProfileName,
                           UserId = u.Id,
                           PandL = r.PandL,
                           NoOfTrades = dbuser.Positions.Count(x => x.UserId == u.Id),
                           AveragePandL = r.PandL / dbuser.Positions.Count(x => x.UserId == u.Id),
                           Accuracy = dbuser.Positions.Count(x => x.UserId == u.Id && x.PandL > 0) * 100 / dbuser.Positions.Count(x => x.UserId == u.Id)
                       }).ToList();

            if (sortBy == "profit")
                return res.OrderByDescending(x => x.PandL).Take(20).ToList();
            else if (sortBy == "average-profit")
                return res.OrderByDescending(x => x.AveragePandL).Take(20).ToList();
            else if (sortBy == "accuracy")
                return res.OrderByDescending(x => x.Accuracy).Take(20).ToList();
            else if (sortBy == "consistency")
                return res.OrderByDescending(x => x.Accuracy).Take(20).ToList();
            else if (sortBy == "last-day")
                return res.OrderByDescending(x => x.PandL).Take(20).ToList();
            else if (sortBy == "last-month")
                return res.OrderByDescending(x => x.PandL).Take(20).ToList();
            else if (sortBy == "my-group")
                return res.OrderByDescending(x => x.PandL).ToList();
            return res;
            //return results.OrderByDescending(x => x.PandL).Take(10).Select(x=> new User{}).ToList();
        }

        public async Task<bool> BulkExecuteTradeOrders(List<BulkTradeOrdersRequest> TradeOrderIds)
        {
            var boorders = TradeOrderIds.Where(x => x.orderType?.ToLower() == "bo").Select(x => new BOOrderRequest
            {
                Guid = x.guid,
                Price = x.Price
            }).ToList();
            if (boorders.Count > 0)
                await PlaceBOOrder(boorders);

            TradeOrderIds.ToList().ForEach((tradeOrderId) =>
            {
                dbuser.TradeOrders.Where(t => t.Guid == tradeOrderId.guid && t.Status != "cancelled").ToList().ForEach(t =>
                {
                    if (t.OrderType.ToLower() == "bo")
                    {
                        t.Price = t.Price <= t.TriggerPrice ? t.Price - t.StopLoss.Value : t.Price + t.TargetPrice.Value;
                    }
                    else
                    {
                        t.Price = tradeOrderId.Price;
                    }
                    t.Status = "executed";
                    t.ExecutionTime = DateTime.Now;
                    //users.Add(t.UserId);
                });
            });
            await dbuser.SaveChangesAsync();

            //var users = dbuser.TradeOrders.Where(t => TradeOrderIds.Contains(t.Id)).Select(x => x.UserId).Distinct().ToList();

            //await BulkUpdatePositions(users);
            return true;
        }
        public async Task<bool> BulkUpdatePositionsForDay(DateTime date)
        {
            var userids = dbuser.Positions.Where(p => p.UpdatedOn.Date == date.Date).Select(x => x.UserId).Distinct().ToList();
            await BulkUpdatePositions(userids);
            return true;
        }
        public async Task<List<int>> BulkUpdatePositions(List<int> UserIds)
        {
            var todayTime = DateTime.Now.Date;
            foreach (var t in dbuser.TradeOrders.Where(x => UserIds.Contains(x.UserId) && x.Time.Date == todayTime && x.Status.ToLower() == "executed").Select(x => new { x.Symbol, x.UserId }).Distinct().ToList())
            {
                //dbuser.RemoveRange(dbuser.Positions.Where(p => p.UpdatedOn.Date == todayTime.Date));
                dbuser.Database.ExecuteSqlInterpolated($"DELETE FROM Positions WHERE CONVERT(varchar,UpdatedOn,103)=CONVERT(varchar,GETDATE(),103) AND UserId={t.UserId}");

                var trade = dbuser.TradeOrders.FirstOrDefault(x => x.Quantity != 0 && x.Time.Date == todayTime && x.Symbol == t.Symbol && x.UserId == t.UserId);
                if (trade != null)
                {
                    var allUserTrades = dbuser.TradeOrders.Where(x => x.Quantity != 0 && x.Time.Date == todayTime && x.Symbol == t.Symbol && x.UserId == trade.UserId);
                    var totalSell = allUserTrades.Where(x => x.OperationType == "sell").Sum(x => x.Quantity);
                    var totalSellAmount = allUserTrades.Where(x => x.OperationType == "sell").Sum(x => x.Quantity * x.Price);

                    var totalBuy = allUserTrades.Where(x => x.OperationType == "buy").Sum(x => x.Quantity);
                    var totalBuyAmount = allUserTrades.Where(x => x.OperationType == "buy").Sum(x => x.Quantity * x.Price);

                    var totalqty = totalBuy - totalSell;
                    var totalAmount = totalBuyAmount - totalSellAmount;

                    var average = totalqty == 0 ? 0 : (totalAmount / totalqty);


                    decimal? pnl = -1 * totalAmount;
                    if (totalqty != 0)
                    {
                        var symbols = new List<string>() { trade.Symbol };
                        var tline = await commonBL.GetTouchline(symbols);
                        pnl = (tline.FirstOrDefault().Ltp - average) * totalqty;
                    }
                    //if (!pnl.HasValue) pnl = trade.Price;
                    dbuser.Positions.Add(new Position
                    {
                        UserId = t.UserId,
                        BuyAvg = totalBuy == 0 ? 0 : (totalBuyAmount / totalBuy),
                        SellAvg = totalSell == 0 ? 0 : (totalSellAmount / totalSell),
                        SellQuantity = totalSell,
                        Ltp = trade.Price,
                        BuyQuantity = totalBuy,
                        Symbol = trade.Symbol,
                        OrderType = trade.OrderType,
                        UpdatedOn = todayTime,
                        PandL = pnl.Value,
                        Quantity = totalqty
                    });

                }
            };
            await dbuser.SaveChangesAsync();
            return UserIds;
        }

        public async Task<int> CreateBasket(string Name, int UserId)
        {
            var basket = new Basket
            {
                Name = Name,
                UserId = UserId,
                Status = "pending"
            };
            dbuser.Baskets.Add(basket);
            await dbuser.SaveChangesAsync();

            return basket.Id;
        }
        public async Task DeleteBasket(int basketId)
        {
            var basket = dbuser.Baskets.FirstOrDefault(b => b.Id == basketId);
            if (basket == null) throw new Exception("Basket Not Found");
            dbuser.Baskets.Remove(basket);
            await dbuser.SaveChangesAsync();
        }
        public async Task<List<Basket>> GetBaskets(int UserId)
        {
            return await dbuser.Baskets.Where(b => b.UserId == UserId).ToListAsync();
        }
        public List<BasketDetails> GetBasketOrders(int userId)
        {
            return dbuser.Baskets.Where(b => b.UserId == userId).Select(b =>
            new BasketDetails
            {
                Id = b.Id,
                Name = b.Name,
                Orders = b.BasketOrders.Where(x => x.Strategy != null && x.Strike.HasValue).Select(o => new BasketOrderDetails
                {
                    Id = o.Id,
                    BasketId = b.Id,
                    OperationType = o.OperationType,
                    OrderType = o.OrderType,
                    Price = o.Price,
                    Quantity = o.Quantity,
                    RateType = o.RateType,
                    Symbol = o.Symbol,
                    Strike = o.Strike,
                    Strategy = o.Strategy,
                    Expiry = o.Expiry
                }).ToList()
            }).ToList();
        }
        public async Task<List<Wallet>> GetLedger(int userId)
        {
            return await dbuser.Wallets.Where(x => x.UserId == userId).ToListAsync();
        }
        public class UserWithBrokerage
        {
            public int UserId { get; set; }
            public List<UserWithBrokeragePlan> Plans { get; set; }
        }
        public class UserWithBrokeragePlan
        {
            public int StrategyId { get; set; }
            public decimal SellMargin { get; set; }
            public decimal BuyMargin { get; set; }
            public decimal SellBrokerage { get; set; }
            public decimal BuyBrokerage { get; set; }
        }
        private List<UserWithBrokerage> UsersWithBrokerages()
        {
            var value = cacheBL.GetValue("brokeragePlans");
            if (value != null)
            {
                return JsonConvert.DeserializeObject<List<UserWithBrokerage>>(value);
            }
            var _brokeragePlans = dbuser.Users.Select(x => new UserWithBrokerage
            {
                UserId = x.Id,
                Plans = dbuser.BrokeragePlans.Select(p => new UserWithBrokeragePlan
                {
                    StrategyId = p.StrategyId,
                    SellMargin = p.SellMargin,
                    BuyMargin = p.BuyMargin,
                    SellBrokerage = p.SellBrokerage
                }).ToList()
            }).ToList();


            cacheBL.SetValue("brokeragePlans", JsonConvert.SerializeObject(_brokeragePlans), null);
            return _brokeragePlans;
        }
        public decimal GetMargin(List<CalculateMarginRequest> request)
        {
            //var userid = request.FirstOrDefault().UserId;
            //var brokerageplans = UsersWithBrokerages().FirstOrDefault(x => x.UserId == userid).Plans;

            //if (brokerageplans.Count == 0)
            //    brokerageplans.AddRange(dbuser.BrokeragePlans.Where(x => x.Name == "Default").Select(x => new UserWithBrokeragePlan
            //    {
            //        BuyBrokerage = x.BuyBrokerage,
            //        BuyMargin = x.BuyMargin,
            //        SellBrokerage = x.SellBrokerage,
            //        SellMargin = x.SellMargin,
            //        StrategyId = x.StrategyId
            //    }));

            //var brokerageplans = dbuser.Users.Where(u => u.Id == userid).Include(u => u.Plans).FirstOrDefault().Plans;
            decimal margin = 0;

            foreach (var item in request.Where(x => x.Quantity != 0))
            {

                if (item.Strategy.ToLower().Equals("options"))
                {
                    if (item.Symbol.EndsWith("CE") || item.Symbol.EndsWith("PE"))
                    {
                        if (item.TransactionType.ToUpper() == "BUY")
                            margin += item.Quantity / item.LotSize * item.Price;
                        else
                            margin += item.Quantity * 10000 / item.LotSize;
                    }
                    else if (item.Symbol.EndsWith("FUT"))
                    {
                        margin += item.Quantity * 100000;
                    }

                }
                else if (item.Strategy.ToLower().Equals("straddle"))
                {
                    if (item.TransactionType.ToUpper() == "BUY")
                        margin += item.Quantity / item.LotSize * 5000;
                    // margin += item.Quantity * brokerageplans.Where(x => x.StrategyId == 1).FirstOrDefault().BuyMargin; ;
                    if (item.TransactionType.ToUpper() == "SELL")
                        margin += item.Quantity / item.LotSize * 10000;
                    // margin += item.Quantity * brokerageplans.Where(x => x.StrategyId == 1).FirstOrDefault().SellMargin; ;
                }
                //else if (item.Strategy.ToLower().Equals("ironfly"))
                //{
                //    if (item.TransactionType.ToUpper() == "BUY")
                //        margin += item.Quantity * 5000;
                //    if (item.TransactionType.ToUpper() == "SELL")
                //        margin += item.Quantity * 5000;
                //}
                //else if (item.Strategy.ToLower().Equals("stocks"))
                //{
                //    if (item.TransactionType.ToUpper() == "BUY")
                //        margin += item.Price * item.Quantity * item.LotSize;
                //    if (item.TransactionType.ToUpper() == "SELL")
                //        margin += 1000000000000;
                //}

            }
            return margin;
        }
        #endregion


        #region User Payment Plans
        public async Task<bool> SavePlan(PaymentPlanRequest request)
        {
            var plan = dbuser.Plans.FirstOrDefault(x => x.Id == request.Id) ?? new Plan();
            plan.Name = request.Name;
            plan.Amount = request.Amount;
            plan.Days = request.Days;
            plan.Remarks = request.Remarks;
            plan.UpdatedOnUtc = DateTime.Now;
            if (!string.IsNullOrEmpty(request.Domain))
                plan.Domain = request.Domain;
            if (plan.Id == 0)
            {
                plan.CreatedOnUtc = DateTime.Now;
                dbuser.Plans.Add(plan);
            }
            await dbuser.SaveChangesAsync();
            var key = $"PaymentPlans";
            await redisBL.DeleteKey(key);
            return true;
        }
        public async Task<bool> DeletePlan(int PlanId)
        {
            dbuser.Plans.RemoveRange(dbuser.Plans.Where(x => x.Id == PlanId));
            await dbuser.SaveChangesAsync();
            var key = $"PaymentPlans";
            await redisBL.DeleteKey(key);
            return true;
        }
        public async Task<List<Plan>> GetPaymentPlans()
        {
            var _result = await dbuser.Plans.ToListAsync();
            return _result;
        }
        #endregion

        #region Social Trade
        public async Task<bool> Follow(int UserId, int FollowerId)
        {
            if (!dbuser.Followers.Any(x => x.UserId == UserId && x.FollowerId == FollowerId))
            {
                dbuser.Followers.Add(new Follower
                {
                    FollowerId = FollowerId,
                    UserId = UserId
                });
                await dbuser.SaveChangesAsync();

                try
                {
                    var requestUrl = $"https://market.i4option.com/followers";
                    HttpClientHandler handler = new();
                    System.Net.Http.HttpClient httpClient = new(handler);
                    HttpContent c = new StringContent(JsonConvert.SerializeObject(new { data = "updateme" }), Encoding.UTF8, "application/json");
                    var result = await httpClient.PostAsync(requestUrl, c);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return true;
        }
        public async Task<bool> UnFollow(int UserId, int FollowerId)
        {
            dbuser.Followers.RemoveRange(dbuser.Followers.Where(x => x.UserId == UserId && x.FollowerId == FollowerId));
            await dbuser.SaveChangesAsync();
            try
            {
                var requestUrl = $"https://market.i4option.com/followers";
                HttpClientHandler handler = new();
                System.Net.Http.HttpClient httpClient = new(handler);
                HttpContent c = new StringContent(JsonConvert.SerializeObject(new { data = "updateme" }), Encoding.UTF8, "application/json");
                var result = await httpClient.PostAsync(requestUrl, c);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }
        public async Task<List<int>> GetFollows(int UserId)
        {
            //user id is the person which is being followed and follower is id of the person who is following
            return await dbuser.Followers.Where(x => x.FollowerId == UserId).Select(x => x.UserId).ToListAsync();
        }
        public async Task<List<TradeOrder>> GetFollowerTrades(int UserId)
        {
            //user id is the person which is being followed and follower is id of the person who is following
            return await (from t in dbuser.TradeOrders
                          join f in dbuser.Followers on t.UserId equals f.UserId
                          where f.FollowerId == UserId && t.Time.Date == DateTime.Now.Date && t.Status != "basket"
                          select
             t).OrderByDescending(x => x.Time).ToListAsync();
            //return await dbuser.Followers.Where(x => x.FollowerId == UserId).Select(x => x.UserId).ToListAsync();
        }
        public async Task<List<Follower>> GetFollowers()
        {
            return await dbuser.Followers.ToListAsync();
        }
        public TradeAnalysisDetails GetTradeAnalysisDetails(int UserId)
        {
            var custom = dbuser.Positions.Where(t => t.UserId == UserId).Select(t => new
            {
                t.Symbol,
                t.Id,
                Time = t.UpdatedOn,
                t.PandL,
                BuyAvg = t.BuyAvg.Value,
                t.BuyQuantity,
                t.SellQuantity,
                SellAvg = t.SellAvg.Value
                //BuyAmount = t.OperationType == "buy" ? t.Price * t.Quantity : 0,
                //SellAmount = t.OperationType == "sell" ? t.Price * t.Quantity : 0,
                //PANDL = (t.OperationType == "buy" ? t.Price * t.Quantity : 0) - (t.OperationType == "sell" ? t.Price * t.Quantity : 0)
            });
            if (custom.Count() == 0) return new TradeAnalysisDetails();

            var pePandLProfit = custom.Where(x => x.Symbol.EndsWith("PE") && x.PandL > 0).Sum(t => t.PandL);
            var pePandLLoss = custom.Where(x => x.Symbol.EndsWith("PE") && x.PandL < 0).Sum(t => t.PandL);
            var cePandLProfit = custom.Where(x => x.Symbol.EndsWith("CE") && x.PandL > 0).Sum(t => t.PandL);
            var cePandLLoss = custom.Where(x => x.Symbol.EndsWith("CE") && x.PandL < 0).Sum(t => t.PandL);
            var futPandLProfit = custom.Where(x => x.Symbol.EndsWith("FUT") && x.PandL > 0).Sum(t => t.PandL);
            var futPandLLoss = custom.Where(x => x.Symbol.EndsWith("FUT") && x.PandL < 0).Sum(t => t.PandL);
            var stocksPandLProfit = custom.Where(x => !(x.Symbol.EndsWith("PE") || x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("FUT")) && x.PandL > 0).Sum(t => t.PandL);
            var stocksPandLLoss = custom.Where(x => !(x.Symbol.EndsWith("PE") || x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("FUT")) && x.PandL < 0).Sum(t => t.PandL);

            var profits = new List<PANDLCategory>() {
                new PANDLCategory
            {
               Category= "CE",
               Profit = cePandLProfit,
               Loss = cePandLLoss
            },new PANDLCategory
            {
               Category= "PE",
               Profit = pePandLProfit,
               Loss = pePandLLoss
            },new PANDLCategory
            {
               Category= "FUT",
               Profit = futPandLProfit,
                Loss = futPandLLoss
            },new PANDLCategory
            {
               Category= "Stocks",
               Profit = stocksPandLProfit,
               Loss = stocksPandLLoss
            }

            };



            var res = JsonConvert.SerializeObject(custom.ToList());
            var tradeAnalysisDetails = new TradeAnalysisDetails();
            tradeAnalysisDetails.Amount = new TradeAnalysisProfitLoss
            {
                Profit = custom.Where(x => x.PandL > 0).Sum(x => x.PandL),
                Loss = custom.Where(x => x.PandL < 0).Sum(x => x.PandL),
                Net = custom.Sum(x => x.PandL)
            };

            tradeAnalysisDetails.Accuracy = new TradeAnalysisProfitLoss
            {
                Profit = custom.Count() == 0 ? 0 : custom.Where(x => x.PandL > 0).Count() * 100 / custom.Count(),
                Loss = custom.Count() == 0 ? 0 : custom.Where(x => x.PandL < 0).Count() * 100 / custom.Count(),
            };

            tradeAnalysisDetails.NumberOfTrades = new TradeAnalysisProfitLoss
            {
                Profit = custom.Where(x => x.PandL > 0).Count(),
                Loss = custom.Where(x => x.PandL < 0).Count(),
                Net = custom.Count()
            };

            tradeAnalysisDetails.Average = new TradeAnalysisProfitLoss
            {
                Profit = custom.Where(x => x.PandL > 0).Count() == 0 ? 0 : custom.Where(x => x.PandL > 0).Sum(x => x.PandL) / custom.Where(x => x.PandL > 0).Count(),
                Loss = custom.Where(x => x.PandL < 0).Count() == 0 ? 0 : custom.Where(x => x.PandL < 0).Sum(x => x.PandL) / custom.Where(x => x.PandL < 0).Count(),
                Net = custom.Count() == 0 ? 0 : custom.Sum(x => x.PandL) / custom.Count()
            };

            tradeAnalysisDetails.AverageTradePerDay = new TradeAnalysisProfitLoss
            {
                Profit = custom.Where(x => x.PandL > 0).Select(x => x.Time.Date).Distinct().Count() == 0 ? 0 : custom.Where(t => custom.Where(x => x.PandL > 0).Select(x => x.Time.Date).Distinct().ToArray().Contains(t.Time.Date)).Sum(t => t.PandL) / custom.Where(x => x.PandL > 0).Select(x => x.Time.Date).Distinct().Count(),
                Loss = custom.Where(x => x.PandL < 0).Select(x => x.Time.Date).Distinct().Count() == 0 ? 0 : custom.Where(t => custom.Where(x => x.PandL < 0).Select(x => x.Time.Date).Distinct().ToArray().Contains(t.Time.Date)).Sum(t => t.PandL) / custom.Where(x => x.PandL < 0).Select(x => x.Time.Date).Distinct().Count(),
            };

            tradeAnalysisDetails.AmountOfDay = new TradeAnalysisProfitLoss
            {
                Profit = custom.Where(t => custom.Where(x => x.PandL > 0).Select(x => x.Time.Date).Distinct().ToArray().Contains(t.Time.Date)).Sum(t => t.PandL),
                Loss = custom.Where(t => custom.Where(x => x.PandL < 0).Select(x => x.Time.Date).Distinct().ToArray().Contains(t.Time.Date)).Sum(t => t.PandL),
            };

            tradeAnalysisDetails.MaxTradeAmount = new TradeAnalysisProfitLoss
            {
                Profit = custom.OrderByDescending(x => x.PandL).FirstOrDefault()?.PandL,
                Loss = custom.OrderBy(x => x.PandL).FirstOrDefault()?.PandL
            };


            tradeAnalysisDetails.MaxTradeDate = new TradeAnalysisProfitLossDate
            {
                Profit = custom.OrderByDescending(x => x.PandL).FirstOrDefault()?.Time,
                Loss = custom.OrderBy(x => x.PandL).FirstOrDefault()?.Time
            };

            tradeAnalysisDetails.NumberOfTradingDays = new TradeAnalysisProfitLoss
            {
                Profit = custom.Where(x => x.PandL > 0).Select(x => x.Time.Date).Distinct().Count(),
                Loss = custom.Where(x => x.PandL < 0).Select(x => x.Time.Date).Distinct().Count()
            };

            tradeAnalysisDetails.MaxDayAmount = new TradeAnalysisProfitLoss
            {
                Profit = custom.Max(x => x.PandL),
                Loss = custom.Min(x => x.PandL),
            };

            tradeAnalysisDetails.MaxDate = new TradeAnalysisProfitLossDate
            {
                Profit = custom.OrderByDescending(x => x.PandL).FirstOrDefault().Time.Date,
                Loss = custom.OrderBy(x => x.PandL).FirstOrDefault().Time.Date,
            };
            var topsymbolProfit = custom.OrderByDescending(x => x.PandL).FirstOrDefault().Symbol;
            var topsymbolLoss = custom.OrderBy(x => x.PandL).FirstOrDefault().Symbol;
            var _symbolProfit = topsymbolProfit.Remove(topsymbolProfit.IndexOf(new string(topsymbolProfit.SkipWhile(c => !char.IsDigit(c))
                          .TakeWhile(c => char.IsDigit(c))
                          .ToArray())));
            var _symbolLoss = topsymbolLoss.Remove(topsymbolLoss.IndexOf(new string(topsymbolLoss.SkipWhile(c => !char.IsDigit(c))
                          .TakeWhile(c => char.IsDigit(c))
                          .ToArray())));
            tradeAnalysisDetails.TopSymbol = new TradeAnalysisProfitLossString
            {
                Profit = db.Stocks.FirstOrDefault(x => x.Name == _symbolProfit)?.DisplayName,
                Loss = db.Stocks.FirstOrDefault(x => x.Name == _symbolLoss)?.DisplayName,
            };
            tradeAnalysisDetails.TopSymbolContribution = new TradeAnalysisProfitLoss
            {
                Profit = custom.Where(x => x.Symbol == topsymbolProfit).Sum(x => x.PandL),
                Loss = custom.Where(x => x.Symbol == topsymbolLoss).Sum(x => x.PandL)
            };

            tradeAnalysisDetails.ContributingDays_80 = new TradeAnalysisProfitLossCounter();
            tradeAnalysisDetails.ContributingDays_60 = new TradeAnalysisProfitLossCounter();
            tradeAnalysisDetails.ContributingDays_60 = new TradeAnalysisProfitLossCounter();
            tradeAnalysisDetails.ContributingDays_80 = new TradeAnalysisProfitLossCounter();
            tradeAnalysisDetails.TopAsset = new TradeAnalysisProfitLossString
            {
                Profit = profits.OrderByDescending(x => x.Profit).FirstOrDefault().Category,
                Loss = profits.OrderByDescending(x => x.Loss).FirstOrDefault().Category
            };

            tradeAnalysisDetails.TopAssetContribution = new TradeAnalysisProfitLoss
            {
                Profit = custom.Where(x => x.PandL > 0).Sum(x => x.PandL) == 0 ? 0 : profits.OrderByDescending(x => x.Profit).FirstOrDefault().Profit / custom.Where(x => x.PandL > 0).Sum(x => x.PandL),
                Loss = custom.Where(x => x.PandL < 0).Sum(x => x.PandL) == 0 ? 0 : profits.OrderByDescending(x => x.Loss).FirstOrDefault().Loss / custom.Where(x => x.PandL < 0).Sum(x => x.PandL)
            };

            tradeAnalysisDetails.TradeExpense = (decimal)((custom.Sum(x => x.BuyQuantity * x.BuyAvg) * .000041m) + (custom.Sum(x => x.SellAvg * x.SellQuantity) * .0001031m) + (dbuser.TradeOrders.Where(t => t.Status == "executed" && t.UserId == UserId).Count() * 23.6m));

            return tradeAnalysisDetails;
        }
        #endregion

        public async Task UpdateOINumberStatus(int userId, bool status)
        {
            var user = dbuser.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) throw new Exception("NOT_FOUND");
            user.ShowOinumbers = status;
            await dbuser.SaveChangesAsync();
        }


        static string EncryptNumber(int number)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(number.ToString()));

                // Take the first 6 characters from the hash as a simple example
                // You may want to adjust this based on your specific requirements
                string encryptedString = BitConverter.ToString(hashedBytes, 0, 3).Replace("-", "");

                // Ensure it's exactly 6 characters
                encryptedString = encryptedString.Substring(0, Math.Min(6, encryptedString.Length));

                return encryptedString;
            }
        }

    }
}
