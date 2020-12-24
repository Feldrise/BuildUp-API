using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities
{
    public class Coach
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The date whene the coach canidated
        /// </summary>
        /// <example><example>2020-08-15T14:40:04.1351158+01:00</example></example>
        public DateTime CandidatingDate { get; set; }


        /// <summary>
        /// The id of the coach's card
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string CoachCardId { get; set; }

        /// <summary>
        /// The coach's user id
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        /// <summary>
        /// The coach current status. Can be : Waiting Validated, Refused
        /// </summary>
        /// <example>Waiting/Validated/Refused</example>
        public string Status { get; set; }
        /// <summary>
        /// The coach current step. Can be : Preselected, Meeting, Active, Stopped
        /// </summary>
        /// <example>Preselected/Meeting/Active/Stopped</example>
        public string Step { get; set; }

        /// <summary>
        /// The coach's department
        /// </summary>
        /// <example>35</example>
        public int Department { get; set; }

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
    }
}
