using BuildUp.API.Entities.Form;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.Builders
{
    public class BuilderRegisterModel
    {
        /// <summary>
        /// The builder's user id
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// The builder current situation
        /// </summary>
        /// <example>Student</example>
        [Required]
        public string Situation { get; set; }
        /// <summary>
        /// The builder description
        /// </summary>
        /// <example>I'm an awesome builder</example>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// The forms answers
        /// </summary>
        /// <example>
        /// [
        ///     {
        ///         "question": "My First Question",
        ///         "answer": "My First Answer"
        ///     },
        ///     {
        ///         "question": "My Second Question",
        ///         "answer": "My Second Answer"
        ///     }
        /// ]
        /// </example>
        [Required]
        public List<BuildupFormQA> FormQAs { get; set; }
    }
}
