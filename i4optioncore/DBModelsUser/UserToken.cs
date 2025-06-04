using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class UserToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public string Remarks { get; set; }
}
