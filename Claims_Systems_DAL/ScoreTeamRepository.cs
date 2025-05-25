using System;
using System.Collections.Generic;
using System.Linq;
using ClaimsSystems_DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace ClaimsSystems_DAL
{
    public class ScoreTeamRepository
    {
        private ClaimsSystemsDbContext context;

        public ScoreTeamRepository(ClaimsSystemsDbContext context)
        {
            this.context = context;
        }

        //GET /api/score-team - List all reviewers
        public List<ScoreTeam> GetAllReviewers()
        {
            var reviewers = (from r in context.ScoreTeams
                             orderby r.ReviewerId
                             select r).ToList();
            return reviewers;
        }


        //GET /api/score-team/{id}/assigned-claims - Claims assigned to a particular reviewer
        public List<ClaimScoreAlcon> GetClaimsAssignedToReviewer(string reviewerId)
        {
            var reviews = (from r in context.ClaimScoreAlcons
                           where r.ReviewerId == reviewerId
                           orderby r.ReviewedAt
                           select r).ToList();
            return reviews;
        }



        // GET /api/claim-score-alcon - get all allocation which is Assign to the score team 
        public List<ClaimScoreAlcon> GetAllClaimScoreAssignments()
        {
            var allAssignments = (from r in context.ClaimScoreAlcons
                                  orderby r.AlconId
                                  where r.ReviewerId != null
                                  select r).ToList();
            return allAssignments;
        }



      
    }
}
