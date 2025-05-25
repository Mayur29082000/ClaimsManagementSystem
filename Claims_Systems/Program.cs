using System;
using System.Collections.Generic;
using System.Net;
using ClaimsSystems_DAL;
using ClaimsSystems_DAL.Models;

namespace Claims_Systems
{
    public class Program
    {
        static ClaimsSystemsDbContext context;
        static CustomerRepository repository;
        static InsuranceRepository insuranceRepo;
        static ScoreTeamRepository scoreTeamRepo;
        static PaymentsRepository paymentsRepo;
        static ClaimHistoryRepository historyRepo;


        static Program()
        {
            context = new ClaimsSystemsDbContext();
            repository = new CustomerRepository(context);
            insuranceRepo = new InsuranceRepository(context);
            scoreTeamRepo = new ScoreTeamRepository(context);
            paymentsRepo = new PaymentsRepository(context);
            historyRepo = new ClaimHistoryRepository(context);
        }


        static void Main(string[] args)
        {
            #region Customer_Methods

            con_GetAllCustomers();
            con_GetCustomerById();
            con_AddCustomerUsingUSP();
            con_GetCustomerPolicies();
            con_GetCustomerClaims();
            con_AddClaimUsingUSP();
            con_ConvertToCollection();

            #endregion

            //#region Insurance_Methods

            //con_AddPolicyUsingUSP();
            //con_GetAllPolicies();
            //con_UpdatePolicy();
            //con_CheckPolicyStatus();
            //con_GetAllClaims();
            //con_GetClaimById();

            //#endregion

            //#region Score_Team_Methods

            //con_GetAllReviewers();
            //con_GetClaimsAssignedToReviewer();
            //con_GetAllClaimScoreAssignments();

            //#endregion


            //#region Payment_Methods

            //con_MakePaymentUsingUSP();
            //con_GetPaymentsByClaimId();
            //con_GetReceiptsByClaimId();
            //con_GetRemainingAmountForClaim();

            //#endregion


            //#region claim_History_Methods

            //con_GetAllClaimHistory();
            //con_GetClaimHistoryByClaimId();

            //#endregion
        }

        #region Customer_Methods

        //Get all Customer 
        public static void con_GetAllCustomers()
        {
            var customers = repository.GetAllCustomers();
            Console.WriteLine("----------------------------------");
            Console.WriteLine("CategoryId\tCategoryName");
            Console.WriteLine("----------------------------------");
            foreach (var customer in customers)
            {
                Console.WriteLine("{0}\t\t{1}", customer.CustomerId, customer.Name);
            }

            Console.WriteLine("");
        }

        //Get Customer by ID 
        public static void con_GetCustomerById()
        {
            string testId = "C001"; 
            var customer = repository.GetCustomerById(testId);

            if (customer != null)
            {
                Console.WriteLine("\n----------------------------------");
                Console.WriteLine("     Customer Details ");
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"ID        : {customer.CustomerId}");
                Console.WriteLine($"Name      : {customer.Name}");
                Console.WriteLine($"Email     : {customer.Email}");
                Console.WriteLine($"Phone     : {customer.Phone}");
                Console.WriteLine($"Description: {customer.Description}");
            }
            else
            {
                Console.WriteLine($" Customer with ID '{testId}' not found.");
            }

           
        }

        // Add Customer 
        public static void con_AddCustomerUsingUSP()
        {
            string name = "Infosys Ltd.";
            string email = "contact@infosys.com";
            string phone = "08012345678";
            string description = "IT Services Company";

            int result = repository.AddCustomerUsingUSP(name, email, phone, description);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("     Add New Customer ");
            Console.WriteLine("----------------------------------");
            if (result == 1)
                Console.WriteLine($" Customer '{name}' added successfully.");
            else if (result == -1)
                Console.WriteLine($"Customer '{name}' already exists.");
            else
                Console.WriteLine("Unexpected error occurred.");


            Console.WriteLine("");
        }

        // Get Policies of Dummy Customer
        public static void con_GetCustomerPolicies()
        {
            string dummyCustomerId = "C001"; 
            var policies = repository.GetCustomerPolicies(dummyCustomerId);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine($"   Policies for Customer '{dummyCustomerId}'");
            Console.WriteLine("----------------------------------");
            if (policies.Any())
            {
                Console.WriteLine("PolicyID\tPolicyNo\t\tType\t\t\t\tActive");
                foreach (var p in policies)
                {
                    Console.WriteLine($"{p.PolicyId}\t\t{p.PolicyNumber}\t{p.PolicyType}\t{(p.IsActive == true ? "Yes" : "No")}");
                }
            }
            else
            {
                Console.WriteLine("No policies found.");
            }

            Console.WriteLine("");
        }

