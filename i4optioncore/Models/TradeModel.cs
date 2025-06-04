using System;
using System.Collections.Generic;

namespace i4optioncore.Models
{
    public class TradeModel
    {
        public class SubWatchlistDetails
        {
            public int? Id { get; set; }
            public string Symbol { get; set; }
            public int SymbolId { get; set; }
            public DateTime LastUpdatedTime { get; set; }
            public decimal LTP { get; set; }
            public long TickVolume { get; set; }
            public decimal ATP { get; set; }
            public long TotalVolume { get; set; }
            public decimal Open { get; set; }
            public decimal High { get; set; }
            public decimal Low { get; set; }
            public decimal PreviousClose { get; set; }
            public decimal TodayOI { get; set; }
            public decimal PreviousOIClose { get; set; }
            public long TurnOver { get; set; }
            public decimal Change { get; set; }
            public decimal ChangePercentage { get; set; }
            public decimal OIChange { get; set; }
            public decimal OIChangePercentage { get; set; }
            public int DisplayOrder { get; set; }
        }
        public class WatchlistDetails
        {
            public string Name { get; set; }
            public List<SubWatchlistDetails> WatchLists { get; set; }
        }
        public class TradeOrderDetails
        {
            public int Id { get; set; }
            public string OrderType { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal TriggerPrice { get; set; }
            public string RateType { get; set; }
            public string Status { get; set; }
            public string Symbol { get; set; }
            public string SymbolId { get; set; }
            public DateTime? Time { get; set; }
            public DateTime? ExecutionTime { get; set; }
            public string OperationType { get; set; }
            public string guid { get; set; }
            public int UserId { get; set; }
            public int? BasketId { get; set; }
            public DateTime? Expiry { get; set; }
            public string Strategy { get; set; }
            public int? Strike { get; set; }
            public decimal? WalletBalance { get; set; }
            public string Exchange { get; set; }
            public string InstrumentType { get; set; }
        }
        public class TradeOrderRequestDetails
        {
            public int? Id { get; set; }
            public string OrderType { get; set; }
            public int Quantity{ get; set; }
            public decimal Price { get; set; }
            public decimal TriggerPrice { get; set; }
            public decimal? StopLoss { get; set; }
            public decimal? TargetPrice { get; set; }
            public string RateType { get; set; }
            public string Status { get; set; }
            public string Strategy { get; set; }
            public string Symbol { get; set; }
            public string OperationType { get; set; }
            public int UserId { get; set; }
            public string guid { get; set; }
            public string boguid { get; set; }
            public int? BasketId { get; set; }
            public int? Strike { get; set; }
            public DateTime? Expiry { get; set; }
            public DateTime? Time { get; set; } = DateTime.Now;
            public string Exchange { get; set; }
            public string InstrumentType { get; set; }
        }

        public class WatchlistRequest
        {
            public int UserId { get; set; }
            public List<WatchlistDetails> List { get; set; }
        }
        public class TradeRequest
        {
            public int? UserId { get; set; }
            public List<TradeOrderRequestDetails> List { get; set; }
        }
        public class PositionsRequest
        {
            public int UserId { get; set; }
            public List<PostionsRequestArray> Positions { get; set; }
        }
        public class PostionsRequestArray
        {
            public int? Id { get; set; }
            public decimal BuyAvg { get; set; }
            public decimal SellAvg { get; set; }
            public decimal Ltp { get; set; }
            public string OrderType { get; set; }
            public decimal PandL { get; set; }
            public int BuyQuantity { get; set; }
            public int SellQuantity { get; set; }
            public string Symbol { get; set; }
            public int UserId { get; set; }
            public string Exchange { get; set; }
            public string InstrumentType { get; set; }
        }
        public class IEditOrderRequest
        {
            public string Guid { get; set; }
            public decimal Price { get; set; }
            public decimal? TriggerPrice { get; set; }
        }
    }
}
