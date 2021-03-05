using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.Pdf
{
    public class PdfBuilderCard
    {
        /// <summary>
        /// The builder's first name
        /// </summary>
        /// <example>Elisa</example>
        public string FirstName { get; set; }
        /// <summary>
        /// The builder's last name 
        /// </summary>
        /// <example>AUFFRAY</example>
        public string LastName { get; set; }

        /// <summary>
        /// The builder's birth date
        /// </summary>
        /// <example>2001-06-20T14:40:04.1351158+01:00</example>
        public DateTime Birthdate { get; set; }
        /// <summary>
        /// The builder's validity date
        /// </summary>
        /// <example>2001-06-20T14:40:04.1351158+01:00</example>
        public DateTime ValidityDate { get; set; }
    }
}
