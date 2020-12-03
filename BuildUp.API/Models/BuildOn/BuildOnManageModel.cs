using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.BuildOn
{
    public class BuildOnManageModel
    {
        /// <summary>
        /// The id of the step in the database
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Represent the image
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// The buildon name
        /// </summary>
        /// <example>Buildon 1</example>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// The buildon description
        /// </summary>
        /// <example>Dans ce buildon vous allez...</example>
        [Required]
        public string Description { get; set; }
    }
}
