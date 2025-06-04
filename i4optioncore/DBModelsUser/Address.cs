using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Address
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Address1 { get; set; }

    public int StateId { get; set; }

    public string City { get; set; }

    public string PinCode { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public int CountryId { get; set; }

    public string Gstin { get; set; }

    public virtual Country Country { get; set; }

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual State State { get; set; }
}
