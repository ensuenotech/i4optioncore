using i4optioncore.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace i4optioncore.Models
{
    public class UserModel
    {
        public class IDhanTokenDetails
        {
            public string Token { get; set; }
            public string ClientId { get; set; }
            public string ClientUcc { get; set; }
        }
        public class UserDetails
        {
            public int Id { get; set; }
            public string UserType { get; set; }
            public string FirstName { get; set; }
            public string Status { get; set; }
            public string Comment { get; set; }
            public string Bio { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public DateTime? CreatedOn { get; set; }
            public DateTime? UpdatedOn { get; set; }
            public DateTime? PlanExpireDate { get; set; }
            public DateTime? BTPlanExpireDate { get; set; }
            public DateTime? ExcelPlanExpireDate { get; set; }
            public DateTime? NiftyPlanExpireDate { get; set; }
            public DateTime? AppPlanExpireDate { get; set; }
            public string Password { get; set; }
            public AddressDetails Address { get; set; }
            public List<UserPaymentDetails> UserPayments { get; set; }
            public string SocialProfileName { get; set; }
            public bool OINumbers { get; set; }
            public string AppToken { get; set; }
            public IDhanTokenDetails Dhan { get; set; }
            public string ReferalCode { get; set; }
            public string AffiliateCode { get; set; }
            public List<AffiliateDetails> Affiliates { get; set; }
            public Decimal WalletBalance { get; set; }
        }
        public class UserPaymentDetails
        {
            public DateTime PaymentDate { get; set; }
            public decimal Amount { get; set; }
            public string PaymentId { get; set; }
            public string Status { get; set; }
            public string OrderId { get; set; }
        }
        public class AffiliateDetails
        {
            public DateTime? RegistrationDate { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public int UserId { get; set; }

            public object Payments { get; set; }
        }

        public class UserDetailsForm
        {
            public int? Id { get; set; }
            public string UserType { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Bio { get; set; }
            public string Comment { get; set; }
            public string Status { get; set; }
            public string Email { get; set; }
            public string Mobile { get; set; }
            public string Password { get; set; }
            public DateTime? PlanExpireDate { get; set; }
            public DateTime? NiftyPlanExpireDate { get; set; }
            public DateTime? BTPlanExpireDate { get; set; }
            public DateTime? ExcelPlanExpireDate { get; set; }
            public DateTime? AppPlanExpireDate { get; set; }
            public int? StateId { get; set; }
            public int? CountryId { get; set; }
            public string Address { get; set; }
            public string GSTIN { get; set; }
            public string Pincode { get; set; }
            public string City { get; set; }
            public string SocialProfileName { get; set; }
            public bool? ShowOINumbers { get; set; }
            public string AffiliateCode { get; set; }
        }
        public class UserRegisterForm
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string ReferalCode { get; set; }
        }
        public class ValidateOTPForm
        {
            public string OTP { get; set; }
            public int UserId { get; set; }
        }
        public class SendOtpRequest
        {
            public int? UserId { get; set; }
            public string Type { get; set; } = "whatsapp";
            public string Mobile { get; set; }
        }
        public class ValidateUser
        {
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class JwtUser
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string UserType { get; set; }
            public DateTime? PlanExpireDate { get; set; }
            public DateTime? EODExpireDate { get; set; }
        }
        public class ChangePasswordForm
        {
            public string Password { get; set; }
            public int UserId { get; set; }
        }
        public class AddressDetails
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
            public int StateId { get; set; }
            public string State { get; set; }
            public string City { get; set; }
            public string PinCode { get; set; }
            public int CountryId { get; set; }
            public string Country { get; set; }
        }
        public class AlertRequest
        {
            public string SymbolType { get; set; }
            public string Symbol { get; set; }
            public string AlertFor { get; set; }
            public string Condition { get; set; }
            public decimal Value { get; set; }
            public int UserId { get; set; }
        }

    }
    public class CalculateMarginRequest
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal TriggerPrice { get; set; }
        public int LotSize { get; set; }
        public string TransactionType { get; set; }
        public int Quantity { get; set; }
        public string Strategy { get; set; }
        public int UserId { get; set; }
    }
    public class PaymentPlanRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string Domain { get; set; }
        public int Days { get; set; }
    }
    public class SocialTradingUserDetails
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string SocialProfileName { get; set; }
        public string LastName { get; set; }
        public decimal PandL { get; set; }
        public decimal AveragePandL { get; set; }
        public decimal Accuracy { get; set; }
        public int NoOfTrades { get; set; }
    }
    public class BulkTradeOrdersRequest
    {
        //public int? Id { get; set; }
        public string guid { get; set; }
        public decimal Price { get; set; }
        public string orderType { get; set; }
    }
    public class TradeAnalysisDetails
    {
        public TradeAnalysisProfitLoss Amount { get; set; }
        public TradeAnalysisProfitLoss NumberOfTrades { get; set; }
        public TradeAnalysisProfitLoss Average { get; set; }
        public TradeAnalysisProfitLoss Accuracy { get; set; }
        public TradeAnalysisProfitLoss MaxTradeAmount { get; set; }
        public TradeAnalysisProfitLossDate MaxTradeDate { get; set; }
        public TradeAnalysisProfitLoss ContributingTrades_80 { get; set; }
        public TradeAnalysisProfitLoss ContributingTrades_60 { get; set; }
        public TradeAnalysisProfitLoss NumberOfTradingDays { get; set; }
        public TradeAnalysisProfitLoss MaxDayAmount { get; set; }
        public TradeAnalysisProfitLossDate MaxDate { get; set; }
        public TradeAnalysisProfitLoss MaxSymbol { get; set; }
        public TradeAnalysisProfitLoss AmountOfDay { get; set; }
        public TradeAnalysisProfitLoss AverageTradePerDay { get; set; }
        public TradeAnalysisProfitLossCounter ContributingDays_80 { get; set; }
        public TradeAnalysisProfitLossCounter ContributingDays_60 { get; set; }
        public TradeAnalysisProfitLossCounter DaysStreak_3 { get; set; }
        public TradeAnalysisProfitLossCounter DaysStreak_5 { get; set; }
        public TradeAnalysisProfitLossCounter DaysStreak_10 { get; set; }
        public TradeAnalysisProfitLossCounter DaysStreak_120 { get; set; }
        public TradeAnalysisProfitLossString TopAsset { get; set; }
        public TradeAnalysisProfitLoss TopAssetContribution { get; set; }
        public TradeAnalysisProfitLossString TopSymbol { get; set; }
        public TradeAnalysisProfitLoss TopSymbolContribution { get; set; }
        public decimal TradeExpense { get; set; }
    }
    public class TradeAnalysisProfitLoss
    {
        public decimal? Profit { get; set; }
        public decimal? Loss { get; set; }
        public decimal? Net { get; set; }
    }

    public class TradeAnalysisProfitLossCounter
    {
        public int? Profit { get; set; }
        public int? Loss { get; set; }
    }
    public class TradeAnalysisProfitLossString
    {
        public string Profit { get; set; }
        public string Loss { get; set; }
    }
    public class TradeAnalysisProfitLossDate
    {
        public DateTime? Profit { get; set; }
        public DateTime? Loss { get; set; }
        public DateTime Net { get; set; }
    }
    public class PANDLCategory
    {
        public string Category { get; set; }
        public decimal Profit { get; set; }
        public decimal Loss { get; set; }
    }
    public class RefreshTokenResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class ValidateUserResponse
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        //public int? Id { get; set; }
        public string UserType { get; set; }
        public string Token { get; set; }
    }
    public class RegisterViaMobileResponse
    {
        public bool Success { get; set; }
        public string Status { get; set; }
        public int? Id { get; set; }
        public UserModel.UserDetails User { get; set; }
    }
    public class BasketDetails
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<BasketOrderDetails> Orders { get; set; }
    }
    public class BasketOrderDetails
    {
        public int Id { get; set; }
        public string OrderType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string RateType { get; set; }
        public string Symbol { get; set; }
        public string OperationType { get; set; }
        public int? BasketId { get; set; }

        public string Strategy { get; set; }

        public DateTime? Expiry { get; set; }

        public int? Strike { get; set; }
    }
    public class IDhanCredentialsRequest
    {
        public int UserId { get; set; }
        public string ClientId { get; set; }
        public string ClientUcc { get; set; }
    }
    public class IUpdateDhanToken
    {
        public string dhanClientId { get; set; }
        public string dhanClientName { get; set; }
        public string dhanClientUcc { get; set; }
        public string givenPowerOfAttorney { get; set; }
        public string accessToken { get; set; }
        public DateTime expiryTime { get; set; }
    }
    public class IDhanConsent
    {
        public string consentId { get; set; }
        public string consentStatus { get; set; }

    }
    public class BOOrderRequest
    {
        public string Guid { get; set; }
        public decimal Price { get; set; }
    }
    public class EODScanRequest
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public string ScanDataName { get; set; }

        public string Volume { get; set; }

        public string Delivery { get; set; }

        public string MovingAverage { get; set; }

        public string CandleStick { get; set; }

        public string Volatility { get; set; }

        public string Future { get; set; }

        public string Option { get; set; }

        public string PriceAction { get; set; }
    }
}
