using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class FaoParticipantVol
{
    public int Id { get; set; }

    public string ClientType { get; set; }

    public decimal FutureIndexLong { get; set; }

    public decimal FutureIndexShort { get; set; }

    public decimal FutureStockLong { get; set; }

    public decimal FutureStockShort { get; set; }

    public decimal OptionIndexCallLong { get; set; }

    public decimal OptionIndexPutLong { get; set; }

    public decimal OptionIndexCallShort { get; set; }

    public decimal OptionIndexPutShort { get; set; }

    public decimal OptionStockCallLong { get; set; }

    public decimal OptionStockPutLong { get; set; }

    public decimal OptionStockCallShort { get; set; }

    public decimal OptionStockPutShort { get; set; }

    public decimal TotalLongContracts { get; set; }

    public decimal TotalShortContracts { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public DateTime Date { get; set; }
}
