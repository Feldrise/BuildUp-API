using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.Pdf
{
    public class PdfIntegrationBuilder
    {
        /// <summary>
        /// The builder's first name
        /// </summary>
        /// <example>Anael</example>
        public string FirstName { get; set; }
        /// <summary>
        /// The builder's last name 
        /// </summary>
        /// <example>MEGRET</example>
        public string LastName { get; set; }
        /// <summary>
        /// The builder's birth date
        /// </summary>
        /// <example>2001-04-31T14:40:04.1351158+01:00</example>
        public DateTime Birthdate { get; set; }
        /// <summary>
        /// The user's birth place
        /// </summary>
        /// <example>Rennes</example>
        public string BirthPlace { get; set; }

        /// <summary>
        /// The user's email
        /// </summary>
        /// <example>admin@feldrise.com</example>
        public string Email { get; set; }
        /// <summary>
        /// The user's phone
        /// </summary>
        /// <example>+33652809335</example>
        public string Phone { get; set; }

        /// <summary>
        /// The user's city
        /// </summary>
        /// <example>Rennes</example>
        public string City { get; set; }
        /// <summary>
        /// The user's postal code
        /// </summary>
        /// <example>35000</example>
        public int PostalCode { get; set; }
        /// <summary>
        /// The user's address
        /// </summary>
        /// <example>17 Rue de La Rimaudière</example>
        public string Address { get; set; }


        /// <summary>
        /// The builder current situation
        /// </summary>
        /// <example>Etudiant</example>
        public string Situation { get; set; }

        /// <summary>
        /// Answer : Vos proches vous présentent comme quelqu’un :
        /// </summary>
        /// <example>Intelligente, passionnée</example>
        public string Keywords { get; set; }
        /// <summary>
        /// Answer : Donnez une phrase d'accroche pour vous
        /// </summary>
        public string Accroche { get; set; }
        
        /// <summary>
        /// The project's domain
        /// </summary>
        /// <example>Plantes, application mobile, Instagram</example>
        public string ProjectDomaine { get; set; }
        /// <summary>
        /// The project's name
        /// </summary>
        /// <example>Pluctis</example>
        public string ProjectName { get; set; }
        /// <summary>
        /// When the project was started
        /// </summary>
        /// <example>2020-03-01T10:00:04.1351158+01:00</example>
        public DateTime ProjectLaunchDate { get; set; }

        /// <summary>
        /// The project's description
        /// </summary>
        /// <example>Pluctis est un super projet visant a vous faire garder vos plantes en vie en...</example>
        public string ProjectDescription { get; set; }
        /// <summary>
        /// The project's team
        /// </summary>
        /// <example>Lucas, Louna, Iulia, Emma, Ethan, Noemie, moi</example>
        public string ProjectTeam { get; set; }

        /// <summary>
        /// Answer : Pourquoi souhaitez-vous intégrer le programme Build Up ?
        /// </summary>
        /// <example>Qu'il m'apprenne a faire du café</example>
        public string Expectation { get; set; }

        /// <summary>
        /// Answer : Quels objectifs souhaitez-vous atteindre au bout des 3 mois de programme ?
        /// </summary>
        /// <example>Avoir fait couler Google</example>
        public string Objectifs { get; set; }
    }
}
