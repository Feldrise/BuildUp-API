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

namespace BuildUp.API.Services
{
    public class BuildersService : IBuildersService
    {
        private readonly IMongoCollection<Builder> _builders;
        private readonly IMongoCollection<Coach> _coachs;
        private readonly IMongoCollection<BuildupForm> _forms;
        private readonly IMongoCollection<BuildupFormQA> _formsQA;

        public BuildersService(IMongoSettings mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _builders = database.GetCollection<Builder>("builders");
            _coachs = database.GetCollection<Coach>("coachs");
            _forms = database.GetCollection<BuildupForm>("forms");
            _formsQA = database.GetCollection<BuildupFormQA>("forms_qa");
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

    }
}
