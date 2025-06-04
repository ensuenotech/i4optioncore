using System;
using System.Collections.Generic;

namespace i4optioncore.Models
{
    public class ImportModel
    {
        public class Import52WeekData
        {
            public string Symbol { get; set; }

            public decimal High { get; set; }

            public DateTime HighDate { get; set; }

            public decimal Low { get; set; }

            public DateTime LowDate { get; set; }
        }

        public class EarningRatioRequest
        {
            public DateTime Date { get; set; } // Corresponds to ErDate in the table
            public List<EarningRatioRequestData> Data { get; set; }
        }

        public class EarningRatioRequestData
        {
            public string IndexName { get; set; } // Corresponds to Symbol in the table
            public DateTime IndexDate { get; set; }
            public decimal OpenIndexValue { get; set; } // New field from JSON
            public decimal HighIndexValue { get; set; } // New field from JSON
            public decimal LowIndexValue { get; set; } // New field from JSON
            public decimal ClosingIndexValue { get; set; } // New field from JSON
            public decimal PointsChange { get; set; } // New field from JSON
            public decimal ChangePercentage { get; set; } // New field from JSON
            public long Volume { get; set; } // New field from JSON
            public decimal Turnover { get; set; } // New field from JSON
            public decimal PE { get; set; } // New field from JSON (corresponds to Pe in the table)
            public decimal PB { get; set; } // New field from JSON (corresponds to Pb in the table)
            public decimal DivYield { get; set; } // New field from JSON (corresponds to DivYield in the table)
        }
    }
}
