using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Follower
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int FollowerId { get; set; }
}
