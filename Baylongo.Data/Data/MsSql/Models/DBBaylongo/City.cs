using System;
using System.Collections.Generic;

namespace Baylongo.Data.Data.MsSql.Models.DBBaylongo;

public partial class City
{
    public int CityId { get; set; }

    public string Name { get; set; } = null!;

    public string Country { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool? CityDefault { get; set; }

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
