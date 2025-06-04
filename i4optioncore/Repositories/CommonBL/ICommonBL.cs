using i4optioncore.Controllers;
using i4optioncore.DBModels;
using i4optioncore.DBModelsMaster;
using i4optioncore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static i4optioncore.Repositories.CommonBL;

namespace i4optioncore.Repositories
{
    public interface ICommonBL
    {
        //Task<bool> SendMail(CommonModel.SendMailDetails mail);
       
        Task<IExpiryOI> GetExpiryOI(string stock, DateTime expiry);
        Task<List<CommonModel.StockDetails>> GetStocks();
        Task<List<Segment>> GetSegments();
        Task<List<CommonModel.CalendarDetails>> GetCalendars();
        Task<List<CommonModel.SymbolDetails>> GetSymbols();
        Task<List<CommonModel.SymbolDetails>> GetTouchlineSymbols();
        Task<List<string>> GetStrikes(string stockName, DateTime expiry, string Series = "CE");
        Task<List<CommonModel.HistoryRecord>> GetHistory(List<string> Symbols, DateTime From, DateTime To, int Interval);
        Task<List<CommonModel.HistoryRecord>> GetFullHistory(List<string> Symbols, DateTime From, DateTime To, int Interval);
        Task<List<VolumeCommentary>> GetVolumeCommentary();
        Task<List<SpotVolumeCommentary>> GetSpotVolumeCommentary();
        Task<int> GetMarketHoliday();
       
        Task<bool> DeleteCalendarDate(DateTime date);
        Task<bool> SaveCalendarDate(int? id, DateTime date, int calendarId, bool Active);
        Task<bool> SaveCalendar(int id, string Name);
        Task<bool> UpdateMaxPain(decimal MaxPain, int StockId);
        Task<int> SaveStockDetails(CommonModel.StockFormDetails values);
        Task DeleteStock(int StockId);
        Task SubscribeSymbol(List<string> Symbols);
        Task<List<DateTime>> GetExpiryDates(int? StockId);
        Task<List<DateTime>> GetAllExpiryDates(int? CalendarId);
        Task<List<DateTime>> GetAllExpiry();
        Task<List<DateTime>> GetAllFinNiftyExpiry();
        int? GetLotSize(string Stock, DateTime Expiry);
        #region Master
        Task<List<DBModels.HistorySubscription>> GetDataForMaster();
        #endregion

        #region Touchline
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetTouchline(List<string> Symbols);
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetOptionTouchline(string stockName, DateTime expiry);
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetTouchlineByDate(List<string> Symbols, DateTime fromdate, DateTime todate);
        Task<List<CommonModel.TouchlineSubscriptionDetailsWithMto>> GetTouchlineWithMTOBulk(List<string> Symbols, DateTime fromDate, DateTime toDate);
        Task<List<SegmentTouchline>> GetSegmentTouchline(string Segment, DateTime fromdate, DateTime todate);

        //Task<List<CommonModel.TouchlineSubscriptionDetails>> GetTouchlineOHOL();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetTouchlineOption();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetTouchlineIndexOption();
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetTouchlineIndexWriting();
        //Task<List<CommonModel.TouchlineSubscriptionDetails>> GetFarActivity();
        Task<List<CommonModel.TouchlineSubscription1minDetails>> Get1minTouchline(List<string> Symbols);
        Task<List<IScalping>> GetScalping(List<string> symbols);
        List<IActiveOI> GetActiveOI(string symbol, DateTime expiry, int strike);
        List<IActiveVOL> GetActiveVOL(string symbol, DateTime expiry, int strike);
        
        List<IPremiumDecay> GetPremiumDecay(string symbol, DateTime expiry, int strike);
        #endregion
        Task<List<DateTime>> GetHolidays();
        #region Option Window
     
        #endregion

       
        Task<List<MaxPain>> GetMaxPain(List<string> Symbols, DateTime Expiry, DateTime fromTime, DateTime toTime);
        Task<List<Ivdatum>> GetIvdata(DateTime date);
        
        Task SendSms(string number, string otp);
        Task<FutureDashboard> GetFutureDashboard();

        Task<CommonModel.MovingAverageDetails> GetMovingAverage(string Symbol);
        Task<CommonModel.MovingAverageDetails> GetMovingExponential(string Symbol);
       
        List<CommonModel.BreadthDetails> GetBreadth();
        Task<string> SendMsg91Email(string template_id, string Var1, string Var2, string ToEmail, string ToName);
        
        //Task<Ivdatum> GetIvdata(string symbol, DateTime date);
        Task<string> POSTREQUEST(string url, string body);
        Task<string> GETREQUEST(string url);
        List<Volatility> GetVolatility(CommonController.GetVolatilityRequest request);
        Task<List<Ivdatum>> GetIvdata(CommonController.GetIVDataByDateRequest request);
        List<Ivdatum> GetIvdataWeeklyMonthly(CommonController.GetIVDataByDateRequestWeeklyMonthly request);
        Task<List<IFutureRollover>> GetFutureRollOver(string expiry);
        object GetExcelCalendar();
        Task<string> SendWhatsapp(string mobile, string template_name, string name, string expiryDate, string amount, string mailId, string otp);
        Task<List<DBModels.EarningRatio>> GetEarningRatios(string symbol);
        Task<List<CommonModel.TouchlineSubscriptionDetails>> GetTouchlineStockByDate(List<string> Symbols, DateTime fromdate, DateTime todate);
    }
}
