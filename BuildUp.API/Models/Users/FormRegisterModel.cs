using BuildUp.API.Entities.Form;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.Users
{
    public class FormRegisterModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string DiscordTag { get; set; }
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }

        [Required]
        public List<BuildupFormQA> FormQAs { get; set; }
    }
}
