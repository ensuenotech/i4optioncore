using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Basket
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Status { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<BasketOrder> BasketOrders { get; set; } = new List<BasketOrder>();
}
