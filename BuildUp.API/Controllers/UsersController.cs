using BuildUp.API.Entities;
using BuildUp.API.Entities.Notification;
using BuildUp.API.Models.Users;
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
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly INotificationService _notificationService;

        public UsersController(IUsersService usersService, INotificationService notificationService)
        {
            _userService = usersService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// (Builder,Coach,Admin) Get the profile picture of the user
        /// </summary>
        /// <param name="id" example="5f1fe90a58c8ab093c4f772a"></param>
        /// <returns></returns>
        /// <response code="401">You are not allowed to view user's profile picture</response>
        /// <response code="404">The profile_picture was not found</response>
        /// <response code="200">Return user's profile picture</response>
        [HttpGet("{id:length(24)}/profile_picture")]
        public async Task<ActionResult<byte[]>> GetProfilePicture(string id)
        {
            var image = await _userService.GetProfilePicture(id);

            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }

        /// <summary>
        /// (admin) Notify all users
        /// </summary>
        /// <param name="notification">The notification</param>
        /// <returns></returns>
        /// <response code="401">You must be an admin</response>
        /// <response code="200">The notification was successfully send</response>
        [Authorize(Roles = Role.Admin)]
        [HttpPost("notify/all")]
        public async Task<IActionResult> NotifyAllUser([FromBody] UserNotification notification)
        {
            await _notificationService.NotifyAllAsync(notification.Content);

            return Ok();
        }

        /// <summary>
        /// (Builder,Coach,Admin) Update a user. Only admin can edit every users
        /// </summary>
        /// <param name="id" exemple="5f1fe90a58c8ab093c4f772a"></param>
        /// <param name="userUpdateModel"></param>
        /// <returns></returns>
        /// <response code="400">Their is an error in the request</response>
        /// <response code="403">You are not allowed to edit this user</response>
        /// <response code="200">The user has been updated</response>
        [HttpPut("{id:length(24)}/update")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody]UserUpdateModel userUpdateModel)
        {
            var currentUserId = User.Identity.Name;

            try
            {
                if (!User.IsInRole(Role.Admin) && currentUserId != id)
                {
                    return Forbid("You can't edit that user");
                }

                await _userService.UpdateUserAsync(id, userUpdateModel);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"Can't update the user: {e.Message}");
            }
        }
    }
}
