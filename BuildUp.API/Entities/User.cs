using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildUp.API.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The profil picture file ID
        /// </summary>
        /// <example>5f1fe90a58c8ab093c4f772a</example>
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProfilePictureId { get; set; }

        /// <summary>
        /// The user's first name
        /// </summary>
        /// <example>Victor</example>
        public string FirstName { get; set; }
        /// <summary>
        /// The user's last name
        /// </summary>
        /// <example>DENIS</example>
        public string LastName { get; set; }
        /// <summary>
        /// The user's birthdate
        /// </summary>
        /// <example>2001-08-15T14:40:04.1351158+01:00</example>
        public DateTime Birthdate { get; set; }
        /// <summary>
        /// The user's birth place
        /// </summary>
        /// <example>Rennes</example>
        public string BirthPlace { get; set; }

        /// <summary>
        /// The user's email
        /// </summary>
        /// <example>admin@feldrise.com</example>
        public string Email { get; set; }
        /// <summary>
        /// The user's phone
        /// </summary>
        /// <example>+33652809335</example>
        public string Phone { get; set; }

        /// <summary>
        /// The user's Discord tag
        /// </summary>
        /// <example>Feldrise#8497</example>
        public string DiscordTag { get; set; }
        /// <summary>
        /// The user's username
        /// </summary>
        /// <example>Feldrise</example>
        public string Username { get; set; }


        /// <summary>
        /// The user's department
        /// </summary>
        /// <example>35</example>
        public int Department { get; set; }
        /// <summary>
        /// The user's city
        /// </summary>
        /// <example>La Chapelle-Thouarault</example>
        public string City { get; set; }
        /// <summary>
        /// The user's postal code
        /// </summary>
        /// <example>35590</example>
        public int PostalCode { get; set; }
        /// <summary>
        /// The user's address
        /// </summary>
        /// <example>17 Rue de La Rimaudière</example>
        public string Address { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// The user role. Could be Builder, Coach or Admin
        /// </summary>
        /// <example>Builder</example>
        public string Role { get; set; }
        /// <summary>
        /// The user connection token. Only returned on login
        /// </summary>
        public string Token { get; set; }
    }
}
