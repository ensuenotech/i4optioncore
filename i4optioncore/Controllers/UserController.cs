using i4optioncore.Models;
using i4optioncore.Repositories;
using i4optioncore.Repositories.Dhan;
using i4optioncore.Services;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using KiteConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static i4optioncore.Models.TradeModel;
using static i4optioncore.Models.UserModel;

namespace i4optioncore.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserBL user;
        IRedisBL redis;
        private readonly IDhanBL dhanBL;
        private readonly IKiteBL kiteBL;
        private readonly IAuthService authService;

        public UserController(IUserBL _user, IKiteBL _kiteBL, IRedisBL redis, IDhanBL dhanBL, IAuthService authService)
        {
            user = _user;
            kiteBL = _kiteBL;
            this.redis = redis;
            this.dhanBL = dhanBL;
            this.authService = authService;
        }
        //[Route("GetUsers"), HttpGet]
        //public async Task<IActionResult> GetUsers()
        //{
        //    try
        //    {
        //        var _result = await user.GetAllUsers();
        //        return Ok(_result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}
        [Route("Ledger"), HttpPost]
        public async Task<IActionResult> GetLedger([FromBody] int userId)
        {
            try
            {
                var _result = await user.GetLedger(userId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetUserDetails"), HttpPost]

        public async Task<IActionResult> GetUserDetails([FromBody] int id)
        {
            try
            {
                var _result = await user.GetUserDetails(id);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("DeleteUser"), HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] int id)
        {
            try
            {
                var _result = await user.DeleteUser(id);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UserNames"), HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserNames()
        {
            try
            {
                var _result = new List<UserNames>();
                var key = "UserNames";
                var time = DateTime.Now; //10000000 = 1 sec
                var timekey = key + "_time";
                var redisValue = await redis.GetValue(key);
                if (redisValue != null)
                {
                    var timekeyvalue = await redis.GetValue(timekey);
                    if (DateTime.Parse(timekeyvalue).AddMinutes(1) > time)
                    {
                        _result = JsonConvert.DeserializeObject<List<UserNames>>(redisValue);
                    }
                }
                var result = await user.GetAllUsers();
                _result = result.Select(x => new UserNames { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, SocialProfileName = x.SocialProfileName }).ToList();
                await redis.SetValue(key, JsonConvert.SerializeObject(_result));
                await redis.SetValue(timekey, time.ToString());
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetUsers"), HttpPost]
        public async Task<IActionResult> GetUsers([FromBody] DataTableModel param)
        {
            try
            {

                var _result = await user.GetAllUsers();
                var result = _result;

                param.Columns.ToList().ForEach(p =>
                {
                    if (!String.IsNullOrEmpty(p.Search.Value))
                    {
                        if (p.Data == "id")
                        {
                            result = result.Where(x => x.Id.ToString().Contains(p.Search.Value.ToLower())).ToList();
                        }
                        if (p.Data == "firstName")
                        {
                            result = result.Where(x => x.FirstName.Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "lastName")
                        {
                            result = result.Where(x => x.LastName.Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "email")
                        {
                            result = result.Where(x => x.Email.Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "planExpireDate")
                        {
                            result = result.Where(x => x.PlanExpireDate != null && x.PlanExpireDate.Value.ToString("dd-MM-yyyy").Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "niftyPlanExpireDate")
                        {
                            result = result.Where(x => x.NiftyPlanExpireDate != null && x.NiftyPlanExpireDate.Value.ToString("dd-MM-yyyy").Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "appPlanExpireDate")
                        {
                            result = result.Where(x => x.AppPlanExpireDate != null && x.AppPlanExpireDate.Value.ToString("dd-MM-yyyy").Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "btPlanExpireDate")
                        {
                            result = result.Where(x => x.BTPlanExpireDate != null && x.BTPlanExpireDate.Value.ToString("dd-MM-yyyy").Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "excelPlanExpireDate")
                        {
                            result = result.Where(x => x.ExcelPlanExpireDate != null && x.ExcelPlanExpireDate.Value.ToString("dd-MM-yyyy").Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "createdOn")
                        {
                            result = result.Where(x => x.CreatedOn.ToString().Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "status")
                        {
                            result = result.Where(x => x.Status.ToLower().Contains(p.Search.Value.ToLower())).ToList();
                        }
                        else if (p.Data == "mobile")
                        {
                            result = result.Where(x => x.Mobile != null && x.Mobile.Contains(p.Search.Value.ToLower())).ToList();
                        }
                    }
                });
                param.Order.ToList().ForEach(p =>
                {
                    if (p.Column == 0 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.Id).ToList();
                    }
                    else if (p.Column == 1 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.FirstName).ToList();
                    }
                    else if (p.Column == 2 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.LastName).ToList();
                    }
                    else if (p.Column == 3 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.Email).ToList();
                    }
                    else if (p.Column == 4 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.CreatedOn).ToList();
                    }
                    else if (p.Column == 5 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.PlanExpireDate).ToList();
                    }
                    else if (p.Column == 6 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.NiftyPlanExpireDate).ToList();
                    }
                    else if (p.Column == 7 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.BTPlanExpireDate).ToList();
                    }
                    else if (p.Column == 8 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.ExcelPlanExpireDate).ToList();
                    }
                    else if (p.Column == 9 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.Mobile).ToList();
                    }
                    else if (p.Column == 10 && p.Dir.ToLower() == "asc")
                    {
                        result = result.OrderBy(x => x.Status).ToList();
                    }

                    if (p.Column == 0 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.Id).ToList();
                    }
                    else if (p.Column == 1 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.FirstName).ToList();
                    }
                    else if (p.Column == 2 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.LastName).ToList();
                    }
                    else if (p.Column == 3 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.Email).ToList();
                    }
                    else if (p.Column == 4 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.CreatedOn).ToList();
                    }
                    else if (p.Column == 5 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.PlanExpireDate).ToList();
                    }
                    else if (p.Column == 6 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.NiftyPlanExpireDate).ToList();
                    }
                    else if (p.Column == 7 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.BTPlanExpireDate).ToList();
                    }
                    else if (p.Column == 8 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.ExcelPlanExpireDate).ToList();
                    }
                    else if (p.Column == 9 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.Mobile).ToList();
                    }
                    else if (p.Column == 10 && p.Dir.ToLower() == "desc")
                    {
                        result = result.OrderByDescending(x => x.Status).ToList();
                    }
                });

                return new JsonResult(new
                {
                    param.Draw,
                    Data = result.Skip((param.Start / param.Length) * param.Length).Take(param.Length).ToList(),
                    RecordsFiltered = result.Count,
                    RecordsTotal = _result.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SaveUser"), HttpPost]
        public async Task<IActionResult> SaveUser([FromBody] UserModel.UserDetailsForm values)
        {
            try
            {
                var _result = await user.SaveUserDetails(values);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Register"), HttpPost]
        public async Task<IActionResult> Register([FromBody] UserModel.UserRegisterForm values)
        {
            try
            {
                var _user = new UserModel.UserDetailsForm()
                {
                    Id = values.Id,
                    Email = values.Email,
                    FirstName = values.FirstName,
                    LastName = values.LastName,
                    Password = values.Password,
                    UserType = "USER",
                    Status = "ACTIVE",
                    PlanExpireDate = DateTime.Now.AddDays(2),
                    NiftyPlanExpireDate = DateTime.Now.AddDays(2),
                    AppPlanExpireDate = DateTime.Now.AddDays(60),
                    ShowOINumbers = false,
                    AffiliateCode = values.ReferalCode
                };
                var _result = await user.Register(_user);


                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("ChangePassword"), HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] UserModel.ChangePasswordForm values)
        {
            try
            {
                var _result = await user.ChangePassword(values);

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("ValidateOTP"), HttpPost]
        public async Task<IActionResult> ValidateOTP([FromBody] UserModel.ValidateOTPForm values)
        {
            try
            {

                var _result = await user.ValidateOtp(values.OTP, values.UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Welcome"), HttpPost]
        public async Task<IActionResult> SendWelcomeEmail([FromBody] int userId)
        {
            try
            {

                var res = await user.SendWelcomeEmail(userId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetStates/{id}"), HttpGet]
        public async Task<IActionResult> GetStates(int id)
        {
            try
            {
                var _result = await user.GetAllStates(id);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetCountries"), HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var _result = await user.GetAllCountries();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SendOTP"), HttpPost]
        public async Task<IActionResult> SendOTP([FromBody] UserModel.SendOtpRequest data)
        {
            try
            {

                var _result = await user.SendOTP(data.UserId, data.Type, data.Mobile);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("RegisterM"), HttpPost]
        public async Task<IActionResult> RegisterM([FromBody] SendOtpRequest request)
        {
            try
            {

                var result = await user.RegisterViaMobile(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Authorize]
        [Route("UpdateAppToken"), HttpPost]
        public async Task<IActionResult> UpdateAppToken([FromBody] UpdateAppTokenRequest request)
        {
            try
            {

                await user.UpdateAppToken(request.UserId, request.Token);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SendOTPMail"), HttpPost]
        public async Task<IActionResult> SendOTPMail([FromBody] int UserId)
        {
            try
            {

                var _result = await user.SendOTP(UserId, "mail", null);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SearchUser"), HttpPost]
        public async Task<IActionResult> SearchUser([FromBody] string Email)
        {
            try
            {

                var _result = await user.SearchUser(Email, "mail");
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SearchUserMobile"), HttpPost]
        public async Task<IActionResult> SearchUserMobile([FromBody] string Mobile)
        {
            try
            {

                var _result = await user.SearchUser(Mobile, "mobile");
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("ValidateUser"), HttpPost]
        public async Task<IActionResult> ValidateUser(UserModel.ValidateUser data)
        {
            try
            {
                var _result = await user.ValidateUser(data);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Validate"), HttpPost]
        public async Task<IActionResult> ValidateToken([FromBody] string Token)
        {
            try
            {
                var _result = await user.ValidateUserByToken(Token);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("RefreshToken"), HttpPost]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] string Token)
        {
            try
            {
                var authorization = Request.Headers.Authorization.ToString();
                var token = authorization.Split(' ')[1];
                var _result = await user.RefreshToken(token);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("RefreshToken"), HttpGet]
        [Authorize]
        public async Task<IActionResult> RefreshTokenGet()
        {
            try
            {
                var authorization = Request.Headers.Authorization.ToString();
                var token = authorization.Split(' ')[1];
                var _result = await user.RefreshToken(token);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Logout"), HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var authorization = Request.Headers.Authorization.ToString();
                var userId = authService.GetUserId(authorization);
                await authService.LogOut(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("GetBuilderStrategies"), HttpPost]
        public async Task<IActionResult> GetBuilderStrategies([FromBody] int UserId)
        {
            try
            {
                var _result = await user.GetBuilderStrategies(UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetBuilderStrategiesComponents/{strategyId}"), HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBuilderComponentStrategies(int StrategyId)
        {
            try
            {
                var _result = await user.GetBuilderStrategyComponents(StrategyId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("GetBuilderStrategySubComponents/{strategySubcomponentId}"), HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBuilderStrategySubComponents(int StrategySubcomponentId)
        {
            try
            {
                var _result = await user.GetBuilderStrategySubComponents(StrategySubcomponentId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("SaveStrategy"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveStrategy([FromBody] SaveBuilderStrategyRequest request)
        {
            try
            {
                var _result = await user.SaveBuilderStrategy(request.Id ?? 0, request.StrategyName, request.UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("SaveStrategyComponent"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveStrategyComponent([FromBody] SaveBuilderStrategyComponentRequest request)
        {
            try
            {

                var _result = await user.SaveBuilderStrategyComponent(request.BuilderStrategyComponentId ?? 0,
                           request.BuilderStrategyId,
                           request.FutureSymbolName,
                           request.LastQuoteFutPrice,
                           request.LastQuotePrice,
                           request.SavedFuturePrice,
                           request.SavedSpotPrice,
                           request.SpotSymbolName,
                           request.SymbolName,
                           DateTime.Now,
                           request.Note);

                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("SaveStrategySubComponent"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveStrategySubComponent([FromBody] List<SaveBuilderStrategySubComponentRequest> _request)
        {
            try
            {
                List<int> ids = new();
                foreach (var request in _request)
                {
                    var _result = await user.SaveBuilderStrategySubComponent(request.BuilderStrategyComponentId,
                    request.Delta,
                    request.EntryPrice,
                    request.ExitPrice,
                    request.Iv,
                    request.Expiry,
                    request.LastQuoteLtp,
                    request.LotQty,
                    request.LotSize,
                    request.OptionType,
                    request.Pnl,
                    request.Strike,
                    request.Theta,
                    request.Vega,
                    request.StrikeSymbolName,
                    request.TradeType,
                    request.SymbolId,
                    request.UpdatedOn);

                    ids.Add(_result);

                };
                return Ok(ids);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message} INNER {ex.InnerException}");
            }
        }

        [Route("UpdateExitPrice"), HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateExitPrice([FromBody] UpdateExitPriceRequest request)
        {
            try
            {
                var _result = await user.UpdateExitPrice(request.Id, request.ExitPrice);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("DeleteBuilderStrategy"), HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteBuilderStrategy([FromBody] int strategyId)
        {
            try
            {
                var _result = await user.DeleteBuilderStrategy(strategyId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("DeleteBuilderStrategiesComponent"), HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteBuilderStrategiesComponent([FromBody] int StrategyComponentId)
        {
            try
            {
                var _result = await user.DeleteBuilderStrategyComponent(StrategyComponentId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("DeleteBuilderStrategySubComponent"), HttpPost]
        [Authorize]

        public async Task<IActionResult> DeleteBuilderStrategySubComponent([FromBody] int StrategySubcomponentId)
        {
            try
            {
                var _result = await user.DeleteBuilderStrategySubComponent(StrategySubcomponentId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        #region AdminNotification

        [Route("SaveNotification"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveNotification([FromBody] CommonModel.AdminNotificationRequest request)
        {
            try
            {
                var _result = await user.SaveAdminNotification(request.Subject, request.Notification);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetAllNotifications"), HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllNotifications()
        {
            try
            {
                var _result = await user.GetAdminNotifications();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseCache(Duration = 12 * 60 * 60)]
        [Route("GetReadNotifications/{userId}"), HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReadNotifications(int UserId)
        {
            try
            {
                var _result = await user.GetReadNotifications(UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("ReadNotification"), HttpPost]
        [Authorize]
        public async Task<IActionResult> ReadNotification([FromBody] CommonModel.AdminReadNotificationRequest req)
        {
            try
            {
                var _result = await user.MarkNotificationRead(req.NotificationId, req.UserId, req.Read);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DeleteNotification"), HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteNotification([FromBody] int NotificationId)
        {
            try
            {
                var _result = await user.DeleteAdminNotification(NotificationId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        #endregion

        [Route("DownloadUsers"), HttpPost]
        public async Task<IActionResult> DownloadUsers()
        {
            try
            {
                var _result = await user.DownloadUsers();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DownloadPandLReports"), HttpPost]
        public async Task<IActionResult> DownloadPandLReports([FromBody] int UserId)
        {
            try
            {
                var _result = await user.DownloadPandLReports(UserId, null);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DownloadPandLReportsDW"), HttpPost]
        public async Task<IActionResult> DownloadPandLReportsDateWise([FromBody] PANDLReportDatewiseRequest request)
        {
            try
            {
                var _result = await user.DownloadPandLReports(request.UserId, request.Date);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        #region Alerts
        [Route("SaveAlert"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveAlert([FromBody] UserModel.AlertRequest request)
        {
            try
            {
                var _result = await user.SaveAlert(request.SymbolType, request.Symbol, request.AlertFor, request.Condition, request.Value, request.UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetAlerts"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetAlerts([FromBody] int UserId)
        {
            try
            {
                var _result = await user.GetAlert(UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DeleteAlert"), HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAlert([FromBody] int AlertId)
        {
            try
            {
                var _result = await user.DeleteAlert(AlertId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        #endregion


        #region Trade
        [Route("GetWatchlists"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetWatchlists([FromBody] int UserId)
        {
            try
            {
                var _result = await user.GetWatchlists(UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("SaveWatchList"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveWatchList([FromBody] TradeModel.WatchlistRequest request)
        {
            try
            {
                var _result = await user.SaveWatchList(request.List, request.UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("RemoveWatchList"), HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveWatchList([FromBody] int Id)
        {
            try
            {
                var _result = await user.DeleteSubWatchList(Id);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("Toppers"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetToppers([FromBody] ISortbyToppers request)
        {
            try
            {
                var _result = await user.GetSocialTradingToppers(request.SortBy, null, request.UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("TopperSearch"), HttpPost]
        [Authorize]
        public async Task<IActionResult> TopperSearch([FromBody] string query)
        {
            try
            {
                var _result = await user.GetSocialTradingToppers("profit", query, null);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetTrades"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetTrades([FromBody] int UserId)
        {
            try
            {
                var _result = await user.GetTradeOrders(UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetAllTrades"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetAllTrades([FromBody] int UserId)
        {
            try
            {

                var _result = await user.GetAllTradeOrders(UserId, null);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetAllTradesByDate"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetAllTradesWithDate([FromBody] DateTime date)
        {
            try
            {
                var authorization = Request.Headers.Authorization.ToString();
                var userId = authService.GetUserId(authorization);
                var _result = await user.GetAllTradeOrders(userId, date);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("AllTradesByDate"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetAllTradesByDateOnly([FromBody] DateTime date)
        {
            try
            {


                var _result = await user.GetAllTradeOrders(null, date);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SaveTrade"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveTrade([FromBody] TradeModel.TradeRequest request)
        {
            try
            {
                var authorization = Request.Headers.Authorization.ToString();
                var userId = authService.GetUserId(authorization);
                var _result = await user.SaveTradeOrder(request.List, request.UserId ?? userId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("EditTrade"), HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTrade([FromBody] IEditOrderRequest request)
        {
            try
            {
                await user.EditOrder(request);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("BOOrder"), HttpPost]
        public async Task<IActionResult> BOOrder([FromBody] List<BOOrderRequest> request)
        {
            try
            {
                await user.PlaceBOOrder(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SaveBasket"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveBasket([FromBody] TradeModel.TradeRequest request)
        {
            try
            {
                var authorization = Request.Headers.Authorization.ToString();
                var userId = authService.GetUserId(authorization);
                var _result = await user.SaveBasketOrder(request.List, request.UserId ?? userId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("RemoveTrade"), HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveTrade([FromBody] int TradeId)
        {
            try
            {
                var _result = await user.DeleteTrade(TradeId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("RemoveBasketOrder"), HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveBasketOrder([FromBody] int TradeId)
        {
            try
            {
                var _result = await user.DeleteBasketOrder(TradeId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetPositions"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetPositions([FromBody] int UserId)
        {
            try
            {
                var _result = await user.GetPositions(UserId, null);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetPositionsByDate"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetPositionsByDate([FromBody] DateTime date)
        {
            try
            {
                var authorization = Request.Headers.Authorization.ToString();
                var userId = authService.GetUserId(authorization);
                var _result = await user.GetPositions(userId, date);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SavePositions"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SavePositions([FromBody] TradeModel.PositionsRequest request)
        {
            try
            {
                var _result = await user.SavePositions(request, true);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("ImportPositions"), HttpPost]
        [Authorize]
        public async Task<IActionResult> ImportPositions([FromBody] TradeModel.PositionsRequest request)
        {
            try
            {
                var _result = await user.SavePositions(request, false);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("BulkUpdatePositions"), HttpPost]
        [Authorize]
        public async Task<IActionResult> BulkUpdatePositions([FromBody] List<int> orders)
        {
            try
            {
                var _result = await user.BulkUpdatePositions(orders);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("BulkUpdatePositionsForDay"), HttpPost]
        [Authorize]
        public async Task<IActionResult> BulkUpdatePositionsForDay([FromBody] DateTime date)
        {
            try
            {
                var _result = await user.BulkUpdatePositionsForDay(date);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        //[Route("Basket/{userId}"), HttpPost]
        //[Authorize]
        //public async Task<IActionResult> GetBaskets(int userId)
        //{
        //    try
        //    {
        //        var _result = await user.GetBaskets(userId);
        //        return Ok(_result.Select(x => new { id = x.Id, name = x.Name, status = x.Status }));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.ToString());
        //    }
        //}
        [Route("Basket/{userId}"), HttpPost]
        [Authorize]
        public IActionResult GetBasketOrders(int userId)
        {
            try
            {
                var _result = user.GetBasketOrders(userId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Basket"), HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBasket([FromBody] IBasketRequest request)
        {
            try
            {
                var authorization = Request.Headers.Authorization.ToString();
                var userId = authService.GetUserId(authorization);
                var _result = await user.CreateBasket(request.Name, request.UserId ?? userId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("BasketDelete"), HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteBasket([FromBody] int basketId)
        {
            try
            {
                await user.DeleteBasket(basketId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        #endregion

        #region MarginCalculation

        [Route("UpdateAccessToken"), HttpGet]
        public async Task<IActionResult> UpdateKiteToken()
        {
            if (!string.IsNullOrEmpty(HttpContext.Request.Query["request_token"]))
            {
                var request_token = HttpContext.Request.Query["request_token"];
                var url = "https://api.straddly.com/updatekitetoken?request_token=" + request_token;
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                }
                await kiteBL.UpdateRequestToken(request_token);
                return Ok();
            }

            return BadRequest("INVALID_TOKEN");
        }


        [Route("GetMargin"), HttpPost]
        [Authorize]
        public IActionResult CalculateMargin([FromBody] List<CalculateMarginRequest> request)
        {
            try
            {
                var _result = user.GetMargin(request);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetWalletBalance"), HttpPost]
        [Authorize]
        public IActionResult GetWalletBalance([FromBody] int UserId)
        {
            try
            {
                var _result = user.GetWalletBalance(UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("Funds"), HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFundsToWallet([FromBody] AddFundsToWalletRequest request)
        {
            try
            {
                await user.AddFundsToWallet(request.UserId, request.Amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        #endregion


        #region PaymentPlans
        [Route("GetPaymentPlans"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetPaymentPlans()
        {
            try
            {
                var _result = await user.GetPaymentPlans();
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("SavePaymentPlan"), HttpPost]
        [Authorize]
        public async Task<IActionResult> SavePositions([FromBody] PaymentPlanRequest request)
        {
            try
            {
                var _result = await user.SavePlan(request);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DeletePaymentPlan"), HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePaymentPlan([FromBody] int Id)
        {
            try
            {
                var _result = await user.DeletePlan(Id);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        #endregion

        #region Social Trade
        [Route("Follow"), HttpPost]
        [Authorize]
        public async Task<IActionResult> Follow([FromBody] FollowRequest request)
        {
            try
            {
                var _result = await user.Follow(request.UserId, request.FollowerId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("UnFollow"), HttpPost]
        [Authorize]
        public async Task<IActionResult> UnFollow([FromBody] FollowRequest request)
        {
            try
            {
                var _result = await user.UnFollow(request.UserId, request.FollowerId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("GetFollows"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetFollows([FromBody] int UserId)
        {
            try
            {
                var _result = await user.GetFollows(UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("Followers"), HttpPost]
        public async Task<IActionResult> GetFollowers([FromBody] string key)
        {
            try
            {
                if (key == "updateme")
                {
                    var _result = await user.GetFollowers();
                    return Ok(_result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("TradeAnalysis"), HttpPost]
        [Authorize]
        public IActionResult GetTradeAnalysisDetails([FromBody] int UserId)
        {
            try
            {

                var _result = user.GetTradeAnalysisDetails(UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("FollowerTrades"), HttpPost]
        [Authorize]
        public async Task<IActionResult> GetFollowerTrades([FromBody] int UserId)
        {
            try
            {

                var _result = await user.GetFollowerTrades(UserId);
                return Ok(_result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("updateoinumberstatus"), HttpPost]
        [Authorize]
        public async Task<IActionResult> updateoinumberstatus([FromBody] IUpdateOINumbersRequest request)
        {
            try
            {
                await user.UpdateOINumberStatus(request.UserId, request.Status);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        #endregion

        #region Dhan Credentails
        [Route("Dhan"), HttpPost]
        public async Task<IActionResult> SaveDhanCredentials(IDhanCredentialsRequest request)
        {
            try
            {
                await dhanBL.UpdateDhanCredentials(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DhanOrders"), HttpPost]
        public async Task<IActionResult> GetDhanOrders([FromBody] int userId)
        {
            try
            {
                var res = await dhanBL.GetOrders(userId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DhanPositions"), HttpPost]
        public async Task<IActionResult> GetDhanPositions([FromBody] int userId)
        {
            try
            {
                var res = await dhanBL.GetPositions(userId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [Route("DhanOrder"), HttpPost]
        public async Task<IActionResult> PlaceDhanOrder([FromBody] DhanModel.DhanOrderRequest request)
        {
            try
            {
                await dhanBL.PlaceOrder(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        #endregion

        public class SaveBuilderStrategyRequest
        {
            public int UserId { get; set; }
            public int? Id { get; set; }
            public string StrategyName { get; set; }
        }
        public class SaveBuilderStrategyComponentRequest
        {
            public int? BuilderStrategyComponentId { get; set; }
            public int BuilderStrategyId { get; set; }
            public string FutureSymbolName { get; set; }
            public decimal? LastQuoteFutPrice { get; set; }
            public decimal? LastQuotePrice { get; set; }
            public decimal SavedFuturePrice { get; set; }
            public decimal SavedSpotPrice { get; set; }
            public string SpotSymbolName { get; set; }
            public string SymbolName { get; set; }
            public DateTime TradeTime { get; set; }
            public string Note { get; set; }
        }
        public class UpdateExitPriceRequest
        {
            public int Id { get; set; }
            public decimal ExitPrice { get; set; }
        }

        public class SaveBuilderStrategySubComponentRequest
        {
            public int BuilderStrategyComponentId { get; set; }
            public decimal? Delta { get; set; }
            public decimal EntryPrice { get; set; }
            public decimal? ExitPrice { get; set; }
            public decimal? Iv { get; set; }
            public DateTime Expiry { get; set; }
            public decimal? LastQuoteLtp { get; set; }
            public decimal LotQty { get; set; }
            public int LotSize { get; set; }
            public string OptionType { get; set; }
            public decimal? Pnl { get; set; }
            public decimal Strike { get; set; }
            public decimal? Theta { get; set; }
            public decimal? Vega { get; set; }
            public string StrikeSymbolName { get; set; }
            public string TradeType { get; set; }
            public int? SymbolId { get; set; }
            public DateTime? UpdatedOn { get; set; }
        }

        public class FollowRequest
        {
            public int UserId { get; set; }
            public int FollowerId { get; set; }
        }
        public class PANDLReportDatewiseRequest
        {
            public int UserId { get; set; }
            public DateTime Date { get; set; }
        }
        public class AddFundsToWalletRequest
        {
            public int UserId { get; set; }
            public decimal Amount { get; set; }
        }
        public class UserNames
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string SocialProfileName { get; set; }
        }
        public class IBasketRequest
        {
            public string Name { get; set; }
            public int? UserId { get; set; }
        }
        public class ISortbyToppers
        {
            public string SortBy { get; set; }
            public int? UserId { get; set; }
        }
        public class IUpdateOINumbersRequest
        {
            public int UserId { get; set; }
            public bool Status { get; set; }
        }
        public class UpdateAppTokenRequest
        {
            public int UserId { get; set; }
            public string Token { get; set; }
        }

    }
}