        // Get Claims for Dummy Customer (via policies)
        public static void con_GetCustomerClaims()
        {
            string dummyCustomerId = "C001"; 
            var claims = repository.GetCustomerClaims(dummyCustomerId);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine($"  Claims for Customer '{dummyCustomerId}'");
            Console.WriteLine("----------------------------------");

            if (claims.Any())
            {
                Console.WriteLine("ClaimID\tType\t\tAmount\t\tStatus");
                foreach (var cl in claims)
                {
                    Console.WriteLine($"{cl.ClaimId}\t{cl.ClaimType}\t{cl.Amount}\t{cl.Status}");
                }
            }
            else
            {
                Console.WriteLine("No claims found.");
            }

            Console.WriteLine("");
        }

        // Add dummy claim using stored procedure
        public static void con_AddClaimUsingUSP()
        {

            string policyId = "P001";
            string type = "Claims";
            string comment = "Transport damage to shipment";
            decimal amount = 75000;

            int result = repository.AddClaimUsingUSP(policyId, type, comment, amount);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("        Raise New Claim              ");
            Console.WriteLine("-----------------------------------");

            switch (result)
            {
                case 1:
                    Console.WriteLine("Claim inserted successfully.");
                    break;
                case -1:
                    Console.WriteLine("Policy not found.");
                    break;
                case -2:
                    Console.WriteLine("Invalid amount.");
                    break;
                case -3:
                    Console.WriteLine("Invalid claim type.");
                    break;
                default:
                    Console.WriteLine("Unexpected error occurred.");
                    break;
            }
        }

        //Convert a dummy claim to 'Collection'
        public static void con_ConvertToCollection()
        {
            string claimId = "CL002"; // must exist and not already 'Collection'
            int result = repository.ConvertToCollection(claimId);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine(" Claim convertion to Collection Case");
            Console.WriteLine("-----------------------------------");

            if (result == 1)
                Console.WriteLine("Claim converted to 'Collection'.");
            else if (result == -1)
                Console.WriteLine("Invalid or already converted.");
            else
                Console.WriteLine("Error occurred during update.");
        }

        #endregion


        #region Insurance_Methods

        // Error In The con_AddPolicyUsingUSP methods
        // 1. Add a new policy using stored procedure
        public static void con_AddPolicyUsingUSP()
        {
            string customerId = "C001";
            string policyNumber = "C001-POL-2025-001";
            string policyType = "Commercial Coverage";
            DateTime issueDate = DateTime.Today;
            DateTime expiryDate = DateTime.Today.AddYears(1);
            decimal coverageAmount = 500000;

            int result = insuranceRepo.AddPolicyUsingUSP(customerId, policyNumber, policyType, issueDate, expiryDate, coverageAmount);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("        Add New Policy             ");
            Console.WriteLine("-----------------------------------");

            switch (result)
            {
                case 1:
                    Console.WriteLine(" Policy inserted successfully.");
                    break;
                case -1:
                    Console.WriteLine(" Customer not found.");
                    break;
                case -2:
                    Console.WriteLine("Invalid coverage amount.");
                    break;
                case -3:
                    Console.WriteLine("Expiry date cannot be before issue date.");
                    break;
                default:
                    Console.WriteLine("Unexpected error occurred.");
                    break;
            }
            Console.WriteLine();
        }

        // 2. Get all policies
        public static void con_GetAllPolicies()
        {
            var policies = insuranceRepo.GetAllPolicies();

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("           All Policies            ");
            Console.WriteLine("-----------------------------------");

            if (policies.Any())
            {
                Console.WriteLine("PolicyID\tPolicyNo\t\tType\t\tActive");
                foreach (var p in policies)
                {
                    Console.WriteLine($"{p.PolicyId}\t{p.PolicyNumber}\t{p.PolicyType}\t{(p.IsActive == true ? "Yes" : "No")}");
                }
            }
            else
            {
                Console.WriteLine("No policies found.");
            }
            Console.WriteLine();
        }

