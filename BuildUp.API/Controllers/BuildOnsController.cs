using BuildUp.API.Entities;
using BuildUp.API.Entities.BuildOn;
using BuildUp.API.Models;
using BuildUp.API.Models.BuildOn;
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
    [Route("buildup/[controller]")]
    [ApiController]
    [Authorize]
    public class BuildOnsController : ControllerBase
    {
        private readonly IBuildOnsService _buildOnsService;

        public BuildOnsController(IBuildOnsService buildOnsService)
        {
            _buildOnsService = buildOnsService;
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get all build-ons
        /// </summary>
        /// <returns>All build-ons</returns>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="200">Return buildons</response>
        [HttpGet]
        public async Task<ActionResult<List<BuildOn>>> GetBuildOns()
        {
            List<BuildOn> buildOns = await _buildOnsService.GetAllAsync();

            return Ok(buildOns);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get the buildon step corresponding to id
        /// </summary>
        /// <param name="buildOnStepId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>All build-ons</returns>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="404">The buildon step was not found</response>
        /// <response code="200">Return buildons</response>
        [HttpGet("steps/{buildOnStepId:length(24)}")]
        public async Task<ActionResult<BuildOnStep>> GetBuildOnStep(string buildOnStepId)
        {
            BuildOnStep step = await _buildOnsService.GetBuildOnStepAsync(buildOnStepId);

            if (step == null)
            {
                return NotFound();
            }

            return Ok(step);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get the first buildon step
        /// </summary>
        /// <returns>All build-ons</returns>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="404">The buildon step was not found</response>
        /// <response code="200">Return buildons</response>
        [HttpGet("steps/first")]
        public async Task<ActionResult<BuildOnStep>> GetFirstBuildOnStep()
        {
            BuildOnStep step = await _buildOnsService.GetFirstBuildOnStepAsync();

            if (step == null)
            {
                return NotFound();
            }

            return Ok(step);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get all buildon's steps
        /// </summary>
        /// <param name="buildOnId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns></returns>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="200">Return buildon's step</response>
        [HttpGet("{buildOnId:length(24)}/steps")]
        public async Task<ActionResult<List<BuildOnStep>>> GetBuildOnSteps(string buildOnId)
        {
            List<BuildOnStep> buildOnSteps = await _buildOnsService.GetAllStepsAsync(buildOnId);

            return Ok(buildOnSteps);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get buildon's image
        /// </summary>
        /// <param name="buildOnId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns></returns>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="404">The image was not found</response>
        /// <response code="200">Return buildon's image</response>
        [HttpGet("{buildOnId:length(24)}/image")]
        public async Task<ActionResult<byte[]>> GetBuildOnImage(string buildOnId)
        {
            byte[] image = await _buildOnsService.GetImageForBuildOnAsync(buildOnId);

            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get buildon's step's image
        /// </summary>
        /// <param name="buildOnStepId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns></returns>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="404">The image was not found</response>
        /// <response code="200">Return buildon's image</response>
        [HttpGet("steps/{buildOnStepId:length(24)}/image")]
        public async Task<ActionResult<byte[]>> GetBuildOnStepImage(string buildOnStepId)
        {
            byte[] image = await _buildOnsService.GetImageForBuildOnStepAsync(buildOnStepId);

            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get returnings for project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="403">You are not allowed to get returnings</response>
        /// <response code="200">Return the returnings</response>
        [HttpGet("projects/{projectId:length(24)}/returnings")]
        public async Task<ActionResult<List<BuildOnReturning>>> GetReturnings(string projectId)
        {
            var currentUserId = User.Identity.Name;
            List<BuildOnReturning> result;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    result = await _buildOnsService.GetReturningsFromAdmin(projectId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    result = await _buildOnsService.GetReturningFromCoach(currentUserId, projectId);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    result = await _buildOnsService.GetReturningFromBuilder(currentUserId, projectId);
                }
                else
                {
                    return Forbid("You must be part of the Build-Up program");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to get returning: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the returnings: {e.Message}");
            }

            return Ok(result);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get a returnging file
        /// </summary>
        /// <param name="returningId"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="403">You are not allowed to view returning file</response>
        /// <response code="200">Return the returning file</response>
        [HttpGet("projects/{projectId:length(24)}/returnings/{returningId}/file")]
        public async Task<ActionResult<FileModel>> GetReturningFile(string returningId)
        {
            var currentUserId = User.Identity.Name;
            FileModel result;

            try
            {
                if (User.IsInRole(Role.Builder))
                {
                    result = await _buildOnsService.GetReturningFileFromBuilder(currentUserId, returningId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    result = await _buildOnsService.GetReturningFileFromCoach(currentUserId, returningId);
                }
                else if (User.IsInRole(Role.Admin))
                {
                    result = await _buildOnsService.GetReturningFileFromAdmin(returningId);
                }
                else
                {
                    return Forbid("You must be a admin or a coach to get returning file");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to view this file: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the returning file: {e.Message}");
            }

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        /// <summary>
        /// (Admin) Sync build-ons with server
        /// </summary>
        /// <param name="buildOnManageModels"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="200">Return synced buildons</response>
        [HttpPost("sync")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<List<BuildOn>>> SyncBuildOns([FromBody] List<BuildOnManageModel> buildOnManageModels)
        {
            List<BuildOn> result = await _buildOnsService.UpdateBuildOnsAsync(buildOnManageModels);

            return Ok(result);
        }

        /// <summary>
        /// (Admin) Sync build-on steps with server
        /// </summary>
        /// <param name="buildOnId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <param name="buildOnManageStepModels"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="200">Return synced buildons</response>
        [HttpPost("{buildOnId:length(24)}/steps/sync")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<List<BuildOnStep>>> SyncBuildOnSteps(string buildOnId, [FromBody] List<BuildOnStepManageModel> buildOnManageStepModels)
        {
            List<BuildOnStep> result = await _buildOnsService.UpdateBuildOnStepsAsync(buildOnId, buildOnManageStepModels);

            return Ok(result);
        }

        /// <summary>
        /// (Builder) Submit a returning
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="buildOnReturningSubmitModel"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to submit returnings</response>
        /// <response code="200">Return the returning id</response>
        [HttpPost("projects/{projectId:length(24)}/returning")]
        [Authorize(Roles = Role.Builder)]
        public async Task<ActionResult<string>> SubmitReturning(string projectId, [FromBody] BuildOnReturningSubmitModel buildOnReturningSubmitModel)
        {
            var currentUserId = User.Identity.Name;
            string result;

            try
            {
                if (User.IsInRole(Role.Builder))
                {
                    result = await _buildOnsService.SendReturningAsync(currentUserId, projectId, buildOnReturningSubmitModel);
                }
                else
                {
                    return Forbid("You must be a builder to submit returnings");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"Can't submit the returning: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't submit the returning: {e.Message}");
            }

            return Ok(result);
        }

        /// <summary>
        /// (Admin,Coach) Accept returning
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="returningId"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to accept returning</response>
        /// <response code="200">The returning has been accepted</response>
        [HttpPut("projects/{projectId:length(24)}/returnings/{returningId}/accept")]
        [Authorize(Roles = Role.Admin + "," + Role.Coach)]
        public async Task<IActionResult> AcceptReturning(string projectId, string returningId)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    await _buildOnsService.AcceptReturningFromAdmin(projectId, returningId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    await _buildOnsService.AcceptReturningFromCoach(currentUserId, projectId, returningId);
                }
                else
                {
                    return Forbid("You must be a admin or a coach to accept returnings");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to accept returning: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't accept the returning: {e.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// (Admin,Coach) Refuse returning
        /// </summary>
        /// <param name="returningId"></param>
        /// <param name="reason">The reason why it has been refused</param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to refuse returning</response>
        /// <response code="200">The returning has been refused</response>
        [HttpPut("projects/{projectId:length(24)}/returnings/{returningId}/refuse")]
        [Authorize(Roles = Role.Admin + "," + Role.Coach)]
        public async Task<IActionResult> RefuseReturnging(string returningId, [FromBody] BuildOnReturningRefusingModel reason)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    await _buildOnsService.RefuseReturningFromAdmin(returningId, reason.Reason);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    await _buildOnsService.RefuseReturningFromCoach(currentUserId, returningId, reason.Reason);
                }
                else
                {
                    return Forbid("You must be a admin or a coach to refuse returnings");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to refuse this returning: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't refuse the returning file: {e.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// (Admin,Coach) Validate build-on step without any proof
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="buildOnStepId"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to accept build-on steps</response>
        /// <response code="200">The build-on step has been accepted</response>
        [HttpPut("projects/{projectId:length(24)}/validate/{buildOnStepId:length(24)}")]
        [Authorize(Roles = Role.Admin + "," + Role.Coach)]
        public async Task<IActionResult> ValidateBuildOnStep(string projectId, string buildOnStepId)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    await _buildOnsService.ValidateBuildOnStepFromAdmin(projectId, buildOnStepId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    await _buildOnsService.ValidateBuildOnStepFromCoach(currentUserId, projectId, buildOnStepId);
                }
                else
                {
                    return Forbid("You must be a admin or a coach to accept build-ons step");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to accept build-on step: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't accept the build-on step: {e.Message}");
            }

            return Ok();
        }

    }
}
