using BuildUp.API.Entities;
using BuildUp.API.Models.Users;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services
{
    public class UsersService : IUsersService
    {
        private readonly IMongoCollection<User> _users;

        private readonly IFilesService _filesService;
        private readonly IAuthenticationService _authenticationService;

        public UsersService(IMongoSettings mongoSettings, IFilesService filesService, IAuthenticationService authenticationService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>("users");

            _filesService = filesService;
            _authenticationService = authenticationService;
        }

        public async Task<byte[]> GetProfilePicture(string userId)
        {
            User user = await GetUser(userId);

            if (user == null || user.ProfilePictureId == null) { return null; }

            return await _filesService.GetFile(user.ProfilePictureId);
        }

        public async Task UpdateUserAsync(string userId, UserUpdateModel userUpdateModel)
        {
            var currentUser = await GetUser(userId);

            if (currentUser == null) throw new ArgumentException("The user doesn't exist", "userId");

            // We need to check that the email is not used
            if (currentUser.Email != userUpdateModel.Email)
            {
                bool exist = _users.AsQueryable<User>().Any(user =>
                    user.Email == userUpdateModel.Email
                );

                if (exist) throw new Exception("The new email is already in use");
            }

            var update = Builders<User>.Update
                .Set(dbUser => dbUser.FirstName, userUpdateModel.FirstName)
                .Set(dbUser => dbUser.LastName, userUpdateModel.LastName)
                .Set(dbUser => dbUser.Birthdate, userUpdateModel.Birthdate)
                .Set(dbUser => dbUser.Email, userUpdateModel.Email)
                .Set(dbUser => dbUser.DiscordTag, userUpdateModel.DiscordTag);

            string fileId = "";

            if (userUpdateModel.ProfilePicture != null && userUpdateModel.ProfilePicture.Length >= 1)
            {
                fileId = await _filesService.UploadFile($"profile_{userId}", userUpdateModel.ProfilePicture);
                update = update.Set(databaseUser => databaseUser.ProfilePictureId, fileId);
            }

            await _users.UpdateOneAsync(databaseUser =>
                databaseUser.Id == userId,
                update
            );

            if (!string.IsNullOrWhiteSpace(userUpdateModel.Password))
            {
                await _authenticationService.UpdatePasswordAsync(userId, userUpdateModel.Password);
            }
        }

        private async Task<User> GetUser(string userId)
        {
            var user = await _users.FindAsync(databaseUser =>
                databaseUser.Id == userId
            );

            return await user.FirstOrDefaultAsync();
        }

    }
}
