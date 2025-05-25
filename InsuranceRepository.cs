using System;
using System.Collections.Generic;
using System.Linq;
using ClaimsSystems_DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClaimsSystems_DAL
{
    public class InsuranceRepository
    {
        private ClaimsSystemsDbContext context;

        public InsuranceRepository(ClaimsSystemsDbContext context)
        {
            this.context = context;
        }


        // POST /api/policies - Add new policy using stored procedure
        public int AddPolicyUsingUSP(string customerId, string policyNumber, string policyType,
                                     DateTime issueDate, DateTime expiryDate, decimal coverageAmount)
        {
            int returnResult = 0;

            SqlParameter prmCustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter prmPolicyNo = new SqlParameter("@PolicyNumber", policyNumber);
            SqlParameter prmType = new SqlParameter("@PolicyType", policyType);
            SqlParameter prmIssue = new SqlParameter("@IssueDate", issueDate);
            SqlParameter prmExpiry = new SqlParameter("@ExpiryDate", expiryDate);
            SqlParameter prmAmount = new SqlParameter("@CoverageAmount", coverageAmount);

            SqlParameter prmReturn = new SqlParameter
            {
                ParameterName = "@ReturnResult",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            try
            {
                context.Database.ExecuteSqlRaw(
                    "EXEC @ReturnResult = usp_AddPolicy @CustomerId, @PolicyNumber, @PolicyType, @IssueDate, @ExpiryDate, @CoverageAmount",
                    prmReturn, prmCustomerId, prmPolicyNo, prmType, prmIssue, prmExpiry, prmAmount
                );

                returnResult = Convert.ToInt32(prmReturn.Value);
            }
            catch (Exception ex)
            {
                returnResult = -99;
            }

            return returnResult;
        }




        // GET /api/policies - Get all policies
        public List<Policy> GetAllPolicies()
        {
            var policiesList = (from p in context.Policies
                                orderby p.PolicyId
                                select p).ToList();

            return policiesList;
        }



        // PUT /api/policies/{id} - Update policy info manually (EF-style)
        public bool UpdatePolicy(string policyId, string policyType, DateTime expiryDate, decimal coverageAmount)
        {
            bool status = false;

            try
            {
                var policy = context.Policies.FirstOrDefault(p => p.PolicyId == policyId);
                if (policy != null)
                {
                    policy.PolicyType = policyType;
                    policy.ExpiryDate = DateOnly.FromDateTime(expiryDate);
                    policy.CoverageAmount = coverageAmount;
                    context.SaveChanges();
                    status = true;
                }
            }
            catch(Exception e)
            {
                status = false;
            }

            return status;
        }


        // GET /api/policies/{id}/status - Check if policy is active
        public bool IsPolicyActive(string policyId)
        {
            var policy = context.Policies.FirstOrDefault(p => p.PolicyId == policyId);
            if (policy != null)
            {
                return policy.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today);
            }
            return false;
        }




        // GET /api/claims - Get all submitted claims
        public List<Claim> GetAllClaims()
        {
            var claimsList = (from c in context.Claims
                              orderby c.ClaimId
                              select c).ToList();

            return claimsList;
        }




        //GET /api/claims/{id} - Get detailed claim info by ID
        public Claim GetClaimById(string claimId)
        {
            var claim = context.Claims.FirstOrDefault(c => c.ClaimId == claimId);
            return claim;
        }


    }
}

