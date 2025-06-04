using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class Eod
{
    public int Id { get; set; }

    public string Stock { get; set; }

    public DateTime Date { get; set; }

    public decimal Ltp { get; set; }

    public decimal? Day { get; set; }

    public decimal? Week { get; set; }

    public decimal? Month { get; set; }

    public decimal? Month3 { get; set; }

    public decimal? Month6 { get; set; }

    public decimal? Year { get; set; }

    public decimal? YearHigh { get; set; }

    public decimal? YearLow { get; set; }

    public DateTime? YearHighDate { get; set; }

    public DateTime? YearLowDate { get; set; }

    public decimal? Davol5 { get; set; }

    public decimal? Davol10 { get; set; }

    public decimal? Davol20 { get; set; }

    public decimal? Dsma10 { get; set; }

    public decimal? Dsma20 { get; set; }

    public decimal? Dsma50 { get; set; }

    public decimal? Dsma100 { get; set; }

    public decimal? Dsma200 { get; set; }

    public decimal? Ddel5 { get; set; }

    public decimal? Ddel10 { get; set; }

    public decimal? Ddel20 { get; set; }

    public decimal? DayGap { get; set; }
}
