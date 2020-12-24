using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
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
        /// (Admin) Get all coachs
        /// </summary>
        /// <returns>All coachs</returns>
        /// <response code="401">You are not allowed to view coachs</response>
        /// <response code="200">Return all coachs</response>
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<List<Coach>>> GetAllCoach()
        {
            List<Coach> coachs = await _coachService.GetAllCoachsAsync();

            return Ok(coachs);
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
        /// (Builder,Admin) Return the user corresponding to the coach
        /// </summary>
        /// <param name="coachId"></param>
        /// <returns></returns>
        /// <response code="403">You are not allowed to view this user info</response>
        /// <response code="404">The user doesn't exist</response>
        /// <response code="200">Return user infos</response>
        [Authorize(Roles = Role.Admin + "," + Role.Builder)]
        [HttpGet("{coachId:length(24)}/user")]
        public async Task<ActionResult<User>> GetUser(string coachId)
        {
            var currentUserId = User.Identity.Name;
            User user;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    user = await _coachService.GetUserFromAdminAsync(coachId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    user = await _coachService.GetUserFromCoachAsync(currentUserId, coachId);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    user = await _coachService.GetUserFromBuilderAsync(currentUserId, coachId);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the user: {e.Message}");
            }

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// (Admin,Coach) Get the builders of a coach
        /// </summary>
        /// <param name="coachId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The coach's builder</returns>
        /// <response code="401">You are not allowed to view this coach's builders</response>s
        /// <response code="403">You are not allowed to view this coach's builders</response>
        /// <response code="404">The coach's builders doesn't exist</response>
        /// <response code="200">Return coach's builders</response>
        [Authorize(Roles = Role.Admin + "," + Role.Coach)]
        [HttpGet("{coachId:length(24)}/builders")]
        public async Task<ActionResult<List<Builder>>> GetBuildersForCoach(string coachId)
        {
            var currentUserId = User.Identity.Name;
            List<Builder> builders;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    builders = await _coachService.GetBuildersFromAdminAsync(coachId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    builders = await _coachService.GetBuildersFromCoachAsync(currentUserId, coachId);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the builders: {e.Message}");
            }

            return Ok(builders);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get the coach's card
        /// </summary>
        /// <param name="coachId" example="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to view coach's card</response>
        /// <response code="404">The coach's card was not found</response>
        /// <response code="200">Return coach's card</response>
        [HttpGet("{coachId:length(24)}/card")]
        public async Task<ActionResult<byte[]>> GetBuilderCard(string coachId)
        {
            var image = await _coachService.GetCoachCardAsync(coachId);

            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }

        /// <summary>
        /// (Coach,Admin) Get coach's form answers
        /// </summary>
        /// <param name="coachId"></param>
        /// <returns>The builder's form answer</returns>
        /// <response code="401">You are not allowed to view this coach's form</response>s
        /// <response code="403">You are not allowed to view this coach's form</response>
        /// <response code="404">The coach's form doesn't exist</response>
        /// <response code="200">Return coach's form</response>
        [Authorize]
        [HttpGet("{coachId:length(24)}/form")]
        public async Task<ActionResult<List<BuildupFormQA>>> GetCoachFormQAs(string coachId)
        {
            var currentUserId = User.Identity.Name;
            List<BuildupFormQA> result;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    result = await _coachService.GetCoachFormFromAdminAsync(coachId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    result = await _coachService.GetCoachFormFromCoachAsync(currentUserId, coachId);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the coach's form: {e.Message}");
            }

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
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

        /// <summary>
        /// (Coach,Admin) Update a coach
        /// </summary>
        /// <param name="coachId" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <param name="coachUpdateModel"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to update this coach</response>
        /// <response code="403">You are not allowed to update this coach</response>
        /// <response code="200">The coach has been successfully updated</response>
        [Authorize(Roles = Role.Admin + "," + Role.Coach)]
        [HttpPut("{coachId:length(24)}/update")]
        public async Task<IActionResult> UpdateCoach(string coachId, [FromBody] CoachUpdateModel coachUpdateModel)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    await _coachService.UpdateCoachFromAdminAsync(coachId, coachUpdateModel);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    await _coachService.UpdateCoachFromCoachAsync(currentUserId, coachId, coachUpdateModel);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Can't update the coach: {e.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// (Admin) Refuse a coach
        /// </summary>
        /// <param name="coachId" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to refuse this coach</response>
        /// <response code="200">The coach has been successfully refused</response>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("{coachId:length(24)}/refuse")]
        public async Task<IActionResult> RefuseCoach(string coachId)
        {
            await _coachService.RefuseCoachAsync(coachId);

            return Ok();
        }
    }
}
