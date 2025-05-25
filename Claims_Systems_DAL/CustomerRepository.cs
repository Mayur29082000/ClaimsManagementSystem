using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClaimsSystems_DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClaimsSystems_DAL
{
    public class CustomerRepository
    {
        private ClaimsSystemsDbContext context;

        public CustomerRepository( ClaimsSystemsDbContext context)
        {
            this.context = context;
        }

        //Get all customers
        public List<Customer> GetAllCustomers()
        {
            var customersList = (from customer in context.Customers
                                  orderby customer.CustomerId
                                  select customer).ToList();

            return customersList;
        }


        //Get Customer By ID
        public Customer GetCustomerById(string customerId)
        {
            var customer = (from c in context.Customers
                            where c.CustomerId == customerId
                            select c).FirstOrDefault();
            return customer;
        }



        //Add New Customer 
        public int AddCustomerUsingUSP(string name, string email, string phone, string description)
        {
            int noOfRowsAffected = 0;
            int returnResult = 0;

            SqlParameter prmName = new SqlParameter("@Name", name);
            SqlParameter prmEmail = new SqlParameter("@Email", email);
            SqlParameter prmPhone = new SqlParameter("@Phone", phone);
            SqlParameter prmDesc = new SqlParameter("@Description", description);

            SqlParameter prmReturnResult = new SqlParameter
            {
                ParameterName = "@ReturnResult",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            try
            {
                noOfRowsAffected = context.Database.ExecuteSqlRaw(
                    "EXEC @ReturnResult = usp_AddCustomer @Name, @Email, @Phone, @Description",
                    prmReturnResult, prmName, prmEmail, prmPhone, prmDesc);

                returnResult = Convert.ToInt32(prmReturnResult.Value);
            }
            catch (Exception)
            {
                noOfRowsAffected = -1;
                returnResult = -99;
            }

            return returnResult;
        }




        //Get Policies By Customer ID
        public List<Policy> GetCustomerPolicies(string customerId)
        {
            var policiesList = (from p in context.Policies
                                where p.CustomerId == customerId
                                orderby p.PolicyId
                                select p).ToList();
            return policiesList;
        }




        //Get Claims By Customer ID (via Policy)
        public List<Claim> GetCustomerClaims(string customerId)
        {
            var claimsList = (from cl in context.Claims
                              join p in context.Policies on cl.PolicyId equals p.PolicyId
                              where p.CustomerId == customerId
                              orderby cl.ClaimId
                              select cl).ToList();
            return claimsList;
        }



        // Add new claim using stored procedure
        public int AddClaimUsingUSP(string policyId, string claimType, string comment, decimal amount)
        {
            int returnResult = 0;

            SqlParameter prmPolicyId = new SqlParameter("@PolicyId", policyId);
            SqlParameter prmType = new SqlParameter("@ClaimType", claimType);
            SqlParameter prmComment = new SqlParameter("@Comment", comment);
            SqlParameter prmAmount = new SqlParameter("@Amount", amount);

            SqlParameter prmReturn = new SqlParameter
            {
                ParameterName = "@ReturnResult",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            try
            {
                context.Database.ExecuteSqlRaw(
                    "EXEC @ReturnResult = usp_AddClaim @PolicyId, @ClaimType, @Comment, @Amount",
                    prmReturn, prmPolicyId, prmType, prmComment, prmAmount
                );

                returnResult = Convert.ToInt32(prmReturn.Value);
            }
            catch (Exception)
            {
                returnResult = -99;
            }

            return returnResult;
        }



        // Convert Claim to 'Collection' or Monitor to 'Collection' using stored procedure
        public int ConvertToCollection(string claimId)
        {
            int returnResult = 0;

            SqlParameter prmClaimId = new SqlParameter("@ClaimId", claimId);
            SqlParameter prmReturn = new SqlParameter
            {
                ParameterName = "@ReturnResult",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            try
            {
                context.Database.ExecuteSqlRaw(
                    "EXEC @ReturnResult = usp_UpdateClaimTypeToCollection @ClaimId",
                    prmReturn, prmClaimId
                );

                returnResult = Convert.ToInt32(prmReturn.Value);
            }
            catch (Exception)
            {
                returnResult = -99;
            }

            return returnResult;
        }

    }
}
