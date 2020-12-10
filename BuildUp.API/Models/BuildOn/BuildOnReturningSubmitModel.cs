using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.BuildOn
{
    public class BuildOnReturningSubmitModel
    {
        /// <summary>
        /// The build-on step the returning correspond to
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string BuildOnStepId { get; set; }

        /// <summary>
        /// The returning type
        /// </summary>
        /// <example>File/External/Comment</example>
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// The current proof status
        /// </summary>
        /// <example>my_file.zip</example>
        public string FileName { get; set; }

        /// <summary>
        /// The bytes of the file to upload to the
        /// server
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        /// The comment associated with the returning
        /// </summary>
        /// <example>J'ai fini l'étape !</example>
        public string Comment { get; set; }
    }
}
