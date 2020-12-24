using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.Coachs
{
    public class CoachUpdateModel
    {
        /// <summary>
        /// The coach's user id
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// the coach's card
        /// </summary>
        public byte[] CoachCard { get; set; }

        /// <summary>
        /// The coach current status. Can be : Waiting Validated, Refused
        /// </summary>
        /// <example>Waiting/Validated/Refused</example>
        [Required]
        public string Status { get; set; }
        /// <summary>
        /// The coach current step. Can be : Preselected, Meeting, Active, Stopped
        /// </summary>
        /// <example>Preselected/Meeting/Active/Stopped</example>
        [Required]
        public string Step { get; set; }

        /// <summary>
        /// The coach's department
        /// </summary>
        /// <example>35</example>
        [Required]
        public int Department { get; set; }

        /// <summary>
        /// The coach current situation
        /// </summary>
        /// <example>Entrepreneur</example>
        [Required]
        public string Situation { get; set; }
        /// <summary>
        /// The coach description
        /// </summary>
        /// <example>I'm an awesome coach</example>
        [Required]
        public string Description { get; set; }
    }
}
