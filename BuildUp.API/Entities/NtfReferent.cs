using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities
{
    public class NtfReferent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The first name of the NTF referent
        /// </summary>
        /// <example>Victor</example>
        public string FirstName { get; set; }
        /// <summary>
        /// The last name of the NTF referent
        /// </summary>
        /// <example>DENIS</example>
        public string LastName { get; set; }
        /// <summary>
        /// The email of the NTF referent
        /// </summary>
        /// <example>contact@feldrise.com</example>
        public string Email { get; set; }
        /// <summary>
        /// The Discord tag of NTF referent
        /// </summary>
        /// <example>Feldrise#8497</example>
        public string DiscordTag { get; set; }

        /// <summary>
        /// The referent competences 
        /// </summary>
        /// <example>Stratégie d’innovation, Acquisition, SEO, Design, HTML, CSS, JS</example>
        public string Competence { get; set; }
    }
}
