using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.Notification
{
    public class CoachNotification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string CoachId { get; set; }

        /// <summary>
        /// The content of the notification
        /// </summary>
        /// <example>This is a notification</example>
        public string Content { get; set; }

        /// <summary>
        /// Indicate if the notification has been seen
        /// </summary>
        /// <example>false</example>
        public bool Seen { get; set; }
    }
}
