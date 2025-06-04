using System;
using System.Collections.Generic;

namespace i4optioncore.Models
{
    public class StocksModel
    {
        public class HistoricPerformance
        {
            public decimal  Change { get; set; }
            public string Month { get; set; }
            public string Year { get; set; }
        }
        
    }
}
