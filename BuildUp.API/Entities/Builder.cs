using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities
{
    public class Builder
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The date whene the builder canidated
        /// </summary>
        /// <example><example>2020-08-15T14:40:04.1351158+01:00</example></example>
        public DateTime CandidatingDate { get; set; }


        /// <summary>
        /// The builder's user id
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        /// <summary>
        /// The builder's coach id
        /// </summary>
        /// <example>5f1fed8458c8ab093c4f77bf</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string CoachId { get; set; }

        /// <summary>
        /// The builder current status. Can be : Waiting Validated, Refused
        /// </summary>
        /// <example>Waiting/Validated/Refused</example>
        public string Status { get; set; }
        /// <summary>
        /// The builder current step. Can be : Preselected, AdminMeeting, CoachMeeting, Active, Finished, Abandoned
        /// </summary>
        /// <example>Preselected/AdminMeeting/CoachMeeting/Active/Finished/Abandoned</example>
        public string Step { get; set; }

        /// <summary>
        /// The builder's department
        /// </summary>
        /// <example>35</example>
        public int Department { get; set; }

        /// <summary>
        /// The builder current situation
        /// </summary>
        /// <example>Student</example>
        public string Situation { get; set; }
        /// <summary>
        /// The builder description
        /// </summary>
        /// <example>I'm an awesome builder</example>
        public string Description { get; set; }
    }
}
