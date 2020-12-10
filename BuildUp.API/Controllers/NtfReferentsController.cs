using BuildUp.API.Entities;
using BuildUp.API.Models;
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
    public class NtfReferentsController : ControllerBase
    {
        private readonly INtfReferentsService _ntfReferentsService;

        public NtfReferentsController(INtfReferentsService ntfReferentsService)
        {
            _ntfReferentsService = ntfReferentsService;
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get all NTF referents
        /// </summary>
        /// <returns></returns>
        /// <response code="401">You are not allowed to view referents</response>
        /// <response code="200">Return a list of all referents</response>
        [HttpGet]
        public async Task<ActionResult<List<NtfReferent>>> GetAll()
        {
            var result = await _ntfReferentsService.GetAllAsync();

            return Ok(result);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get a single NTF referent
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to view referent</response>
        /// <response code="404">The referent doesn't exist</response>
        /// <response code="200">Return the referent</response>
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<NtfReferent>> GetOne(string id)
        {
            var result = await _ntfReferentsService.GetOneAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// (Admin) Add a NTF referent
        /// </summary>
        /// <param name="ntfReferentManageModel"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to add referent</response>
        /// <response code="400">The request failed</response>
        /// <response code="200">Return the id of the newly created referent</response>
        [Authorize(Roles = Role.Admin)]
        [HttpPost("add")]
        public async Task<ActionResult<String>> AddReferent([FromBody]NtfReferentManageModel ntfReferentManageModel)
        {
            string id;

            try
            {
                id = await _ntfReferentsService.AddOneAsync(ntfReferentManageModel);
            }
            catch (Exception e)
            {
                return BadRequest($"Can't add the referent: {e.Message}");
            }

            return Ok(id);
        }

        /// <summary>
        /// (Admin) Update a NTF referent
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ntfReferentManageModel"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to update referent</response>
        /// <response code="400">The request failed</response>
        /// <response code="200">The referent has been updated</response>
        [HttpPut("{id:length(24)}/update")]
        public async Task<ActionResult<NtfReferent>> UpdateReferent(string id, [FromBody] NtfReferentManageModel ntfReferentManageModel)
        {
            try
            {
                await _ntfReferentsService.UpdateOneAsync(id, ntfReferentManageModel);
            }
            catch (Exception e)
            {
                return BadRequest($"Can't update the referent: {e.Message}");
            }

            return Ok();
        }
    }
}
