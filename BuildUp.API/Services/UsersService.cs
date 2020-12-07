using BuildUp.API.Entities;
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

        public UsersService(IMongoSettings mongoSettings, IFilesService filesService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>("users");

            _filesService = filesService;
        }

        public async Task<byte[]> GetProfilePicture(string userId)
        {
            User user = await GetUser(userId);

            if (user == null || user.ProfilePictureId == null) { return null; }

            return await _filesService.GetFile(user.ProfilePictureId);
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
