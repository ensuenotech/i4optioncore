using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Log
{
    public int Id { get; set; }

    public string Log1 { get; set; }

    public DateTime? CreatedOnUtc { get; set; }
}
