using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Wallet
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedOn { get; set; }

    public string Remarks { get; set; }
}
