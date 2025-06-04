using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Strategy
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<BrokeragePlan> BrokeragePlans { get; set; } = new List<BrokeragePlan>();
}
