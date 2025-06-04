using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class NiftyUser
{
    public short? Id { get; set; }

    public string UserType { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public DateOnly? PlanExpireDate { get; set; }

    public string Password { get; set; }

    public string Status { get; set; }

    public string Mobile { get; set; }
}
