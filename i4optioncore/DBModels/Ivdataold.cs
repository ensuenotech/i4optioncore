using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels
{
    public partial class Ivdataold
    {
        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public double PeIv { get; set; }
        public double CeIv { get; set; }
    }
}
