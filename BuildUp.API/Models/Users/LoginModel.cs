using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.Users
{
    public class LoginModel
    {
        /// <summary>
        /// Could be either the email or the actual username
        /// </summary>
        /// <example>Feldrise</example>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// The user password
        /// </summary>
        /// <example>MySecurePassword</example>
        [Required]
        public string Password { get; set; }
    }
}
