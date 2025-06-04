using i4optioncore.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace i4optioncore.Models
{
    public class CommonModel
    {

        public class SendMailDetails
        {
            public string Body { get; set; }
            public string Subject { get; set; }
            public string To { get; set; }
            public string CC { get; set; }
            public List<string> BCC { get; set; }
            public string ReplyTo { get; set; }
            public List<string> Attachments { get; set; }
        }
        public class StockDetails
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string DisplayName { get; set; }
            public int Depth { get; set; }
            public decimal Change { get; set; }
            public int LotSize { get; set; }
            public int? CalendarId { get; set; }
            public DateTime CreatedOnUtc { get; set; }
            public DateTime UpdatedOnUtc { get; set; }
            public decimal? MaxPain { get; set; }
            public DateTime? MaxPainLastUpdatedUtc { get; set; }
            public bool Active { get; set; }
            public string Name { get; set; }
            public List<string> Segments { get; set; }
            public List<int> SegmentIds { get; set; }
            public object Expiry { get; set; }
            public decimal? FreeFloat { get; set; }
        }
        public class StockFormDetails
        {
            public int? Id { get; set; }
            public string Type { get; set; }
            public string DisplayName { get; set; }
            public int Depth { get; set; }
            public decimal Change { get; set; }
            public int LotSize { get; set; }
            public int CalendarId { get; set; }
            public DateTime CreatedOnUtc { get; set; }
            public DateTime UpdatedOnUtc { get; set; }
            public decimal? MaxPain { get; set; }
            public DateTime? MaxPainLastUpdatedUtc { get; set; }
            public bool Active { get; set; }
            public string Name { get; set; }
            public List<int> SegmentIds { get; set; }
            public CalendarDetails Calendar { get; set; }
            public decimal FreeFloat { get; set; }
        }
        public class CalendarDetails
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<CalendarDateDetails> Dates { get; set; }
            //public List<DateTime> Dates { get; set; }
        }

        public class CalendarDateDetails
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public bool Active { get; set; }
        }
        public class CopyStocks
        {
            public string Status { get; set; }
            public string Type { get; set; }
            public string DisplayName { get; set; }
            public string Name { get; set; }
            public int Depth { get; set; }
            public decimal Change { get; set; }
            public int LotSize { get; set; }
            public string Calender { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public decimal MaxPain { get; set; }
            public DateTime MaxPainlastUpdated { get; set; }
        }
        public class SymbolResponseDetails
        {
            public int SymbolId { get; set; }
            public string Symbol1 { get; set; }
            public string Series { get; set; }
            public string Isin { get; set; }
            public string Exchange { get; set; }
            public int LotSize { get; set; }
            public decimal Strike { get; set; }
            public DateTime Expiry { get; set; }
            public string Alias { get; set; }
            public string Symbol15 { get; set; }
        }
        public class GetStrikesForm
        {
            public string StockName { get; set; }
            public string Series { get; set; }
            public DateTime Expiry { get; set; }
        }
        public class TouchlineDetails
        {
            public string Symbol { get; set; }
            public int SymbolId { get; set; }
            public DateTime LastUpdatedTime { get; set; }
            public decimal Ltp { get; set; }
            public decimal TickVolume { get; set; }
            public decimal Atp { get; set; }
            public long TotalVolume { get; set; }
            public decimal Open { get; set; }
            public decimal High { get; set; }
            public decimal Low { get; set; }
            public decimal PreviousClose { get; set; }
            public decimal TodayOi { get; set; }
            public decimal PreviousOiclose { get; set; }
            public decimal TurnOver { get; set; }
        }
        public class HistoryResponse
        {
            public string Symbol { get; set; }
            public int? SymbolId { get; set; }
            public List<HistoryRecord> Records { get; set; }
        }
        public class HistoryRecord
        {
            public string Symbol { get; set; }
            public DateTime LastTradeTime { get; set; }
            public decimal Open { get; set; }
            public decimal High { get; set; }
            public decimal Low { get; set; }
            public decimal Close { get; set; }
            public decimal Volume { get; set; }
            public long TotalVolume { get; set; }
            public decimal OpenInterest { get; set; }
            public decimal Atp { get; set; }
            public int Interval { get; set; }
        }
        public class FoigDataBar
        {
            public string Symbol { get; set; }
            public decimal OIChange { get; set; }
            public decimal OIChangePercentage { get; set; }
        }
        public class GetHistoryRequest
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public List<string> Symbols { get; set; }
            public int Interval { get; set; }
        }
        public class TouchlineSubscriptionDetails
        {
            public int Id { get; set; }
            public string Symbol { get; set; }
            public int SymbolId { get; set; }
            public DateTime LastUpdatedTime { get; set; }
            public decimal Ltp { get; set; }
            public decimal TickVolume { get; set; }
            public decimal Atp { get; set; }
            public long TotalVolume { get; set; }
            public decimal Open { get; set; }
            public decimal High { get; set; }
            public decimal Low { get; set; }
            public decimal PreviousClose { get; set; }
            public decimal TodayOi { get; set; }
            public decimal PreviousOiclose { get; set; }
            public decimal TurnOver { get; set; }
            public decimal Change { get; set; }
            public decimal ChangePercentage { get; set; }
            public decimal OiChange { get; set; }
            public decimal OiChangePercentage { get; set; }
            public decimal? DeliverablePercentage { get; set; }
            public decimal? DeliverableQuantity { get; set; }
            public decimal? Ddel20 { get; set; }
            public decimal? Davol20 { get; set; }
        }
        public class TouchlineSubscriptionDetailsWithMto
        {
            public int Id { get; set; }
            public string Symbol { get; set; }
            public int SymbolId { get; set; }
            public DateTime LastUpdatedTime { get; set; }
            public decimal Ltp { get; set; }
            public decimal TickVolume { get; set; }
            public decimal Atp { get; set; }
            public long TotalVolume { get; set; }
            public decimal Open { get; set; }
            public decimal High { get; set; }
            public decimal Low { get; set; }
            public decimal PreviousClose { get; set; }
            public decimal TodayOi { get; set; }
            public decimal PreviousOiclose { get; set; }
            public decimal TurnOver { get; set; }
            public decimal Change { get; set; }
            public decimal ChangePercentage { get; set; }
            public decimal OiChange { get; set; }
            public decimal OiChangePercentage { get; set; }
            public long? Delivery { get; set; }
            public decimal? DeliveryPercentage { get; set; }
            public decimal? DDEL20 { get; set; }
            public decimal? DAVOL20 { get; set; }
        }

        public class TouchlineSubscription1minDetails
        {
            public string Symbol { get; set; }
            public int SymbolId { get; set; }
            public decimal Ltp { get; set; }
            public decimal Atp { get; set; }
            public decimal Open { get; set; }
            public decimal High { get; set; }
            public decimal Low { get; set; }
        }
        public class SymbolDetails
        {
            public int SymbolId { get; set; }
            public string Symbol { get; set; }
            public decimal Strike { get; set; }
            public int LotSize { get; set; }
            public List<string> SearchTerm { get; set; }
            public string Alias { get; set; }
            public string TradingSymbol { get; set; }
            public string Series { get; set; }
            public DateTime? Expiry { get; set; }
            public decimal LTP { get; set; }
        }
        public class SymbolDetailsResponse
        {
            public int SymbolId { get; set; }
            public string Symbol { get; set; }
            public int LotSize { get; set; }
            public string TradingSymbol { get; set; }
            public DateTime? Expiry { get; set; }
        }
        public class MaxpainRequest
        {
            public List<string> Symbol { get; set; }
            public DateTime Expiry { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }
        public class AdminNotificationRequest
        {
            public string Subject { get; set; }
            public string Notification { get; set; }
        }
        public class AdminReadNotificationRequest
        {
            public int UserId { get; set; }
            public int NotificationId { get; set; }
            public bool Read { get; set; }
        }
        public class FileDetails
        {
            public string Blob { get; set; }
            public string FileName { get; set; }
            public string Url { get; set; }
        }
        public class IVRequest
        {
            public string Symbol { get; set; }
            public string Expiry { get; set; }
            public decimal CEIV { get; set; }
            public decimal PEIV { get; set; }
        }
        public class UpdateIVPIVRRequest
        {
            public DateTime Date { get; set; }
        }

        public class MovingAverageDetails
        {
            public decimal DMA20 { get; set; }
            public decimal DMA50 { get; set; }
            public decimal DMA100 { get; set; }
            public decimal DMA200 { get; set; }
            public decimal DMA52WL { get; set; }
            public decimal DMA52WH { get; set; }
            public decimal CMP { get; set; }
            public DateTime LastTime { get; set; }
        }
        public class BreadthDetails
        {
            public DateTime Time { get; set; }
            public int AdvanceChange { get; set; }
            public int DeclineChange { get; set; }
            public int AdvanceATP { get; set; }
            public int DeclineATP { get; set; }
            public int AdvanceOpen { get; set; }
            public int DeclineOpen { get; set; }
        }

    }
    public class IActiveOI
    {
        public DateTime LastTradeTime { get; set; }
        public long CEOI { get; set; }
        public long PEOI { get; set; }
        public decimal FutureVwap { get; set; }
        public decimal FutureLTP { get; set; }
    }
    public class IFutureRollover
    {
        public decimal TodayOI { get; set; }
        public decimal PreviousOIClose { get; set; }
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
    }
    public class IActiveVOL
    {
        public DateTime LastTradeTime { get; set; }
        public long CEVOL { get; set; }
        public long PEVOL { get; set; }
        public decimal FutureVwap { get; set; }
    }
    public class IScalping
    {
        public DateTime LastTradeTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Atp { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public decimal OpenInterest { get; set; }
        public string Symbol { get; set; }
    }
    public class IIntradayPCR
    {
        public DateTime LastTradeTime { get; set; }
        public long CEVolume { get; set; }
        public long PEVolume { get; set; }
        public decimal CEOI { get; set; }
        public decimal PEOI { get; set; }
        public decimal OIPCR { get; set; }
        public decimal VolumePCR { get; set; }
        public decimal OISum { get; set; }
        public long TotalVolumeSum { get; set; }
    }
    public class IPremiumDecay
    {
        public DateTime LastTradeTime { get; set; }
        public decimal CEDecay { get; set; }
        public decimal PEDecay { get; set; }
        public decimal Average { get; set; }
    }

    public class IActiveOIRequest
    {
        public string Symbol { get; set; }
        public DateTime Expiry { get; set; }
        public int Strike { get; set; }
    }
    public class IExpiryOI
    {
        public decimal OICE { get; set; }
        public decimal OIPE { get; set; }
        public decimal COICE { get; set; }
        public decimal COIPE { get; set; }
        public long VolumeCE { get; set; }
        public long VolumePE { get; set; }
        public DateTime Expiry { get; set; }
    }
    public class IPCRByDateRequest
    {
        public string Symbol { get; set; }
        public DateTime Expiry { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public enum IEODScreenerENUM
    {
        Greater,
        Lesser
    }
    public class IEODScreenerData
    {
        public string Symbol { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Delivery { get; set; }
        public decimal? MovingAverage { get; set; }
        public string? CandleStick { get; set; }
        public string PriceAction { get; set; }
        public decimal? Volatility { get; set; }
        public string? Future { get; set; }
        public DateTime? Option { get; set; }
        public string Sector { get; set; }
    }
    public class IEODScreenerDataRequest
    {
        public string Column { get; set; }
        public string Type { get; set; }
        public decimal Value { get; set; }
        public IEODScreenerENUM Condition { get; set; }
    }

    public class FOIG
    {
        public string Symbol { get; set; }
        public decimal PriceChangePercentage { get; set; }
        public decimal OIChangePercentage { get; set; }
    }
    public class FOIGRequest
    {
        public int Interval { get; set; }
        public string Type { get; set; }
    }
    public class OptionDashboard
    {
        public string Symbol { get; set; }
        public string FUTSymbol { get; set; }
        public decimal MaxPain { get; set; }
        public decimal MinIV { get; set; }
        public decimal MaxIV { get; set; }
        public decimal MPD { get; set; }
        public decimal SpotPrice { get; set; }
        public decimal FutLTP { get; set; }
        public decimal PreviousClose { get; set; }
        public decimal FutPreviousClose { get; set; }
        public decimal FutPriceChange { get; set; }
        public decimal FutPriceChangePercentage { get; set; }
        public int SymbolId { get; set; }
        public int ATM { get; set; }
        public int FutSymbolId { get; set; }
        public string CEStrikeSymbol { get; set; }
        public string PEStrikeSymbol { get; set; }
        public DateTime LatestExpiry { get; set; }
        public decimal OIPCR { get; set; }
    }
    public class GetPCRRequest
    {
        public string Symbol { get; set; }
        public DateTime Expiry { get; set; }
        public DateTime? Date { get; set; }
    }
}
