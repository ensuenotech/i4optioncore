using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels
{
    public partial class Nse20220218
    {
        public string Symbol { get; set; }
        public DateTime? LastTradeTime { get; set; }
        public double? Open { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public double? Close { get; set; }
        public double? Volume { get; set; }
        public double? OpenInterest { get; set; }
    }
}
