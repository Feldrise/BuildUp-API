using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The builder who own the project
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string BuilderId { get; set; }

        /// <summary>
        /// The project's name
        /// </summary>
        /// <example>Pluctis</example>
        public string Name { get; set; }
        /// <summary>
        /// The project's description
        /// </summary>
        /// <example>Pluctis is the tool to never let your plants die again</example>
        public string Description { get; set; }
        /// <summary>
        /// Keywords that represent the project
        /// </summary>
        /// <example>Plantes, application, tool</example>
        public string Keywords { get; set; }
        /// <summary>
        /// The team behind the project
        /// </summary>
        /// <example>I'm alone but supported by my friends!</example>
        public string Team { get; set; }

        /// <summary>
        /// The project launch date
        /// </summary>
        /// <example>2020-04-20T14:40:04.1351158+01:00</example>
        public DateTime LaunchDate { get; set; }
        /// <summary>
        /// Indicate if the project is lucratif or not
        /// </summary>
        /// <example>true</example>
        public bool IsLucratif { get; set; }

        /// <summary>
        /// The id of the current buildon for the project
        /// </summary>
        /// <example>5f1fe90a58c8att73c4f882b</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string CurrentBuildOn { get; set; }

        /// <summary>
        /// The id of the current step in the current buildon
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string CurrentBuildOnStep { get; set; }

    }
}
