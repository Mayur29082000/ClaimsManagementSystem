using System;
using System.Collections.Generic;

namespace ClaimsSystems_DAL.Models;

public partial class Policy
{
    public string PolicyId { get; set; } = null!;

    public string? CustomerId { get; set; }

    public string? PolicyNumber { get; set; }

    public string? PolicyType { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public decimal? CoverageAmount { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();

    public virtual Customer? Customer { get; set; }
}
