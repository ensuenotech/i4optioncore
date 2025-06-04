using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class BasketOrder
{
    public int Id { get; set; }

    public string OrderType { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TriggerPrice { get; set; }

    public string RateType { get; set; }

    public string Status { get; set; }

    public string Symbol { get; set; }

    public DateTime Time { get; set; }

    public string OperationType { get; set; }

    public int UserId { get; set; }

    public int? BasketId { get; set; }

    public string Strategy { get; set; }

    public DateTime? Expiry { get; set; }

    public int? Strike { get; set; }

    public virtual Basket Basket { get; set; }

    public virtual User User { get; set; }
}
