using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class BrokeragePlanUserMapping
{
    public int UserId { get; set; }

    public int PlanId { get; set; }

    public virtual User User { get; set; }
}
