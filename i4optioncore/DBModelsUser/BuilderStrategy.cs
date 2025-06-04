using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class BuilderStrategy
{
    public int Id { get; set; }

    public string StrategyName { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool Deleted { get; set; }

    public virtual ICollection<BuilderStrategyComponent> BuilderStrategyComponents { get; set; } = new List<BuilderStrategyComponent>();
}
