using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.BuildOn
{
    public class BuildOnStepManageModel
    {
        /// <summary>
        /// The id of the step in the database
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Represent the image file 
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// The name of the step
        /// </summary>
        /// <example>Clarifier mon idée</example>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// The description of the step
        /// </summary>
        /// <example>Dans cet étape vous allez...</example>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// The description of the proof needed to validate the buildon
        /// </summary>
        /// <example>Vous devrez rendre...</example>
        [Required]
        public string ProofDescription { get; set; }
    }
}
