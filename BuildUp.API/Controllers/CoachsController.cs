using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
using BuildUp.API.Entities.Notification;
using BuildUp.API.Entities.Notification.CoachRequest;
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
        private readonly INotificationService _notificationService;

        public CoachsController(ICoachsService coachService, INotificationService notificationService)
        {
            _coachService = coachService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// (Admin) Get all coachs
        /// </summary>
        /// <returns>All coachs</returns>
        /// <response code="401">You don't have enough permissions</response>
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
        /// <param name="userId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The coach corresponding to the user id</returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="403">You are not allowed to view this coach info</response>
        /// <response code="404">The coach doesn't exist</response>
        /// <response code="200">Return the coach infos</response>
        [Authorize(Roles = Role.Admin + "," + Role.Coach)]
        [HttpGet("{userId:length(24)}")]
        public async Task<ActionResult<Coach>> GetCoach(string userId)
        {
            var currentUserId = User.Identity.Name;
            Coach coach;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    coach = await _coachService.GetCoachFromAdminAsync(userId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    coach = await _coachService.GetCoachFromCoachAsync(currentUserId, userId);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not authorized to view this coach info: {e.Message}");
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
        /// (Builder,Coach,Admin) Return the user corresponding to the coach
        /// </summary>
        /// <param name="coachId"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="403">You are not allowed to view this builder user</response>
        /// <response code="404">The user doesn't exist</response>
        /// <response code="200">Return the user's infos</response>
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
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to get user's info: {e.Message}");
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
        /// (Admin) Get candidating coachs
        /// </summary>
        /// <returns>A list of candidating coachs</returns>
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="200">return a list of candidating coachs</response>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("candidating")]
        public async Task<ActionResult<List<Coach>>> GetCandidatingCoachs()
        {
            var result = await _coachService.GetCandidatingCoachsAsync();

            return Ok(result);
        }

        /// <summary>
        /// (Admin) Get active coachs
        /// </summary>
        /// <returns>A list of active coachs</returns>
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="200">return a list of active coachs</response>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("active")]
        public async Task<ActionResult<List<Coach>>> GetActiveCoachs()
        {
            var result = await _coachService.GetActiveCoachsAsync();

            return Ok(result);
        }

        /// <summary>
        /// (Builder,Admin) Get available coachs
        /// </summary>
        /// <returns>A list of available coachs</returns>
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="200">return a list of available coachs</response>
        [Authorize(Roles = Role.Builder + "," + Role.Admin)]
        [HttpGet("available")]
        public async Task<ActionResult<List<AvailableCoachModel>>> GetAvailableCoachs()
        {
            var result = await _coachService.GetAvailableCoachAsync();

            return Ok(result);
        }

        /// <summary>
        /// (Admin,Coach) Get the builders of a coach
        /// </summary>
        /// <param name="coachId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The coach's builder</returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to view this coach's builders</response>
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
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to get the builders: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the builders: {e.Message}");
            }

            return Ok(builders);
        }

        /// <summary>
        /// (Coach,Admin) Get coach's form answers
        /// </summary>
        /// <param name="coachId"></param>
        /// <returns>The builder's form answer</returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to view this coach's form</response>
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
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"Impossible to get the coach's form: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the coach's form: {e.Message}");
            }

            return Ok(result);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get the coach's card
        /// </summary>
        /// <param name="coachId" example="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns></returns>
        /// <response code="401">You don't have enough permissions</response>s
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
        /// (Coach) Get requests for a coach
        /// </summary>
        /// <param name="coachId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The requests</returns>
        /// <response code="401">You are not allowed to view requests</response>s
        /// <response code="403">You are not allowed to view requests</response>
        /// <response code="200">Return the requests</response>
        [Authorize(Roles = Role.Coach)]
        [HttpGet("{coachId:length(24)}/coach_requests")]
        public async Task<ActionResult<List<CoachRequest>>> GetCoachRequests(string coachId)
        {
            var currentUserId = User.Identity.Name;
            List<CoachRequest> requests;

            try
            {
                if (User.IsInRole(Role.Coach))
                {
                    requests = await _coachService.GetCoachRequestsAsync(currentUserId, coachId);
                }
                else
                {
                    return Forbid("You must be a coach");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the requests: {e.Message}");
            }

            return Ok(requests);
        }

        /// <summary>
        /// (Coach) Get notification for a coach
        /// </summary>
        /// <param name="coachId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The requests</returns>
        /// <response code="401">You are not allowed to view notifications</response>s
        /// <response code="403">You are not allowed to view notifications</response>
        /// <response code="200">Return the notifications</response>
        [Authorize(Roles = Role.Coach)]
        [HttpGet("{coachId:length(24)}/notifications")]
        public async Task<ActionResult<List<CoachNotification>>> GetNotifications(string coachId)
        {
            var currentUserId = User.Identity.Name;
            List<CoachNotification> notifications;

            try
            {
                if (User.IsInRole(Role.Coach))
                {
                    notifications = await _notificationService.GetCoachNotificationsAsync(coachId);
                }
                else
                {
                    return Forbid("You must be a coach");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the notifications: {e.Message}");
            }

            return Ok(notifications);
        }

        /// <summary>
        /// (*) Register the coach
        /// </summary>
        /// <param name="coachRegisterModel"></param>
        /// <returns>The registered coach ID</returns>
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
        /// <response code="401">You don't have enough permissions</response>
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
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to update this coach: {e.Message}");
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
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="200">The coach has been successfully refused</response>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("{coachId:length(24)}/refuse")]
        public async Task<IActionResult> RefuseCoach(string coachId)
        {
            await _coachService.RefuseCoachAsync(coachId);

            return Ok();
        }

        /// <summary>
        /// (Coach) Sign the coach integration paper
        /// </summary>
        /// <param name="coachId" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <returns>If the PDF was successfully generated</returns>
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="403">You are not allowed to sign</response>
        /// <response code="200">The paper was successfully signed</response>
        [Authorize(Roles = Role.Coach)]
        [HttpPut("{coachId:length(24)}/sign_integration")]
        public async Task<IActionResult> SignIntegration(string coachId)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                var sucess = await _coachService.SignFicheIntegrationAsync(currentUserId, coachId);

                if (sucess)
                {
                    return Ok(true);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to sign: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Signing was unsuccessfull: {e.Message}");
            }

            return BadRequest("The PDF can't be generated");
        }

        /// <summary>
        /// (Coach) Accept a coach request
        /// </summary>
        /// <param name="coachId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <param name="requestId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to accept the requests</response>
        /// <response code="200">The request has been accepted</response>
        [Authorize(Roles = Role.Coach)]
        [HttpPut("{coachId:length(24)}/coach_requests/{requestId:length(24)}/accept")]
        public async Task<IActionResult> AcctepCoachRequest(string coachId, string requestId)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Coach))
                {
                    await _coachService.AcceptCoachRequestAsync(currentUserId, coachId, requestId);
                }
                else
                {
                    return Forbid("You must be a coach");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to accept this request: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't accept the requests: {e.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// (Coach) Refuse a coach request
        /// </summary>
        /// <param name="coachId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <param name="requestId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to refuse the requests</response>
        /// <response code="200">The request has been refused</response>
        [Authorize(Roles = Role.Coach)]
        [HttpPut("{coachId:length(24)}/coach_requests/{requestId:length(24)}/refuse")]
        public async Task<IActionResult> RefuseCoachRequest(string coachId, string requestId)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Coach))
                {
                    await _coachService.RefuseCoachRequestAsync(currentUserId, coachId, requestId);
                }
                else
                {
                    return Forbid("You must be a coach");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"Can't refuse the request: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't refuse the requests: {e.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// (Coach) Mark a notification as read
        /// </summary>
        /// <param name="coachId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <param name="notificationId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <response code="400">Their was an error reading the notification</response> 
        /// <response code="401">You are not allowed to mark this notification as read</response>s
        /// <response code="403">You are not allowed to mark this notification as read</response>
        /// <response code="200">The request has been accepted</response>
        [Authorize(Roles = Role.Coach)]
        [HttpPut("{coachId:length(24)}/notifications/{notificationId:length(24)}/read")]
        public async Task<IActionResult> ReadNotification(string coachId, string notificationId)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Coach))
                {
                    await _notificationService.MakeCoachNotificationReadAsync(coachId, notificationId);
                }
                else
                {
                    return Forbid("You must be a coach");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Can't accept the requests: {e.Message}");
            }

            return Ok();
        }
    }
}
