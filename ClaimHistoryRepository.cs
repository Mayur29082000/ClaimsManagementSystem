using System;
using System.Collections.Generic;
using System.Linq;
using ClaimsSystems_DAL.Models;

namespace ClaimsSystems_DAL
{
    public class ClaimHistoryRepository
    {
        private readonly ClaimsSystemsDbContext context;

        public ClaimHistoryRepository(ClaimsSystemsDbContext context)
        {
            this.context = context;
        }

        //GET /api/history - Get all completed claim history records
        public List<ClaimHistory> GetAllClaimHistory()
        {
            var historyList = (from h in context.ClaimHistories
                               orderby h.DateOfClosed descending
                               select h).ToList();

            return historyList;
        }

        // GET /api/history/{claimId} - Get history for a specific claim
        public ClaimHistory GetClaimHistoryByClaimId(string claimId)
        {
            var history = (from h in context.ClaimHistories
                           where h.ClaimId == claimId
                           select h).FirstOrDefault();

            return history;
        }
    }
}
