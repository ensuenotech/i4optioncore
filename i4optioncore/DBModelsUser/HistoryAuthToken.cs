using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class HistoryAuthToken
{
    public int Id { get; set; }

    public string HistoryToken { get; set; }

    public DateTime UpdatedOn { get; set; }
}
