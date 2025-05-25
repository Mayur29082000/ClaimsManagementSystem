using Microsoft.AspNetCore.Mvc;
using ClaimsSystems_DAL;
using ClaimsSystems_DAL.Models;
using System;
using System.Collections.Generic;

namespace ClaimsSystems_WebServiceLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreTeamController : Controller
    {
        private readonly ScoreTeamRepository repository;

        public ScoreTeamController(ScoreTeamRepository repository)
        {
            this.repository = repository;
        }

        //GET: /api/scoreteam/reviewers
        [HttpGet("reviewers")]
        public JsonResult GetAllReviewers()
        {
            List<ScoreTeam> reviewers;
            try
            {
                reviewers = repository.GetAllReviewers();
            }
            catch
            {
                reviewers = null;
            }
            return new JsonResult(reviewers);
        }

        //GET: /api/scoreteam/{id}/assigned-claims
        [HttpGet("{id}/assigned-claims")]
        public JsonResult GetClaimsAssignedToReviewer(string id)
        {
            List<ClaimScoreAlcon> claims;
            try
            {
                claims = repository.GetClaimsAssignedToReviewer(id);
            }
            catch
            {
                claims = null;
            }

            return new JsonResult(claims);
        }

        // GET: /api/scoreteam/claim-score-alcon
        [HttpGet("claim-score-alcon")]
        public JsonResult GetAllClaimScoreAssignments()
        {
            List<ClaimScoreAlcon> assignments;
            try
            {
                assignments = repository.GetAllClaimScoreAssignments();
            }
            catch
            {
                assignments = null;
            }

            return new JsonResult(assignments);
        }
    }
}
