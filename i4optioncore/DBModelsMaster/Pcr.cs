using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class Pcr
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Stock { get; set; }

    public long? Cevolume { get; set; }

    public long? Pevolume { get; set; }

    public decimal? Ceoi { get; set; }

    public decimal? Peoi { get; set; }

    public decimal? Oipcr { get; set; }

    public decimal? VolumePcr { get; set; }

    public decimal? Oisum { get; set; }

    public long? TotalVolumeSum { get; set; }

    public DateTime Expiry { get; set; }
}
