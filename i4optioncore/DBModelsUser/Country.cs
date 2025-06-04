using System;
using System.Collections.Generic;

namespace i4optioncore.DBModelsUser;

public partial class Country
{
    public int Id { get; set; }

    public string Name { get; set; }

    public bool Active { get; set; }

    public bool Deleted { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime UpdatedOnUtc { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
