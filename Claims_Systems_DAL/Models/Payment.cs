using System;
using System.Collections.Generic;

namespace ClaimsSystems_DAL.Models;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public string? ClaimId { get; set; }

    public string? PayerType { get; set; }

    public decimal? AmountPaid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? Notes { get; set; }

    public virtual Claim? Claim { get; set; }

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
