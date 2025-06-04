using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels
{
    public partial class _2812
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public DateTime LastTradeTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public int Volume { get; set; }
        public int OpenInterest { get; set; }
        public double Atp { get; set; }
    }
}
