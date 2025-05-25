using System;
using System.Collections.Generic;
using System.Linq;
using ClaimsSystems_DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClaimsSystems_DAL
{
    public class PaymentsRepository
    {
        private readonly ClaimsSystemsDbContext context;

        public PaymentsRepository(ClaimsSystemsDbContext context)
        {
            this.context = context;
        }

        //POST /api/payments - Insert payment using stored procedure
        public int MakePaymentUsingUSP(string claimId, decimal amountPaid, string notes)
        {
            int returnResult = 0;

            SqlParameter prmClaimId = new SqlParameter("@claim_id", claimId);
            SqlParameter prmAmount = new SqlParameter("@amount_paid", amountPaid);
            SqlParameter prmNotes = new SqlParameter("@notes", notes);

            try
            {
                context.Database.ExecuteSqlRaw(
                    "EXEC InsertPayment @claim_id, @amount_paid, @notes",
                    prmClaimId, prmAmount, prmNotes
                );

                returnResult = 1;
            }
            catch (Exception)
            {
                returnResult = -99;
            }

            return returnResult;
        }



        // GET /api/payments/{claimId} - Get payments made for a claim
        public List<Payment> GetPaymentsByClaimId(string claimId)
        {
            var payments = (from p in context.Payments
                            where p.ClaimId == claimId
                            orderby p.PaymentDate
                            select p).ToList();
            return payments;
        }




        //GET /api/receipts/{claimId} - Get receipts for a claim
        public List<Receipt> GetReceiptsByClaimId(string claimId)
        {
            var receipts = (from r in context.Receipts
                            where r.ClaimId == claimId
                            orderby r.CreatedAt
                            select r).ToList();
            return receipts;
        }




        //GET /api/claims/{id}/remaining-balance - Get remaining balance on a claim
        public decimal GetRemainingAmountForClaim(string claimId)
        {
            var claim = context.Claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim == null)
                return -1;

            var paidAmount = context.Payments
                            .Where(p => p.ClaimId == claimId)
                            .Sum(p => (decimal?)p.AmountPaid) ?? 0;

            return (decimal)(claim.Amount - paidAmount);
        }
    }
}
