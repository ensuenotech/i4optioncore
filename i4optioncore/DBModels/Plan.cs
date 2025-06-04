using System;
using System.Collections.Generic;

namespace i4optioncore.DBModels
{
    public partial class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int Days { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
