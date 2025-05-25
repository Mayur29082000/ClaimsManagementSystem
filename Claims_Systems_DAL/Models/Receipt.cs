using System;
using System.Collections.Generic;

namespace ClaimsSystems_DAL.Models;

public partial class Receipt
{
    public string ReceiptId { get; set; } = null!;

    public string? PaymentId { get; set; }

    public string? ClaimId { get; set; }

    public decimal? Amount { get; set; }

    public decimal? RemainingAmount { get; set; }

    public decimal? BuyerFees { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Claim? Claim { get; set; }

    public virtual Payment? Payment { get; set; }
}
