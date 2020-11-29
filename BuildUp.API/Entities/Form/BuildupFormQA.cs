using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.Form
{
    public class BuildupFormQA
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string FormId { get; set; }

        [JsonIgnore]
        public int Index { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
