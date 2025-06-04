using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels
{
    public partial class BuilderStrategy
    {
        public BuilderStrategy()
        {
            BuilderStrategyComponents = new HashSet<BuilderStrategyComponent>();
        }

        public int Id { get; set; }
        public string StrategyName { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<BuilderStrategyComponent> BuilderStrategyComponents { get; set; }
    }
}
