using BuildUp.API.Entities;
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
    [Authorize]
    [Route("buildup/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// (*) Login a user
        /// </summary>
        /// <remarks>
        /// The username can be both the email or the actual username
        /// </remarks>
        /// <param name="loginModel">Represent the username and the password</param>
        /// <returns>The user with all informations (especially the authentication token)</returns>
        /// <response code="400">The user doesn't exist or the password doesn't match the username</response>
        /// <response code="200">Return the logged user infos</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> Login([FromBody]LoginModel loginModel)
        {
            var user = await _authenticationService.LoginAsync(loginModel.Username, loginModel.Password);

            if (user == null)
            {
                return BadRequest("Username or password incorrect");
            }

            return Ok(user);
        }

        /// <summary>
        /// (*) Register a user as a coach or builder
        /// </summary>
        /// <remarks>
        /// The role must be "Coach" or "Builder"
        /// </remarks>
        /// <param name="registerModel"></param>
        /// <returns>The registered user ID</returns>
        /// <response code="400">The user can't be registered</response>
        /// <response code="200">Return the registered user id</response>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody]RegisterModel registerModel)
        {
            string userId;
            try
            {
                userId = await _authenticationService.RegisterAsync(registerModel);
            }
            catch (Exception e)
            {
                return BadRequest($"Can't regster the user: {e.Message}");
            }

            return Ok(userId);
        }

        /// <summary>
        /// (*) Register a user as a coach or builder with form question and answer
        /// </summary>
        /// <remarks>
        /// The role must be "Coach" or "Builder"
        /// </remarks>
        /// <param name="formRegisterModel"></param>
        /// <returns>The registered user ID</returns>
        /// <response code="400">The user can't be registered</response>
        /// <response code="200">Return the registered user id</response>
        [AllowAnonymous]
        [HttpPost("register/with_form")]
        public async Task<ActionResult<string>> RegisterWithForm([FromBody]FormRegisterModel formRegisterModel)
        {
            string userId;
            try
            {
                userId = await _authenticationService.RegisterWithFormAsync(formRegisterModel);
            }
            catch (Exception e)
            {
                return BadRequest($"Can't regster the user: {e.Message}");
            }

            return Ok(userId);
        }

        /// <summary>
        /// (Admin) Register a user as an admin
        /// </summary>
        /// <remarks>
        /// The role  must be admin
        /// </remarks>
        /// <param name="registerModel"></param>
        /// <returns>The registered user ID</returns>
        /// <response code="400">The user can't be registered</response>
        /// <response code="401">You are not logged in</response>
        /// <response code="403">You are not an admin</response>
        /// <response code="200">Return the registered user id</response>
        [Authorize(Roles = Role.Admin)]
        [HttpPost("register/admin")]
        public async Task<ActionResult<string>> RegisterAdmin([FromBody] RegisterModel registerModel)
        {
            string userId;
            try
            {
                userId = await _authenticationService.RegisterAdminAsync(registerModel);
            }
            catch (Exception e)
            {
                return BadRequest($"Can't regster the admin: {e.Message}");
            }

            return Ok(userId);
        }
    }
}
