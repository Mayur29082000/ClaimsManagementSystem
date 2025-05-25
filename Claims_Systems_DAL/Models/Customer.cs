using System;
using System.Collections.Generic;

namespace ClaimsSystems_DAL.Models;

public partial class Customer
{
    public string CustomerId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
}
