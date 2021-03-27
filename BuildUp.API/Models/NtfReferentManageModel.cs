using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models
{
    public class NtfReferentManageModel
    {
        /// <summary>
        /// The first name of the NTF referent
        /// </summary>
        /// <example>Victor</example>
        [Required]
        public string FirstName { get; set; }
        /// <summary>
        /// The last name of the NTF referent
        /// </summary>
        /// <example>DENIS</example>
        [Required]
        public string LastName { get; set; }
        /// <summary>
        /// The email of the NTF referent
        /// </summary>
        /// <example>contact@feldrise.com</example>
        [Required]
        public string Email { get; set; }
        /// <summary>
        /// The Discord tag of NTF referent
        /// </summary>
        /// <example>Feldrise#8497</example>
        [Required]
        public string DiscordTag { get; set; }

        /// <summary>
        /// The referent competences 
        /// </summary>
        /// <example>Stratégie d’innovation, Acquisition, SEO, Design, HTML, CSS, JS</example>
        public string Competence { get; set; }
    }
}
