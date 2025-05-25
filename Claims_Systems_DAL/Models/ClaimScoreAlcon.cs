using System;
using System.Collections.Generic;

namespace ClaimsSystems_DAL.Models;

public partial class ClaimScoreAlcon
{
    public string AlconId { get; set; } = null!;

    public string? ClaimId { get; set; }

    public string? ReviewerId { get; set; }

    public int? RiskScore { get; set; }

    public string? Decision { get; set; }

    public string? Comments { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public virtual Claim? Claim { get; set; }

    public virtual ScoreTeam? Reviewer { get; set; }
}
