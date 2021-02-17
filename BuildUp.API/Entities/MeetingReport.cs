using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities
{
    public class MeetingReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The builder ID
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string BuilderId { get; set; }

        /// <summary>
        /// The coach ID
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string CoachId { get; set; }

        /// <summary>
        /// The date when the repport was written
        /// </summary>
        /// <example>2001-08-15T14:40:04.1351158+01:00</example>
        public DateTime Date { get; set; }

        /// <summary>
        /// The date planned for the next meeting
        /// </summary>
        /// <example>2001-08-15T14:40:04.1351158+01:00</example>
        public DateTime NextMeetingDate { get; set; }

        /// <summary>
        /// What were the objectives for the meeting
        /// </summary>
        /// <example>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est.</example>
        public string Objectif { get; set; }

        /// <summary>
        /// Whate are the progress constated during the meeting
        /// </summary>
        /// <example>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est.</example>
        public string Evolution { get; set; }

        /// <summary>
        /// What is planned for the next meeting
        /// </summary>
        /// <example>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est.</example>
        public string WhatsNext { get; set; }

        /// <summary>
        /// Some comments made by the coach
        /// </summary>
        /// <example>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est.</example>
        public string Comments { get; set; }
    }
}
