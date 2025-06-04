using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Configuration
{
    public int Id { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }
}
