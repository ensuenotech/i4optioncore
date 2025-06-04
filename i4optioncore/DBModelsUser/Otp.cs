using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Otp
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Otp1 { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual User User { get; set; }
}
