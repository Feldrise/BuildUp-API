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
        /// The name of the NTF referent
        /// </summary>
        /// <example>Victor DENIS</example>
        public string Name { get; set; }
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
    }
}
