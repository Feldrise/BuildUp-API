using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models
{
    public class CoachAssignmentModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string CoachId { get; set; }
    }
}