        // 3. Update a policy
        public static void con_UpdatePolicy()
        {
            string policyId = "P001";
            string newType = "Updated Coverage";
            DateTime newExpiry = DateTime.Today.AddYears(2);
            decimal newAmount = 600000;

            bool result = insuranceRepo.UpdatePolicy(policyId, newType, newExpiry, newAmount);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("         Update Policy             ");
            Console.WriteLine("-----------------------------------");

            Console.WriteLine(result
                ? "Policy updated successfully."
                : "Policy update failed or not found.");

            Console.WriteLine();
        }

        // 4. Check if policy is active
        public static void con_CheckPolicyStatus()
        {
            string policyId = "P001";
            bool isActive = insuranceRepo.IsPolicyActive(policyId);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("       Check Policy Status         ");
            Console.WriteLine("-----------------------------------");

            Console.WriteLine($"Policy '{policyId}' is {(isActive ? " Active" : " Expired")}.");
            Console.WriteLine();
        }



        // 5. Get all claims (submitted in system)
        public static void con_GetAllClaims()
        {
            var claims = insuranceRepo.GetAllClaims();

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("           All Claims              ");
            Console.WriteLine("-----------------------------------");

            if (claims.Any())
            {
                Console.WriteLine("ClaimID\t\tType\t\tAmount\t\tStatus");
                foreach (var cl in claims)
                {
                    Console.WriteLine($"{cl.ClaimId}\t{cl.ClaimType}\t{cl.Amount}\t{cl.Status}");
                }
            }
            else
            {
                Console.WriteLine("No claims found.");
            }

            Console.WriteLine();
        }



        // 6. Get detailed info about a claim
        public static void con_GetClaimById()
        {
            string claimId = "CL001";
            var claim = insuranceRepo.GetClaimById(claimId);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("        Claim Details              ");
            Console.WriteLine("-----------------------------------");

            if (claim != null)
            {
                Console.WriteLine($"Claim ID   : {claim.ClaimId}");
                Console.WriteLine($"Policy ID  : {claim.PolicyId}");
                Console.WriteLine($"Type       : {claim.ClaimType}");
                Console.WriteLine($"Comment    : {claim.Comment}");
                Console.WriteLine($"Amount     : {claim.Amount}");
                Console.WriteLine($"Status     : {claim.Status}");
                Console.WriteLine($"Submitted  : {claim.SubmittedAt}");
            }
            else
            {
                Console.WriteLine($" Claim with ID '{claimId}' not found.");
            }

            Console.WriteLine();
        }

        #endregion


        #region Score_Team_Methods

        //List all reviewers

        public static void con_GetAllReviewers()
        {
            var reviewers = scoreTeamRepo.GetAllReviewers();

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("         Score Team Members        ");
            Console.WriteLine("-----------------------------------");

            if (reviewers.Any())
            {
                Console.WriteLine("ReviewerID\tName\t\t\tRole");
                foreach (var r in reviewers)
                {
                    Console.WriteLine($"{r.ReviewerId}\t\t{r.Name}\t\t{r.Role}");
                }
            }
            else
            {
                Console.WriteLine("No reviewers found.");
            }

            Console.WriteLine();
        }



        //Claims assigned to a reviewer
        public static void con_GetClaimsAssignedToReviewer()
        {
            string reviewerId = "SC001"; // test reviewer

            var claims = scoreTeamRepo.GetClaimsAssignedToReviewer(reviewerId);

            Console.WriteLine("\n--------------------------------------------");
            Console.WriteLine($"   Claims Assigned to Reviewer '{reviewerId}'");
            Console.WriteLine("--------------------------------------------");

            if (claims.Any())
            {
                Console.WriteLine("AlconID\tClaimID\tDecision\tReviewedAt");
                foreach (var c in claims)
                {
                    Console.WriteLine($"{c.AlconId}\t{c.ClaimId}\t{c.Decision}\t{c.ReviewedAt}");
                }
            }
            else
            {
                Console.WriteLine("No claims assigned to this reviewer.");
            }

            Console.WriteLine();
        }


        //get all allocation which is Assign to the score team
        public static void con_GetAllClaimScoreAssignments()
        {
            var assignments = scoreTeamRepo.GetAllClaimScoreAssignments();

            Console.WriteLine("\n------------------------------------------");
            Console.WriteLine("     All Assigned Claim Score Reviews     ");
            Console.WriteLine("------------------------------------------");

            if (assignments.Any())
            {
                Console.WriteLine("AlconID\tClaimID\tReviewerID\tDecision\tReviewedAt");
                foreach (var a in assignments)
                {
                    Console.WriteLine($"{a.AlconId}\t{a.ClaimId}\t{a.ReviewerId}\t{a.Decision}\t{a.ReviewedAt}");
                }
            }
            else
            {
                Console.WriteLine("No claim score assignments found.");
            }

            Console.WriteLine();
        }

