using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
using BuildUp.API.Settings.Interfaces;
using BuildUp.API.Services.Interfaces;
using MongoDB.Bson;
using BuildUp.API.Entities.Steps;
using BuildUp.API.Entities.Status;
using BuildUp.API.Models.Builders;

namespace BuildUp.API.Services
{
    public class BuildersService : IBuildersService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Builder> _builders;
        private readonly IMongoCollection<Coach> _coachs;

        private readonly IFormsService _formsService;

        public BuildersService(IMongoSettings mongoSettings, IFormsService formsService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>("users");
            _builders = database.GetCollection<Builder>("builders");
            _coachs = database.GetCollection<Coach>("coachs");

            _formsService = formsService;
        }

        public Task<Builder> GetBuilderFromAdminAsync(string userId)
        {
            return GetBuilder(userId);
        }

        public async Task<Builder> GetBuilderFromCoachAsync(string currentUserId, string userId)
        {
            Coach coach = await GetCoach(currentUserId);

            if (coach == null) throw new ArgumentException("The current user is not a coach", "currentUserId");

            Builder builder = await GetBuilder(userId);

            if (builder == null)
            {
                return null;
            }

            return (builder.CoachId == coach.Id) ? builder : null;
        }

        public async Task<Builder> GetBuilderFromBuilderAsync(string currentUserId, string userId)
        {
            Builder builder = await GetBuilder(userId);

            if (builder == null)
            {
                return null;
            }

            if (builder.UserId != currentUserId) throw new ArgumentException("The current user is not the builder he want's to see info", "currentUserId");

            return builder;
        }

        public async Task<string> RegisterBuilderAsync(BuilderRegisterModel builderRegisterModel)
        {
            if (!UserExist(builderRegisterModel.UserId)) throw new ArgumentException("The user doesn't existe", "builderRegisterModel.UserId");
            if (BuilderExist(builderRegisterModel.UserId)) throw new Exception("The builder already exists");

            string builderId = await RegisterToDatabase(builderRegisterModel);

            await _formsService.RegisterFormToDatabseAsync(builderRegisterModel.UserId, builderRegisterModel.FormQAs);

            return builderId;

        }

        public async Task AssignCoach(string coachId, string builderId)
        {
            var update = Builders<Builder>.Update
                .Set(dbBuilder => dbBuilder.CoachId, coachId);

            await _builders.UpdateOneAsync(databaseBuilder => 
                databaseBuilder.Id == builderId, 
                update
            );
        }

        public async Task<List<Builder>> GetCandidatingBuildersAsync()
        {
            var _candidatingBuilders = await (await _builders.FindAsync(databaseBuilder =>
                databaseBuilder.Status == BuilderStatus.Candidating
            )).ToListAsync();

            return _candidatingBuilders;
        }

        public async Task<List<Builder>> GetActiveBuildersAsync()
        {
            var _candidatingBuilders = await (await _builders.FindAsync(databaseBuilder =>
                databaseBuilder.Step == BuilderSteps.Active
            )).ToListAsync();

            return _candidatingBuilders;
        }

        private async Task<string> RegisterToDatabase(BuilderRegisterModel builderRegisterModel)
        {
            Builder databaseBuilder = new Builder()
            {
                UserId = builderRegisterModel.UserId,
                Status = BuilderStatus.Candidating,
                Step = BuilderSteps.Preselected,

                Department = builderRegisterModel.Department,
                Situation = builderRegisterModel.Situation,
                Description = builderRegisterModel.Description
            };

            await _builders.InsertOneAsync(databaseBuilder);

            return databaseBuilder.Id;
        }

        private async Task<Builder> GetBuilder(string userId)
        {
            var builder = await _builders.FindAsync(databaseBuilder =>
                databaseBuilder.UserId == userId
            );

            return await builder.FirstOrDefaultAsync();
        }

        private async Task<Coach> GetCoach(string userId)
        {
            var coach = await _coachs.FindAsync(databaseCoach =>
                databaseCoach.UserId == userId
            );

            return await coach.FirstOrDefaultAsync();
        }

        private bool BuilderExist(string userId)
        {
            return _builders.AsQueryable<Builder>().Any(builder =>
                builder.UserId == userId
            );
        }

        private bool UserExist(string userId)
        {
            return _users.AsQueryable<User>().Any(user =>
                user.Id == userId
            );
        }

    }
}
