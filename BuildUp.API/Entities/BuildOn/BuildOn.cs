using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.BuildOn
{
    public class BuildOn
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Represent the image file ID 
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example> 
        [BsonRepresentation(BsonType.ObjectId)]
        public string ImageId { get; set; }

        /// <summary>
        /// The number of the buildon
        /// </summary>
        /// <example>1</example>
        [JsonIgnore]
        public int Index { get; set; }

        /// <summary>
        /// The buildon name
        /// </summary>
        /// <example>Buildon 1</example>
        public string Name { get; set; }
        /// <summary>
        /// The buildon description
        /// </summary>
        /// <example>Dans ce buildon vous allez...</example>
        public string Description { get; set; }
    }
}
