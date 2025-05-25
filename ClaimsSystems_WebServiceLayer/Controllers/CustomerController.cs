using Microsoft.AspNetCore.Mvc;
using ClaimsSystems_DAL;
using ClaimsSystems_DAL.Models;
using System.Security.Claims;


namespace ClaimsSystems_WebServiceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        CustomerRepository repository;
  
        public CustomerController(CustomerRepository repository)
        {
            this.repository = repository;
        }


        [HttpGet]
        public JsonResult GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                customers = repository.GetAllCustomers();
            }
            catch (Exception ex)
            {
                customers = null;
            }
            return Json(customers);
        }


        // GET: /api/customers/{id}
        [HttpGet("{id}")]
        public JsonResult GetCustomerById(string id)
        {
            Customer customer;
            try
            {
                customer = repository.GetCustomerById(id);
            }
            catch (Exception ex)
            {
                customer = null;
            }
            return new JsonResult(customer);
        }




        //POST: /api/customers/add (accepts input from Swagger)
        [HttpPost("add")]
        public JsonResult AddCustomer (string name, string email, string phone, string description)
        {
            string message = "";
            try
            {
                int result = repository.AddCustomerUsingUSP(
                name, email, phone, description
            );

                if (result == 1)
                    message = $" Customer '{name}' added successfully.";
                else if (result == -1)
                    message = $"Customer '{name}' already exists.";
                else
                    message = "Unexpected error occurred.";

            }
            catch(Exception ex) 
            {
                message = "Some Exception occurred.";

            }
            

            return new JsonResult(message);
        }



        //Get Policies By Customer ID
        // GET: /api/customers/{id}/policies
        [HttpGet("{id}/policies")]
        public JsonResult GetCustomerPolicies(string id)
        {
            List<Policy> policies;
            try
            {
                policies = repository.GetCustomerPolicies(id);
            }
            catch
            {
                policies = null;
            }
            return new JsonResult(policies);
        }



        //Get Claims By Customer ID (via Policy)
        // GET: /api/customers/{id}/claims
        [HttpGet("{id}/claims")]
        public JsonResult GetCustomerClaims(string id)
        {
            List<ClaimsSystems_DAL.Models.Claim> claims ;
            try
            {
                claims = repository.GetCustomerClaims(id);
            }
            catch
            {
                claims = null;
            }
            return new JsonResult(claims);
        }

        // Add new claim using stored procedure
        // POST: /api/claims/add
        [HttpPost("/api/claims/add")]
        public JsonResult AddClaim(string policyId, string claimType, string comment, decimal amount)
        {
            string message = "";
            try
            {
                int result = repository.AddClaimUsingUSP(policyId, claimType, comment, amount);

                
                if (result == 1)
                    message = "Claim inserted successfully.";
                else if (result == -1)
                    message = "Policy not found.";

                else if (result == -2)
                    message = "Invalid amount.";

                else if (result == -3)
                    message = "Invalid claim type.";

                else
                    message = "Unexpected error occurred.";
                        
                
            }
            catch (Exception ex)
            {

                message = "Some Exception occurred.";

            }


            return new JsonResult(message);
            
        }



        // Convert Claim to 'Collection' or Monitor to 'Collection' using stored procedure
        //PUT: /api/claims/{id}/convert-to-collection
        [HttpPut("/api/claims/{id}/convert-to-collection")]
        public JsonResult ConvertClaimToCollection(string id)
        {
          
            string message = "";
            try
            {
                int result = repository.ConvertToCollection(id);


                if (result == 1)
                    message = "Claim converted to 'Collection'.";
                else if (result == -1)
                    message = "Invalid or already converted.";
                else
                    message = "Error occurred during update.";

                
            }
            catch (Exception ex)
            {

                message = "Some Exception occurred.";

            }


            return new JsonResult(message);

        }

    }
}

