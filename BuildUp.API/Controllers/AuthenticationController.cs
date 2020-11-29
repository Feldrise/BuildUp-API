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

        [HttpGet("test")]
        [AllowAnonymous]
        public ActionResult<string> test()
        {
            return Ok(DateTime.Now);
        }

        /// <summary>
        /// Login a user
        /// </summary>
        /// <remarks>
        /// NOTE: The username can be both the email or the actual username
        /// 
        /// Sample request:
        ///
        ///     POST /login
        ///     {
        ///         "username": "Feldrise",
        ///         "password": "MySecurePassword"
        ///     } 
        /// </remarks>
        /// <param name="loginModel">Represent the username and the password</param>
        /// <returns>The user with all informations (especially the authentication token)</returns>
        /// <response code="400">If the user doesn't exist or the password doesn't match the username</response>
        /// <response code="201">Return the logged user infos</response>
        [AllowAnonymous]
        [HttpPost("login")]
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
        /// Register a user as a coach or builder
        /// </summary>
        /// <remarks>
        /// NOTE: The role 
        /// Sample request:
        ///
        ///     POST /register
        ///     {
        ///         "firstName": "Victor",
        ///         "lastName": "DENIS",
        ///         "birthdate": "2001-08-15 14:40:36.309721",
        ///         "email": "admin@feldrise.com",
        ///         "discordTag": "Feldrise#8497",
        ///         "username": "Feldrise",
        ///         "password": "MySecurePassword",
        ///         "role": "Coach"
        ///     }   
        /// </remarks>
        /// <param name="registerModel"></param>
        /// <returns>The registered user ID</returns>
        /// <response code="400">If the can't be registered</response>
        /// <response code="201">Return the registered user id</response>
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
        /// Register a user as an admin
        /// </summary>
        /// <remarks>
        /// NOTE: The role 
        /// Sample request:
        ///
        ///     POST /register
        ///     {
        ///         "firstName": "Victor",
        ///         "lastName": "DENIS",
        ///         "birthdate": "2001-08-15 14:40:36.309721",
        ///         "email": "admin@feldrise.com",
        ///         "discordTag": "Feldrise#8497",
        ///         "username": "Feldrise",
        ///         "password": "MySecurePassword",
        ///         "role": "Admin"
        ///     }   
        /// </remarks>
        /// <param name="registerModel"></param>
        /// <returns>The registered user ID</returns>
        /// <response code="400">If the can't be registered</response>
        /// <response code="400">If you try to do this as not admin</response>s
        /// <response code="201">Return the registered user id</response>
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
