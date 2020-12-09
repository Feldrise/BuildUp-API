using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.Projects
{
    public class ProjectUpdateModel
    {
        /// <summary>
        /// The project's name
        /// </summary>
        /// <example>Pluctis</example>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// The project's categorye
        /// </summary>
        /// <example>Agroalimentaire</example>
        public string Categorie { get; set; }
        /// <summary>
        /// The project's description
        /// </summary>
        /// <example>Pluctis is the tool to never let your plants die again</example>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Keywords that represent the project
        /// </summary>
        /// <example>Plantes, application, tool</example>
        [Required]
        public string Keywords { get; set; }
        /// <summary>
        /// The team behind the project
        /// </summary>
        /// <example>I'm alone but supported by my friends!</example>
        [Required]
        public string Team { get; set; }

        /// <summary>
        /// The project launch date
        /// </summary>
        /// <example>2020-04-20T14:40:04.1351158+01:00</example>
        [Required]
        public DateTime LaunchDate { get; set; }
        /// <summary>
        /// Indicate if the project is lucratif or not
        /// </summary>
        /// <example>true</example>
        public bool IsLucratif { get; set; }
        /// <summary>
        /// Indicate if the project is declared or not
        /// </summary>
        /// <example>true</example>
        public bool IsDeclared { get; set; }
    }
}
