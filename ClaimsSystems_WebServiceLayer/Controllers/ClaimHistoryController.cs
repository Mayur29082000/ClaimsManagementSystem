using Microsoft.AspNetCore.Mvc;
using ClaimsSystems_DAL;
using ClaimsSystems_DAL.Models;
using System;
using System.Collections.Generic;

namespace ClaimsSystems_WebServiceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimHistoryController : Controller
    {
        private readonly ClaimHistoryRepository repository;

        public ClaimHistoryController(ClaimHistoryRepository repository)
        {
            this.repository = repository;
        }

        //GET: /api/claimhistory
        [HttpGet]
        public JsonResult GetAllClaimHistory()
        {
            List<ClaimHistory> history;
            try
            {
                history = repository.GetAllClaimHistory();
            }
            catch
            {
                history = null;
            }

            return new JsonResult(history);
        }

        // GET: /api/claimhistory/{claimId}
        [HttpGet("{claimId}")]
        public JsonResult GetClaimHistoryByClaimId(string claimId)
        {
            ClaimHistory history;
            try
            {
                history = repository.GetClaimHistoryByClaimId(claimId);
            }
            catch
            {
                history = null;
            }

            if (history != null)
                return new JsonResult(history);
            else
                return new JsonResult($" No claim history found for Claim ID '{claimId}'.");
        }
    }
}
