using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.Pdf
{
    public class PdfIntegrationCoach
    {
        /// <summary>
        /// The coach's first name
        /// </summary>
        /// <example>Elisa</example>
        public string FirstName { get; set; }
        /// <summary>
        /// The coach's last name 
        /// </summary>
        /// <example>AUFFRAY</example>
        public string LastName { get; set; }
        /// <summary>
        /// The coach's birth date
        /// </summary>
        /// <example>2001-06-20T14:40:04.1351158+01:00</example>
        public DateTime Birthdate { get; set; }
        /// <summary>
        /// The user's birth place
        /// </summary>
        /// <example>Quimper</example>
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
        /// <example>La Chapelle-Thouarault</example>
        public string City { get; set; }
        /// <summary>
        /// The user's postal code
        /// </summary>
        /// <example>35590</example>
        public int PostalCode { get; set; }
        /// <summary>
        /// The user's address
        /// </summary>
        /// <example>17 Rue de La Rimaudière</example>
        public string Address { get; set; }


        /// <summary>
        /// The coach current situation
        /// </summary>
        /// <example>Etudiant</example>
        public string Situation { get; set; }

        /// <summary>
        /// Answer : Quelles sont les mots clés qui vous décrivent ?
        /// </summary>
        /// <example>Intelligente, passionnée</example>
        public string Keywords { get; set; }
        /// <summary>
        /// Answer : Quel est votre experience ?
        /// </summary>
        public string Experience { get; set; }
        /// <summary>
        /// Answer : Donnez une phrase d'accroche pour vous
        /// </summary>
        public string Accroche { get; set; }
        /// <summary>
        /// Answer : Quel serait le Builder idéal pour vous ?
        /// </summary>
        public string IdealBuilder { get; set; }
        /// <summary>
        /// Answer : Quels objectifs souhaitez-vous que votre Builder atteignent au bout des 3 mois ?
        /// </summary>
        public string Objectifs { get; set; }

    }
}
