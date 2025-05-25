using Microsoft.AspNetCore.Mvc;
using ClaimsSystems_DAL;
using ClaimsSystems_DAL.Models;
using System;
using System.Collections.Generic;

namespace ClaimsSystems_WebServiceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : Controller
    {
        private readonly PaymentsRepository repository;

        public PaymentsController(PaymentsRepository repository)
        {
            this.repository = repository;
        }

        //POST: /api/payments
        [HttpPost]
        public JsonResult MakePayment(string claimId, decimal amountPaid, string notes)
        {
            string message;
            try
            {
                int result = repository.MakePaymentUsingUSP(claimId, amountPaid, notes);

                if (result == 1)
                    message = " Payment added successfully.";
                else
                    message = " Failed to add payment.";
            }
            catch
            {
                message = " Exception occurred while processing payment.";
            }

            return new JsonResult(message);
        }

        // GET: /api/payments/{claimId}
        [HttpGet("{claimId}")]
        public JsonResult GetPaymentsByClaimId(string claimId)
        {
            List<Payment> payments;
            try
            {
                payments = repository.GetPaymentsByClaimId(claimId);
            }
            catch
            {
                payments = null;
            }
            return new JsonResult(payments);
        }



        // GET: /api/receipts/{claimId}
        [HttpGet("/api/receipts/{claimId}")]
        public JsonResult GetReceiptsByClaimId(string claimId)
        {
            List<Receipt> receipts;
            try
            {
                receipts = repository.GetReceiptsByClaimId(claimId);
            }
            catch
            {
                receipts = null;
            }
            return new JsonResult(receipts);
        }

        //GET: /api/claims/{id}/remaining-balance
        [HttpGet("/api/claims/{id}/remaining-balance")]
        public JsonResult GetRemainingAmountForClaim(string id)
        {
            decimal remaining = -1;
            try
            {
                remaining = repository.GetRemainingAmountForClaim(id);
            }
            catch
            {
                return new JsonResult("Error occurred while retrieving balance.");
            }

            if (remaining == -1)
                return new JsonResult($"Claim with ID '{id}' not found.");
            else
                return new JsonResult($"Remaining balance for claim '{id}' is ₹{remaining}.");
        }
    }
}