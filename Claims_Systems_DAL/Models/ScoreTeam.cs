using System;
using System.Collections.Generic;

namespace ClaimsSystems_DAL.Models;

public partial class ScoreTeam
{
    public string ReviewerId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Role { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<ClaimScoreAlcon> ClaimScoreAlcons { get; set; } = new List<ClaimScoreAlcon>();
}
