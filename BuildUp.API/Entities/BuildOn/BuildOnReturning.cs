using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.BuildOn
{
    public class BuildOnReturning
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The project who send the proof
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProjectId { get; set; }

        /// <summary>
        /// The build-on step the returning correspond to
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string BuildOnStepId { get; set; }

        /// <summary>
        /// The returning type
        /// </summary>
        /// <example>File/External/Comment</example>
        public string Type { get; set; }

        /// <summary>
        /// The current proof status
        /// </summary>
        /// <example>Validated/Waiting/Refused</example>
        public string Status { get; set; }

        /// <summary>
        /// The file id in the database
        /// </summary>
        /// <example>hello.zip</example>
        public string FileName { get; set; }

        /// <summary>
        /// The file id in the database
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        public string FileId { get; set; }

        /// <summary>
        /// The comment associated with the returning
        /// </summary>
        /// <example>J'ai fini l'étape !</example>
        public string Comment { get; set; }

    }
}
