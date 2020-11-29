using BuildUp.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BuildUp.API.Entities;
using BuildUp.API.Models;

namespace BuildUp.API.Controllers
{
    [Authorize]
    [Route("buildup/[controller]")]
    [ApiController]
    public class BuildersController : ControllerBase
    {
        private readonly IBuildersService _buildersService;

        public BuildersController(IBuildersService buildersService)
        {
            _buildersService = buildersService;
        }

        [Authorize]
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Builder>> GetBuilder(string id)
        {
            var currentUserId = User.Identity.Name;
            Builder builder;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    builder = await _buildersService.GetBuilderFromAdminAsync(id);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    builder = await _buildersService.GetBuilderFromCoachAsync(currentUserId, id);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    builder = await _buildersService.GetBuilderFromBuilderAsync(currentUserId, id);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the builder: {e.Message}");
            }

            if (builder == null)
            {
                return NotFound();
            }

            return builder;
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("candidating")]
        public async Task<ActionResult<List<Builder>>> GetCandidatingBuilders()
        {
            var result = await _buildersService.GetCandidatingBuildersAsync();

            return Ok(result);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("active")]
        public async Task<ActionResult<List<Builder>>> GetActiveBuilders()
        {
            var result = await _buildersService.GetActiveBuildersAsync();

            return Ok(result);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("{id:length(24)}")]
        public async Task<IActionResult> AssignCoach(string id, [FromBody]CoachAssignmentModel coachAssignmentModel)
        {
            await _buildersService.AssignCoach(coachAssignmentModel.CoachId, id);

            return Ok();
        }
    }
}
