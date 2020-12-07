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

        public UsersController(IUsersService usersService)
        {
            _userService = usersService;
        }

        /// <summary>
        /// Get the profile picture of the user
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
    }
}
