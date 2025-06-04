using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Coupon
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public string DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public bool Active { get; set; }

    public bool Deleted { get; set; }
}
