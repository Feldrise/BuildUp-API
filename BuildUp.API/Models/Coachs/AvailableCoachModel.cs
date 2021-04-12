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
        /// The user's linkedin
        /// </summary>
        /// <example>victor-denis-2a351a170</example>
        public string LinkedIn { get; set; }

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
        /// Answer: Quelles sont vos compétences clés ?
        /// </summary>
        public string Competences { get; set; }

        /// <summary>
        /// All questions to show to the builder
        /// </summary>
        public List<string> Questions { get; set; }
        /// <summary>
        /// All the answers for the questions to show to the builder
        /// </summary>
        public List<string> Answers { get; set; }
    }
}
