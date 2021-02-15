using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.Coachs
{
    public class AvailableCoachModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The user ID
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

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
        /// The coach current situation
        /// </summary>
        /// <example>Entrepreneur</example>
        public string Situation { get; set; }
        /// <summary>
        /// The coach description
        /// </summary>
        /// <example>I'm an awesome coach</example>
        public string Description { get; set; }

        /// <summary>
        /// Answer : Quelles sont vos compétences ?
        /// </summary>
        public string Competences { get; set; }
        /// <summary>
        /// Answer : Quelles sont, selon vous, les principales perspectives pour qu’un projet fonctionne ?
        /// </summary>
        public string Perspectives { get; set; }
        /// <summary>
        /// Answer : Comment définissez-vous le rôle de Coach ?
        /// </summary>
        public string CoachDefinition { get; set; }
    }
}
