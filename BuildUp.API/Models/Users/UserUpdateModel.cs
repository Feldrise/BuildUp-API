using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.Users
{
    public class UserUpdateModel
    {
        /// <summary>
        /// Represent the profile picture
        /// </summary>
        public byte[] ProfilePicture { get; set; }

        /// <summary>
        /// The user's first name
        /// </summary>
        /// <example>Victor</example>
        public string FirstName { get; set; }
        /// <summary>
        /// The user's last name
        /// </summary>
        /// <example>DENIS</example>
        public string LastName { get; set; }
        /// <summary>
        /// The user's birthdate
        /// </summary>
        /// <example>2001-08-15T14:40:04.1351158+01:00</example>
        public DateTime Birthdate { get; set; }

        /// <summary>
        /// The user's email
        /// </summary>
        /// <example>admin@feldrise.com</example>
        public string Email { get; set; }
        /// <summary>
        /// The user's Discord tag
        /// </summary>
        /// <example>Feldrise#8497</example>
        public string DiscordTag { get; set; }

        /// <summary>
        /// A new password 
        /// </summary>
        public string Password { get; set; }
    }
}
