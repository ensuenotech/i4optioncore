using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Eodscan
{
    public int Id { get; set; }

    public string ScanDataName { get; set; }

    public string Volume { get; set; }

    public string Delivery { get; set; }

    public string MovingAverage { get; set; }

    public string CandleStick { get; set; }

    public string Volatility { get; set; }

    public string Future { get; set; }

    public string Option { get; set; }

    public string PriceAction { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedOn { get; set; }
}
