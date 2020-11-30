using BuildUp.API.Entities;
using BuildUp.API.Models.Coachs;
using BuildUp.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Controllers
{
    [Authorize]
    [Route("buildup/[controller]")]
    [ApiController]
    public class CoachsController : ControllerBase
    {
        private readonly ICoachsService _coachService;

        public CoachsController(ICoachsService coachService)
        {
            _coachService = coachService;
        }

        /// <summary>
        /// (Coach,Admin) Get a coach from his user's ID
        /// </summary>
        /// <param name="id" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The coach with all informations</returns>
        /// <response code="403">You are not allowed to view this coach info</response>
        /// <response code="404">The coach doesn't exist</response>
        /// <response code="200">Return the coach infos</response>
        [Authorize(Roles = Role.Admin + "," + Role.Coach)]
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Coach>> GetCoach(string id)
        {
            var currentUserId = User.Identity.Name;
            Coach coach;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    coach = await _coachService.GetCoachFromAdminAsync(id);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    coach = await _coachService.GetCoachFromCoachAsync(currentUserId, id);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the coach: {e.Message}");
            }

            if (coach == null)
            {
                return NotFound();
            }

            return Ok(coach);
        }

        /// <summary>
        /// (Admin) Get candidating coachs
        /// </summary>
        /// <returns>A list of candidating coachs</returns>
        /// <response code="401">You are not allowed to view candidating coachs</response>
        /// <response code="200">return a list of candidating coachs</response>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("candidating")]
        public async Task<ActionResult<List<Builder>>> GetCandidatingCoachs()
        {
            var result = await _coachService.GetCandidatingCoachsAsync();

            return Ok(result);
        }

        /// <summary>
        /// (Admin) Get active coachs
        /// </summary>
        /// <returns>A list of active coachs</returns>
        /// <response code="401">You are not allowed to view active coachs</response>
        /// <response code="200">return a list of active coachs</response>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("active")]
        public async Task<ActionResult<List<Builder>>> GetActiveCoachs()
        {
            var result = await _coachService.GetActiveCoachsAsync();

            return Ok(result);
        }

        /// <summary>
        /// (*) Register the coach
        /// </summary>
        /// <param name="coachRegisterModel"></param>
        /// <returns>The registered user ID</returns>
        /// <response code="400">The coach can't be registered</response>
        /// <response code="200">Return the registered coach id</response>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterCoach([FromBody] CoachRegisterModel coachRegisterModel)
        {
            string coachId;
            try
            {
                coachId = await _coachService.RegisterCoachAsync(coachRegisterModel);
            }
            catch (Exception e)
            {
                return BadRequest($"Can't regster the coach: {e.Message}");
            }

            return Ok(coachId);
        }
    }
}
