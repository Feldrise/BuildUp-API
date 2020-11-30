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
using BuildUp.API.Models.Builders;

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

        /// <summary>
        /// (Builder,Coach,Admin) Get a builder from his user's ID
        /// </summary>
        /// <param name="id" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The builder with all informations</returns>
        /// <response code="403">You are not allowed to view this builder info</response>
        /// <response code="404">The builder doesn't exist</response>
        /// <response code="200">Return the logged user infos</response>
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

            return Ok(builder);
        }

        /// <summary>
        /// (Admin) Get candidating builders
        /// </summary>
        /// <returns>A list of candidating builders</returns>
        /// <response code="401">You are not allowed to view candidating builders</response>
        /// <response code="200">return a list of candidating builders</response>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("candidating")]
        public async Task<ActionResult<List<Builder>>> GetCandidatingBuilders()
        {
            var result = await _buildersService.GetCandidatingBuildersAsync();

            return Ok(result);
        }

        /// <summary>
        /// (Admin) Get active builders
        /// </summary>
        /// <returns>A list of active builders</returns>
        /// <response code="401">You are not allowed to view active builders</response>
        /// <response code="200">return a list of active builders</response>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("active")]
        public async Task<ActionResult<List<Builder>>> GetActiveBuilders()
        {
            var result = await _buildersService.GetActiveBuildersAsync();

            return Ok(result);
        }

        /// <summary>
        /// (*) Register the builder
        /// </summary>
        /// <param name="builderRegisterModel"></param>
        /// <returns>The registered user ID</returns>
        /// <response code="400">The builder can't be registered</response>
        /// <response code="200">Return the registered builder id</response>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<String>> RegisterBuilder([FromBody]BuilderRegisterModel builderRegisterModel)
        {
            string builderId;
            try
            {
                builderId = await _buildersService.RegisterBuilderAsync(builderRegisterModel);
            }
            catch (Exception e)
            {
                return BadRequest($"Can't regster the builder: {e.Message}");
            }

            return Ok(builderId);
        }

        /// <summary>
        /// (Admin) Assigna coach to a builder
        /// </summary>
        /// <param name="id" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <param name="coachAssignmentModel"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to view active builders</response>
        /// <response code="200">The coach has been successfully assigned</response>
        [Authorize(Roles = Role.Admin)]
        [HttpPost("{id:length(24)}/assign")]
        public async Task<IActionResult> AssignCoach(string id, [FromBody]CoachAssignmentModel coachAssignmentModel)
        {
            await _buildersService.AssignCoach(coachAssignmentModel.CoachId, id);

            return Ok();
        }
    }
}
