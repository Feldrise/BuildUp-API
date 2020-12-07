using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.BuildOn
{
    public class BuildOnStep
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
        /// The buildon wich this step is in
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string BuildOnId { get; set; }

        /// <summary>
        /// The index of the step in the buildon
        /// </summary>
        /// <example>1</example>
        [JsonIgnore]
        public int Index { get; set; }

        /// <summary>
        /// The name of the step
        /// </summary>
        /// <example>Clarifier mon idée</example>
        public string Name { get; set; }
        /// <summary>
        /// The description of the step
        /// </summary>
        /// <example>Dans cet étape vous allez...</example>
        public string Description { get; set; }

        /// <summary>
        /// The type of returning expected for buildon
        /// </summary>
        /// <example>File/External/Comment</example>
        public string ReturningType { get; set; }
        /// <summary>
        /// The description of the proof needed to validate the buildon
        /// </summary>
        /// <example>Vous devrez rendre...</example>
        public string ReturningDescription { get; set; }
        /// <summary>
        /// A link to show to the builder
        /// </summary>
        /// <example>https://form.google.com/...</example>
        public string ReturningLink { get; set; }
    }
}
