using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.Pdf
{
    public class PdfAttestationMineur
    {
        /// <summary>
        /// The legal tutor first name
        /// </summary>
        /// <example>Victor</example>
        [Required]
        public string FistName { get; set; }
        /// <summary>
        /// The legal tutor last name
        /// </summary>
        /// <example>DENIS</example>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// The legal tutor address line 1
        /// </summary>
        /// <example>17 Rue de La Rimaudière</example>
        [Required]
        public string AddressLine1 { get; set; }
        /// <summary>
        /// The legal tutor address line 2
        /// </summary>
        /// <example>Appartement C24</example> 
        public string AddressLine2 { get; set; }
        /// <summary>
        /// The legal tutor city
        /// </summary>
        /// <example>La Chapelle-Thouarault</example>
        [Required]
        public string City { get; set; }
        /// <summary>
        /// The legal tutor postal code
        /// </summary>
        /// <example>35590</example>
        [Required]
        public string PostalCode { get; set; }

        /// <summary>
        /// The legal tutor email
        /// </summary>
        /// <example>admin@feldrise.com</example>
        [Required]
        public string Email { get; set; }
        /// <summary>
        /// The legal tutor phone number
        /// </summary>
        /// <example>0652809335</example>
        [Required]
        public string Phone { get; set; }

        /// <summary>
        /// The place where the attestation was made
        /// </summary>
        /// <example>Rennes</example>
        [Required]
        public string MadeAt { get; set; }
        /// <summary>
        /// The date when the attestation was made
        /// </summary>
        /// <example>15/12/2020</example>
        [Required]
        public string MadeDate { get; set; }
    }
}
