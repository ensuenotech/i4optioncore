using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static i4optioncore.Models.UserModel;

namespace i4optioncore.Repositories
{
    public interface IUserBL
    {
        Task<ValidateUserResponse>  ValidateOtp(string Otp, int UserId);
        Task<List<Country>> GetAllCountries();
        Task<List<State>> GetAllStates(int CountryId);
        Task<List<UserModel.UserDetails>> GetAllUsers();
        Task<int> Register(UserModel.UserDetailsForm _user);
        Task<int> SaveUserDetails(UserModel.UserDetailsForm _user);
        Task<bool> ChangePassword(UserModel.ChangePasswordForm _user);
        Task<UserModel.UserDetails> GetUserDetails(int Id);
        UserModel.AddressDetails GetAddress(int UserId);
        Task<bool> DeleteUser(int id);
        Task<int> SendOTP(int? userId, string type, string Mobile);
        Task<bool> SendWelcomeEmail(int userId);
        Task<int?> SearchUser(string Email, string type);
        Task<ValidateUserResponse> ValidateUser(UserModel.ValidateUser data);
        Task<ValidateUserResponse> ValidateUserByToken(string token);
        Task<RefreshTokenResponse> RefreshToken(string Token);
        
        Task<RegisterViaMobileResponse> RegisterViaMobile(SendOtpRequest request);

        #region BuilderStrategies
        Task<List<BuilderStrategy>> GetBuilderStrategies(int UserId);
        Task<List<BuilderStrategyComponent>> GetBuilderStrategyComponents(int BuilderStrategyId);
        Task<List<BuilderStrategySubComponent>> GetBuilderStrategySubComponents(int BuilderStrategySubId);
        Task<int> SaveBuilderStrategy(int BuilderStrategyId, string StrategyName, int UserId);
        Task<int> SaveBuilderStrategyComponent(int BuilderStrategyComponentId, int BuilderStrategyId, string FutureSymbolName,
            decimal? LastQuoteFutPrice, decimal? LastQuotePrice, decimal SavedFuturePrice, decimal SavedSpotPrice, string SpotSymbolName, string SymbolName,
            DateTime TradeTime, string Note);
        Task<int> SaveBuilderStrategySubComponent(int BuilderStrategyComponentId, decimal? Delta, decimal EntryPrice,
            decimal? ExitPrice, decimal? Iv, DateTime Expiry, decimal? LastQuoteLtp, decimal LotQty, int LotSize, string OptionType,
            decimal? Pnl, decimal Strike, decimal? Theta, decimal? Vega, string StrikeSymbolName, string TradeType, int? SymbolId, DateTime? updatedOn);
        Task<bool> DeleteBuilderStrategy(int StrategyId);
        Task<bool> DeleteBuilderStrategyComponent(int BuilderStrategyComponentId);
        Task<bool> DeleteBuilderStrategySubComponent(int BuilderStrategySubComponentId);
        Task<bool> UpdateExitPrice(int Id, decimal ExitPrice);
        #endregion

        #region AdminNotification
        Task<bool> SaveAdminNotification(string subject, string notification);
        Task<List<AdminNotification>> GetAdminNotifications();
        Task<bool> MarkNotificationRead(int NotificationId, int UserId, bool Read);
        Task<List<int>> GetReadNotifications(int UserId);
        Task<bool> DeleteAdminNotification(int NotificationId);
        #endregion
        Task<CommonModel.FileDetails> DownloadUsers();
        Task<CommonModel.FileDetails> DownloadPandLReports(int UserId, DateTime? date);

        string GetKeyValue(string Key);

        #region Alert
        Task<List<Alert>> GetAlert(int UserId);
        Task<bool> SaveAlert(string symbolType, string symbol, string alertFor, string condition, decimal value, int userId);
        Task<bool> DeleteAlert(int AlertId);
        #endregion

        #region Trade
        Task<List<TradeModel.WatchlistDetails>> GetWatchlists(int UserId);
        Task<bool> DeleteSubWatchList(int SubwatchlistId);
        Task<bool> SaveWatchList(List<TradeModel.WatchlistDetails> watchlist, int userId);
        Task<bool> SaveTradeOrder(List<TradeModel.TradeOrderRequestDetails> request, int UserId);
        Task<bool> SaveBasketOrder(List<TradeModel.TradeOrderRequestDetails> request, int UserId);
        Task<List<TradeModel.TradeOrderDetails>> GetTradeOrders(int UserId);
        Task<List<TradeModel.TradeOrderDetails>> GetAllTradeOrders(int? UserId, DateTime? date);
        Task<List<TradeModel.TradeOrderDetails>> GetTodayTrades();
        Task<bool> DeleteTrade(int TradeId);
        Task<bool> DeleteBasketOrder(int id);
        Task<bool> SavePositions(TradeModel.PositionsRequest request, bool clearPrevious);
        decimal GetWalletBalance(int UserId);
        Task<List<SocialTradingUserDetails>> GetSocialTradingToppers(string sortBy, string userSearch, int? userId);
        Task<bool> BulkExecuteTradeOrders(List<BulkTradeOrdersRequest> TradeOrderIds);
        Task<bool> BulkUpdatePositionsForDay(DateTime date);
        Task<List<int>> BulkUpdatePositions(List<int> UserIds);
        Task<int> CreateBasket(string Name, int UserId);
        Task DeleteBasket(int basketId);
        Task<List<Basket>> GetBaskets(int UserId);
        List<BasketDetails> GetBasketOrders(int userId);
        #endregion


        #region PaymentPlans
        Task<bool> SavePlan(PaymentPlanRequest request);
        Task<bool> DeletePlan(int PlanId);
        Task AddFundsToWallet(int UserId, decimal Amount);
        Task<List<Plan>> GetPaymentPlans();

        #endregion

        #region Social Trade
        Task<bool> Follow(int UserId, int FollowerId);
        Task<bool> UnFollow(int UserId, int FollowerId);
        Task<List<int>> GetFollows(int UserId);
        Task<List<Follower>> GetFollowers();
        TradeAnalysisDetails GetTradeAnalysisDetails(int UserId);
        Task<List<TradeOrder>> GetFollowerTrades(int UserId);
        #endregion

        Task UpdateOINumberStatus(int userId, bool status);
        ValidateUserResponse ValidateInternal(string username, string password);
        Task UpdateAppToken(int UserId, string AppToken);
        Task PlaceBOOrder(List<BOOrderRequest> request);
        Task<List<Position>> GetPositions(int UserId, DateTime? date);
        decimal GetMargin(List<CalculateMarginRequest> request);
        Task<List<Wallet>> GetLedger(int userId);
        Task EditOrder(TradeModel.IEditOrderRequest request);
    }
}
