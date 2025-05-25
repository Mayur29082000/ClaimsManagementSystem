using System;
using System.Collections.Generic;

namespace ClaimsSystems_DAL.Models;

public partial class Claim
{
    public string ClaimId { get; set; } = null!;

    public string? PolicyId { get; set; }

    public string? ClaimType { get; set; }

    public string? Comment { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ClaimHistory> ClaimHistories { get; set; } = new List<ClaimHistory>();

    public virtual ICollection<ClaimScoreAlcon> ClaimScoreAlcons { get; set; } = new List<ClaimScoreAlcon>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Policy? Policy { get; set; }

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
