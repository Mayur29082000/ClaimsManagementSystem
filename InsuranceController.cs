using Microsoft.AspNetCore.Mvc;
using ClaimsSystems_DAL;
using ClaimsSystems_DAL.Models;
using System;
using System.Collections.Generic;

namespace ClaimsSystems_WebServiceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsuranceController : Controller
    {
        private readonly InsuranceRepository repository;

        public InsuranceController(InsuranceRepository repository)
        {
            this.repository = repository;
        }

        // POST: /api/insurance/add-policy
        [HttpPost("add-policy")]
        public JsonResult AddPolicy(string customerId, string policyNumber, string policyType, DateTime issueDate, DateTime expiryDate, decimal coverageAmount)
        {
            string message;
            try
            {
                int result = repository.AddPolicyUsingUSP(customerId, policyNumber, policyType, issueDate, expiryDate, coverageAmount);

                if (result == 1)
                    message = "Policy added successfully.";
                else if (result == -1)
                    message = " Customer not found.";
                else if (result == -2)
                    message = "Invalid coverage amount.";
                else if (result == -3)
                    message = "Expiry date must be after issue date.";
                else
                    message = "Unexpected error occurred.";
            }
            catch (Exception)
            {
                message = "Exception occurred while adding policy.";
            }

            return new JsonResult(message);
        }

        // GET: /api/insurance/policies
        [HttpGet("policies")]
        public JsonResult GetAllPolicies()
        {
            List<Policy> policies;
            try
            {
                policies = repository.GetAllPolicies();
            }
            catch
            {
                policies = null;
            }
            return new JsonResult(policies);
        }

        //  PUT: /api/insurance/update-policy
        [HttpPut("update-policy")]
        public JsonResult UpdatePolicy(string policyId, string policyType, DateTime expiryDate, decimal coverageAmount)
        {
            string message;
            try
            {
                bool result = repository.UpdatePolicy(policyId, policyType, expiryDate, coverageAmount);
                message = result ? "Policy updated successfully." : " Policy not found or update failed.";
            }
            catch
            {
                message = " Exception occurred while updating policy.";
            }

            return new JsonResult(message);
        }

        // GET: /api/insurance/policy-status/{id}
        [HttpGet("policy-status/{id}")]
        public JsonResult IsPolicyActive(string id)
        {
            bool? status = null;
            try
            {
                status = repository.IsPolicyActive(id);
            }
            catch
            {
                status = null;
            }

            return new JsonResult(status != null
                ? (status == true ? "Policy is active." : " Policy is expired.")
                : "Policy not found.");
        }

        // GET: /api/insurance/claims
        [HttpGet("claims")]
        public JsonResult GetAllClaims()
        {
            List<Claim> claims;
            try
            {
                claims = repository.GetAllClaims();
            }
            catch
            {
                claims = null;
            }
            return new JsonResult(claims);
        }

        // GET: /api/insurance/claims/{id}
        [HttpGet("claims/{id}")]
        public JsonResult GetClaimById(string id)
        {
            Claim claim;
            try
            {
                claim = repository.GetClaimById(id);
            }
            catch
            {
                claim = null;
            }

            if (claim != null)
            {
                return new JsonResult(claim);
            }
            else
            {
                return new JsonResult($" Claim with ID '{id}' not found.");
            }
        }
    }
}
