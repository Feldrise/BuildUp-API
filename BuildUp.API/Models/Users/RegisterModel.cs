using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.Users
{
    public class RegisterModel
    {
        /// <summary>
        /// The user's first name
        /// </summary>
        /// <example>Victor</example>
        [Required]
        public string FirstName { get; set; }
        /// <summary>
        /// The user's last name
        /// </summary>
        /// <example>DENIS</example>
        [Required]
        public string LastName { get; set; }
        /// <summary>
        /// The user's bithdate
        /// </summary>
        /// <example>2001-08-15T14:40:04.1351158+01:00</example>
        [Required]
        public DateTime Birthdate { get; set; }
        /// <summary>
        /// The user's birth place
        /// </summary>
        /// <example>Rennes</example>
        public string BirthPlace { get; set; }

        /// <summary>
        /// The uers's email
        /// </summary>
        /// <example>admin@feldrise.com</example>
        [Required]
        public string Email { get; set; }
        /// <summary>
        /// The user's phone
        /// </summary>
        /// <example>+33652809335</example>
        public string Phone { get; set; }
        /// <summary>
        /// The user's Discord tag
        /// </summary>
        /// <example>Feldrise#8497</example>
        [Required]
        public string DiscordTag { get; set; }
        /// <summary>
        /// The user's username
        /// </summary>
        /// <example>Feldrise</example>
        [Required]
        public string Username { get; set; }


        /// <summary>
        /// The user's department
        /// </summary>
        /// <example>35</example>
        [Required]
        public int Department { get; set; }
        /// <summary>
        /// The user's city
        /// </summary>
        /// <example>La Chapelle-Thouarault</example>
        [Required]
        public string City { get; set; }
        /// <summary>
        /// The user's postal code
        /// </summary>
        /// <example>35590</example>
        [Required]
        public int PostalCode { get; set; }
        /// <summary>
        /// The user's address
        /// </summary>
        /// <example>17 Rue de La Rimaudière</example>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// The user's role
        /// </summary>
        /// <example>Builder/Coach/Builder</example>
        [Required]
        public string Role { get; set; }
    }
}
