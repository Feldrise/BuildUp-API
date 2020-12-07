using BuildUp.API.Entities;
using BuildUp.API.Entities.BuildOn;
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
        /// Get all build-ons
        /// </summary>
        /// <returns>All build-ons</returns>
        /// <response code="401">You are not allowed to view buildons</response>
        /// <response code="200">Return buildons</response>
        [HttpGet]
        public async Task<ActionResult<List<BuildOn>>> GetBuildOns()
        {
            List<BuildOn> buildOns = await _buildOnsService.GetAllAsync();

            return Ok(buildOns);
        }

        /// <summary>
        /// Get all buildon's steps
        /// </summary>
        /// <param name="buildOnId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to view buildon's step</response>
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
        /// <response code="401">You are not allowed to view buildon's image</response>
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
        /// <response code="401">You are not allowed to view buildon's step's image</response>
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
        /// (Admin) Sync build-ons with server
        /// </summary>
        /// <param name="buildOnManageModels"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to view buildon's step</response>
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
        /// <response code="401">You are not allowed to view buildon's step</response>
        /// <response code="200">Return synced buildons</response>
        [HttpPost("{buildOnId:length(24)}/steps/sync")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<List<BuildOnStep>>> SyncBuildOnSteps(string buildOnId, [FromBody] List<BuildOnStepManageModel> buildOnManageStepModels)
        {
            List<BuildOnStep> result = await _buildOnsService.UpdateBuildOnStepsAsync(buildOnId, buildOnManageStepModels);

            return Ok(result);
        }
    }
}
