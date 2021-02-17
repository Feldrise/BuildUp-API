using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.MeetingReports
{
    public class CreateMeetingReportModel
    {
        /// <summary>
        /// The date planned for the next meeting
        /// </summary>
        /// <example>2001-08-15T14:40:04.1351158+01:00</example>
        [Required]
        public DateTime NextMeetingDate { get; set; }

        /// <summary>
        /// What were the objectives for the meeting
        /// </summary>
        /// <example>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est.</example>
        [Required]
        public string Objectif { get; set; }

        /// <summary>
        /// Whate are the progress constated during the meeting
        /// </summary>
        /// <example>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vel est quam. Donec sapien ex, convallis eu magna vel, efficitur molestie est.</example>
        [Required]
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
