using System;
using System.Collections.Generic;

namespace ClaimsSystems_DAL.Models;

public partial class ClaimHistory
{
    public string HistoryId { get; set; } = null!;

    public string? ClaimId { get; set; }

    public string? Status { get; set; }

    public DateTime? DateOfClosed { get; set; }

    public string? Comment { get; set; }

    public virtual Claim? Claim { get; set; }
}
