using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class DhanCredential
{
    public int Id { get; set; }

    public string DhanUcc { get; set; }

    public string DhanClientId { get; set; }

    public int UserId { get; set; }

    public string TokenId { get; set; }

    public string AccessToken { get; set; }

    public virtual User User { get; set; }
}
