using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class CustomerAddress
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int AddressId { get; set; }

    public virtual Address Address { get; set; }

    public virtual User User { get; set; }
}
