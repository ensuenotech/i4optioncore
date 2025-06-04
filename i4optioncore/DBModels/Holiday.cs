using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels;

public partial class Holiday
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime Date { get; set; }
}