        #endregion


        #region Payment_Methods

        //Insert payment using stored procedure
        public static void con_MakePaymentUsingUSP()
        {
            string claimId = "CL001";
            decimal amount = 100000;
            string notes = "Third installment";

            int result = paymentsRepo.MakePaymentUsingUSP(claimId, amount, notes);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("         Make a Payment            ");
            Console.WriteLine("-----------------------------------");

            switch (result)
            {
                case 1:
                    Console.WriteLine(" Payment added successfully.");
                    break;
                case -99:
                    Console.WriteLine(" Error occurred while inserting payment.");
                    break;
                default:
                    Console.WriteLine("Unexpected error.");
                    break;
            }

            Console.WriteLine();
        }


        //Get payments made for a claim
        public static void con_GetPaymentsByClaimId()
        {
            string claimId = "CL001";
            var payments = paymentsRepo.GetPaymentsByClaimId(claimId);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine($"     Payments for Claim {claimId}   ");
            Console.WriteLine("-----------------------------------");

            if (payments.Any())
            {
                Console.WriteLine("PaymentID\tAmount\t\tDate\t\tNotes");
                foreach (var p in payments)
                {
                    Console.WriteLine($"{p.PaymentId}\t\t{p.AmountPaid}\t{p.PaymentDate:d}\t{p.Notes}");
                }
            }
            else
            {
                Console.WriteLine("No payments found for this claim.");
            }

            Console.WriteLine();
        }



        //Get receipts for a claim
        public static void con_GetReceiptsByClaimId()
        {
            string claimId = "CL001";
            var receipts = paymentsRepo.GetReceiptsByClaimId(claimId);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine($"     Receipts for Claim {claimId}   ");
            Console.WriteLine("-----------------------------------");

            if (receipts.Any())
            {
                Console.WriteLine("ReceiptID\tAmount\t\tRemaining\tStatus\tFees");
                foreach (var r in receipts)
                {
                    Console.WriteLine($"{r.ReceiptId}\t\t{r.Amount}\t{r.RemainingAmount}\t{r.Status}\t{r.BuyerFees}");
                }
            }
            else
            {
                Console.WriteLine("No receipts found for this claim.");
            }

            Console.WriteLine();
        }



        //Get remaining balance on a claim
        public static void con_GetRemainingAmountForClaim()
        {
            string claimId = "CL001";
            decimal remaining = paymentsRepo.GetRemainingAmountForClaim(claimId);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("    Remaining Balance for Claim     ");
            Console.WriteLine("-----------------------------------");

            if (remaining == -1)
            {
                Console.WriteLine(" Claim not found.");
            }
            else
            {
                Console.WriteLine($"Remaining balance on claim '{claimId}' is ₹{remaining}");
            }

            Console.WriteLine();
        }

        #endregion



        #region Claim_History_Methods

        //Get all completed claim records
        public static void con_GetAllClaimHistory()
        {
            var history = historyRepo.GetAllClaimHistory();

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("      All Completed Claim History   ");
            Console.WriteLine("-----------------------------------");

            if (history.Any())
            {
                Console.WriteLine("HistoryID\tClaimID\tStatus\t\tClosed On\tComment");
                foreach (var h in history)
                {
                    Console.WriteLine($"{h.HistoryId}\t\t{h.ClaimId}\t{h.Status}\t{h.DateOfClosed:d}\t{h.Comment}");
                }
            }
            else
            {
                Console.WriteLine("No completed claims found in history.");
            }

            Console.WriteLine();
        }



        //Get history for a specific claim
        public static void con_GetClaimHistoryByClaimId()
        {
            string claimId = "CL001";  
            var history = historyRepo.GetClaimHistoryByClaimId(claimId);

            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine($"   Claim History for ID: {claimId}   ");
            Console.WriteLine("-----------------------------------");

            if (history != null)
            {
                Console.WriteLine($"History ID : {history.HistoryId}");
                Console.WriteLine($"Claim ID   : {history.ClaimId}");
                Console.WriteLine($"Status     : {history.Status}");
                Console.WriteLine($"Closed On  : {history.DateOfClosed:d}");
                Console.WriteLine($"Comment    : {history.Comment}");
            }
            else
            {
                Console.WriteLine($"No history found for claim ID '{claimId}'.");
            }

            Console.WriteLine();
        }

        #endregion

    }
}
