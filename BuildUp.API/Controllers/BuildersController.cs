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
using BuildUp.API.Entities.Form;
using BuildUp.API.Models.Projects;
using BuildUp.API.Models.MeetingReports;
using BuildUp.API.Entities.Notification;

namespace BuildUp.API.Controllers
{
    [Authorize]
    [Route("buildup/[controller]")]
    [ApiController]
    public class BuildersController : ControllerBase
    {
        private readonly IBuildersService _buildersService;
        private readonly INotificationService _notificationService;

        public BuildersController(IBuildersService buildersService, INotificationService notificationService)
        {
            _buildersService = buildersService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get a builder from his user's ID
        /// </summary>
        /// <param name="id" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The builder with all informations</returns>
        /// <response code="401">You are not allowed to refuse the requests</response>s
        /// <response code="403"></response>
        /// <response code="404">The builder doesn't exist</response>
        /// <response code="200">Return builder infos</response>
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
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to view this builder: {e.Message}");
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
        /// (Builder,Coach,Admin) Return the user corresponding to the builder
        /// </summary>
        /// <param name="builderId"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to view this user info</response>
        /// <response code="200">Return user infos</response>
        [Authorize(Roles = Role.Admin + "," + Role.Coach + "," + Role.Builder)]
        [HttpGet("{builderId:length(24)}/user")]
        public async Task<ActionResult<User>> GetUser(string builderId)
        {
            var currentUserId = User.Identity.Name;
            User user;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    user = await _buildersService.GetUserFromAdminAsync(builderId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    user = await _buildersService.GetUserFromCoachAsync(currentUserId, builderId);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    user = await _buildersService.GetUserFromBuilderAsync(currentUserId, builderId);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to get this user: {e.Message}");
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
        /// (Admin) Get candidating builders
        /// </summary>
        /// <returns>A list of candidating builders</returns>
        /// <response code="401">You don't have enough permissions</response>s
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
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="200">return a list of active builders</response>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("active")]
        public async Task<ActionResult<List<Builder>>> GetActiveBuilders()
        {
            var result = await _buildersService.GetActiveBuildersAsync();

            return Ok(result);
        }

        /// <summary>
        /// (Builder,Admin) Get the coach of a builder
        /// </summary>
        /// <param name="builderId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The builder's coach</returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to view this builder's coach</response>
        /// <response code="404">The builder's coach doesn't exist</response>
        /// <response code="200">Return builder's coach</response>
        [Authorize(Roles = Role.Admin + "," + Role.Builder)] 
        [HttpGet("{builderId:length(24)}/coach")]
        public async Task<ActionResult<Coach>> GetCoachForBuilder(string builderId)
        {
            var currentUserId = User.Identity.Name;
            Coach coach;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    coach = await _buildersService.GetCoachForBuilderFromAdminAsync(builderId);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    coach = await _buildersService.GetCoachForBuilderFromBuilderAsync(currentUserId, builderId);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to get the builder's coach: {e.Message}");
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
        /// (Builder,Coach,Admin) Get builder's form answers
        /// </summary>
        /// <param name="builderId"></param>
        /// <returns>The builder's form answer</returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to view this builder's form</response>
        /// <response code="404">The builder's form doesn't exist</response>
        /// <response code="200">Return builder's form</response>
        [Authorize]
        [HttpGet("{builderId:length(24)}/form")]
        public async Task<ActionResult<List<BuildupFormQA>>> GetBuilderFormQAs(string builderId)
        {
            var currentUserId = User.Identity.Name;
            List<BuildupFormQA> result;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    result = await _buildersService.GetBuilderFormFromAdminAsync(builderId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    result = await _buildersService.GetBuilderFormFromCoachAsync(currentUserId, builderId);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    result = await _buildersService.GetBuilderFormFromBuilderAsync(currentUserId, builderId);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to view this form: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the builder's form: {e.Message}");
            }

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get builder's project
        /// </summary>
        /// <param name="builderId"></param>
        /// <returns>The builder's project</returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to view this builder's project</response>
        /// <response code="404">The builder's project doesn't exist</response>
        /// <response code="200">Return builder's project</response>
        [Authorize]
        [HttpGet("{builderId:length(24)}/project")]
        public async Task<ActionResult<Project>> GetBuilderProject(string builderId)
        {
            var currentUserId = User.Identity.Name;
            Project result;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    result = await _buildersService.GetBuilderProjectFromAdminAsync(builderId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    result = await _buildersService.GetBuilderProjectFromCoachAsync(currentUserId, builderId);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    result = await _buildersService.GetBuilderProjectFromBuilderAsync(currentUserId, builderId);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to see this project: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the builder's project: {e.Message}");
            }

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get the builder's card
        /// </summary>
        /// <param name="builderId" example="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns></returns>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="404">The builder's card was not found</response>
        /// <response code="200">Return builder's card</response>
        [HttpGet("{builderId:length(24)}/card")]
        public async Task<ActionResult<byte[]>> GetBuilderCard(string builderId)
        {
            var image = await _buildersService.GetBuilderCardAsync(builderId);

            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get builder's meeting reports
        /// </summary>
        /// <param name="builderId"></param>
        /// <returns>The builder's meeting reports</returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to view this builder's meeting reports</response>
        /// <response code="200">Return meeting reports's project</response>
        [Authorize]
        [HttpGet("{builderId:length(24)}/meeting_reports")]
        public async Task<ActionResult<List<MeetingReport>>> GetMeetingReports(string builderId)
        {
            var currentUserId = User.Identity.Name;
            List<MeetingReport> result;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    result = await _buildersService.GetMeetingReportsFromAdminAsync(builderId);
                }
                else if (User.IsInRole(Role.Coach))
                {
                    result = await _buildersService.GetMeetingReportsFromCoachAsync(currentUserId, builderId);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    result = await _buildersService.GetMeetingReportsFromBuilderAsync(currentUserId, builderId);
                }
                else
                {
                    return Unauthorized("You must be part of the Buildup program");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized($"You are not allowed to get reports: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't get the builder's meeting reports: {e.Message}");
            }

            return Ok(result);
        }

        /// <summary>
        /// (Builder) Get notification for a builder
        /// </summary>
        /// <param name="builderId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns>The notifications</returns>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="200">Return the notifications</response>
        [Authorize(Roles = Role.Builder)]
        [HttpGet("{builderId:length(24)}/notifications")]
        public async Task<ActionResult<List<BuilderNotification>>> GetNotifications(string builderId)
        {
            List<BuilderNotification> notifications = await _notificationService.GetBuilderNotificationsAsync(builderId);

            return Ok(notifications);
        }

        /// <summary>
        /// (*) Register the builder
        /// </summary>
        /// <param name="builderRegisterModel" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <returns>The registered user ID</returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="200">Return the registered builder id</response>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterBuilder([FromBody]BuilderRegisterModel builderRegisterModel)
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
        /// (*) Submit a project for a builder
        /// </summary>
        /// <param name="builderId" example="5f1fe90a58c8ab093c4f772a"></param>
        /// <param name="projectSubmitModel"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="403">You are not allowed to submit project for this builder</response>s
        /// <response code="200">Return the submited project's id</response>
        [AllowAnonymous]
        [HttpPost("{builderId:length(24)}/submit_project")]
        public async Task<ActionResult<string>> SubmitProject(string builderId, [FromBody]ProjectSubmitModel projectSubmitModel)
        {
            if (builderId != projectSubmitModel.BuilderId)
            {
                return Forbid("The submitted project doesn't have the same builder ID as the current builder");
            }

            string projectId;

            try
            {
                projectId = await _buildersService.SubmitProjectAsync(projectSubmitModel);
            }
            catch (Exception e)
            {
                return BadRequest($"Can't submit the project: {e.Message}");
            }

            return Ok(projectId);
        }

        /// <summary>
        /// (Admin,Builder) Assigna coach to a builder
        /// </summary>
        /// <param name="builderId" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <param name="coachAssignmentModel"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>
        /// <response code="403">You are not allowed to submit project for this builder</response>s
        /// <response code="200">The coach has been successfully assigned</response>
        [Authorize(Roles = Role.Admin + "," + Role.Builder)]
        [HttpPost("{builderId:length(24)}/assign")]
        public async Task<IActionResult> AssignCoach(string builderId, [FromBody] CoachAssignmentModel coachAssignmentModel)
        {
            try
            {
                await _buildersService.AssignCoachAsync(coachAssignmentModel.CoachId, builderId);
            }
            catch (Exception e)
            {
                return BadRequest($"There was an error assigning the coach: {e.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// (Coach) Submit a new meeting report
        /// </summary>
        /// <param name="builderId" example="5f1fe90a58c8ab093c4f772a"></param>
        /// <param name="createMeetingReportModel"></param>
        /// <returns></returns>
        /// <response code="400">The meeting report can't be created</response>
        /// <response code="401">You are not authorized to create the meeting report</response>
        /// <response code="403">You are not authorized to create the meeting report</response>
        /// <response code="200">Return the created meeting report id</response>
        [Authorize(Roles = Role.Coach)]
        [HttpPost("{builderId:length(24)}/meeting_reports")]
        public async Task<ActionResult<string>> CreateMeetingReport(string builderId, [FromBody] CreateMeetingReportModel createMeetingReportModel)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                await _buildersService.CreateMeetingReportAsync(currentUserId, builderId, createMeetingReportModel);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized($"You are not authorized: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Their was an error creating the meeting report: {e.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// (Builder,Admin) Update a builder
        /// </summary>
        /// <param name="builderId" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <param name="builderUpdateModel"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to update this builder</response>
        /// <response code="200">The builder has been successfully updated</response>
        [Authorize(Roles = Role.Admin + "," + Role.Builder)]
        [HttpPut("{builderId:length(24)}/update")]
        public async Task<IActionResult> UpdateBuilder(string builderId, [FromBody]BuilderUpdateModel builderUpdateModel)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    await _buildersService.UpdateBuilderFromAdminAsync(builderId, builderUpdateModel);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    await _buildersService.UpdateBuilderFromBuilderAsync(currentUserId, builderId, builderUpdateModel);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not authorized to update this builder: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't update the builder: {e.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// (Builder,Admin) Update a builder
        /// </summary>
        /// <param name="projectId" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <param name="builderId" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <param name="projectUpdateModel"></param>
        /// <returns></returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to update this project</response>
        /// <response code="200">The project has been successfully updated</response>
        [Authorize(Roles = Role.Admin + "," + Role.Builder)]
        [HttpPut("{builderId:length(24)}/projects/{projectId:length(24)}/update")]
        public async Task<IActionResult> UpdateProject(string builderId, string projectId, [FromBody] ProjectUpdateModel projectUpdateModel)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Admin))
                {
                    await _buildersService.UpdateProjectFromAdmin(projectId, projectUpdateModel);
                }
                else if (User.IsInRole(Role.Builder))
                {
                    await _buildersService.UpdateProjectFromBuilder(currentUserId, builderId, projectId, projectUpdateModel);
                }
                else
                {
                    return Forbid("You must be part of the Buildup program");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to update this project: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't update the project: {e.Message}");
            }

            return Ok();
        }

        /// <summary>
        /// (Admin) Refuse a builder
        /// </summary>
        /// <param name="builderId" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <returns></returns>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="200">The builder has been successfully refused</response>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("{builderId:length(24)}/refuse")]
        public async Task<IActionResult> RefuseBuilder(string builderId)
        {
            await _buildersService.RefuseBuilderAsync(builderId);

            return Ok();
        }

        /// <summary>
        /// (Builder) Sign the builder integration paper
        /// </summary>
        /// <param name="builderId" example="5f1fed8458c8ab093c4f77bf"></param>
        /// <returns>If the PDF was successfully generated</returns>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to sign</response>
        /// <response code="200">The paper was successfully signed</response>
        [Authorize(Roles = Role.Builder)]
        [HttpPut("{builderId:length(24)}/sign_integration")]
        public async Task<IActionResult> SignIntegration(string builderId)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                var sucess = await _buildersService.SignFicheIntegrationAsync(currentUserId, builderId);

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
                return BadRequest(e.Message);
            }

            return BadRequest("The PDF can't be generated");
        }

        /// <summary>
        /// (Builder) Mark a notification as read
        /// </summary>
        /// <param name="builderId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <param name="notificationId" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <response code="400">There was an error in the request</response>
        /// <response code="401">You don't have enough permissions</response>s
        /// <response code="403">You are not allowed to mark this notification as read</response>
        /// <response code="200">The request has been accepted</response>
        [Authorize(Roles = Role.Builder)]
        [HttpPut("{builderId:length(24)}/notifications/{notificationId:length(24)}/read")]
        public async Task<IActionResult> ReadNotification(string builderId, string notificationId)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (User.IsInRole(Role.Builder))
                {
                    await _notificationService.MakeBuilderNotificationReadAsync(builderId, notificationId);
                }
                else
                {
                    return Forbid("You must be a builder");
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid($"You are not allowed to mark this notification as read: {e.Message}");
            }
            catch (Exception e)
            {
                return BadRequest($"Can't mark the notification as read: {e.Message}");
            }

            return Ok();
        }

    }
}
