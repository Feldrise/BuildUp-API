using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities
{
    public class Coach
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public string Status { get; set; }
        public string Step { get; set; }

        public int Department { get; set; }

        public string Situation { get; set; }
        public string Description { get; set; }
    }
}
