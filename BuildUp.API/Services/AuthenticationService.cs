using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
using BuildUp.API.Models.Users;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BuildUp.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMongoCollection<User> _users;

        private readonly IBuildupSettings _buildupSettings;

        private readonly INotificationService _notificationService;
        public AuthenticationService(IMongoSettings mongoSettings, IBuildupSettings buildupSettings, INotificationService notificationService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>("users");

            _buildupSettings = buildupSettings;

            _notificationService = notificationService;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = await (
                await _users.FindAsync(
                    user => user.Email == username ||
                    user.Username == username
                )
            ).FirstOrDefaultAsync();

            if (user == null) { return null; }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            // authentication successful so generate jwt token
            user.Token = TokenForUser(user);

            return user;
        }

        public Task<string> RegisterAsync(RegisterModel userRegister)
        {
            // Basic checks
            if (userRegister.Role == Role.Admin) throw new ArgumentException("Only admins can register admins", "userRegister.Role");
            if (userRegister.Role != Role.Builder && userRegister.Role != Role.Coach) throw new ArgumentException("You can only register builders and coachs", "userRegister.Role");

            return RegisterToDatabaseAsync(userRegister);
        }

        public Task<string> RegisterAdminAsync(RegisterModel userRegister)
        {
            // Basic checks
            if (userRegister.Role != Role.Admin) throw new ArgumentException("You did not registerd an admin", "userRegister.Role");

            return RegisterToDatabaseAsync(userRegister);
        }

        private async Task<string> RegisterToDatabaseAsync(RegisterModel userRegister)
        {
            // Basic cheks
            if (string.IsNullOrWhiteSpace(userRegister.Email)) throw new Exception("You must provide an email");
            if (string.IsNullOrWhiteSpace(userRegister.Username)) throw new Exception("You must provide a username");
            if (UserExist(userRegister.Email, userRegister.Username)) throw new Exception("The email or the username is already in use");

            string password = GeneratePassword();

            // Password stuff, to ensure we never have clear password stored
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            User databaseUser = new User()
            {
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
                Birthdate = userRegister.Birthdate,

                Email = userRegister.Email,
                DiscordTag = userRegister.DiscordTag,
                Username = userRegister.Username,

                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = passwordSalt,

                Role = userRegister.Role
            };

            await _users.InsertOneAsync(databaseUser);
            await _notificationService.NotifieAccountCreationAsync(userRegister, password);

            return databaseUser.Id;
        }
        
        private bool UserExist(string email, string username)
        {
            return _users.AsQueryable<User>().Any(user =>
                user.Email == email ||
                user.Username == username
            );
        }

        private string GeneratePassword()
        {
            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
            };
            
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            for (int i = chars.Count; i < 10 || chars.Distinct().Count() < 8; ++i)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                
                chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, string storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordSalt");

            var storedHashBytes = Convert.FromBase64String(storedHash);

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; ++i)
                {
                    if (computedHash[i] != storedHashBytes[i]) return false;
                }
            }

            return true;
        }

        private string TokenForUser(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_buildupSettings.ApiSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
