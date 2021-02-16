using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.Notification.CoachRequest
{
    public class CoachRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string CoachId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string BuilderId { get; set; }

        /// <summary>
        /// The current proof status
        /// </summary>
        /// <example>Validated/Waiting/Refused</example>
        public string Status { get; set; }
    }
}
