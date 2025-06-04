using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsMaster;

public partial class Sector
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool Deleted { get; set; }
}
