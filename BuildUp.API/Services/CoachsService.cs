using BuildUp.API.Entities;
using BuildUp.API.Entities.Status;
using BuildUp.API.Entities.Steps;
using BuildUp.API.Models.Coachs;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services
{
    public class CoachsService : ICoachsService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Coach> _coachs;

        private readonly IFormsService _formsService;

        public CoachsService(IMongoSettings mongoSettings, IFormsService formsService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>("users");
            _coachs = database.GetCollection<Coach>("coachs");

            _formsService = formsService;
        }

        public Task<Coach> GetCoachFromAdminAsync(string userId)
        {
            return GetCoach(userId);
        }

        public async Task<Coach> GetCoachFromCoachAsync(string currentUserId, string userId)
        {
            Coach coach = await GetCoach(userId);

            if (coach == null)
            {
                return null;
            }

            if (coach.UserId != currentUserId) throw new ArgumentException("The current user is not the coach he want's to see info", "currentUserId");

            return coach;
        }

        public async Task<string> RegisterCoachAsync(CoachRegisterModel coachRegisterModel)
        {
            if (!UserExist(coachRegisterModel.UserId)) throw new ArgumentException("The user doesn't existe", "coachRegisterModel.UserId");
            if (CoachExist(coachRegisterModel.UserId)) throw new Exception("The coach already exists");

            string coachId = await RegisterToDatabase(coachRegisterModel);

            await _formsService.RegisterFormToDatabseAsync(coachRegisterModel.UserId, coachRegisterModel.FormQAs);

            return coachId;

        }


        public async Task<List<Coach>> GetCandidatingCoachsAsync()
        {
            var candidatingCoachs = await (await _coachs.FindAsync(databaseCoach =>
                databaseCoach.Status == CoachStatus.Candidating
            )).ToListAsync();

            return candidatingCoachs;
        }

        public async Task<List<Coach>> GetActiveCoachsAsync()
        {
            var activeCoachs = await (await _coachs.FindAsync(databaseCoach =>
                databaseCoach.Step == CoachSteps.Active
            )).ToListAsync();

            return activeCoachs;
        }

        private async Task<string> RegisterToDatabase(CoachRegisterModel coachRegisterModel)
        {
            Coach databaseCoach = new Coach()
            {
                UserId = coachRegisterModel.UserId,
                Status = CoachStatus.Candidating,
                Step = CoachSteps.Preselected,

                Department = coachRegisterModel.Department,
                Situation = coachRegisterModel.Situation,
                Description = coachRegisterModel.Description
            };

            await _coachs.InsertOneAsync(databaseCoach);

            return databaseCoach.Id;
        }

        private async Task<Coach> GetCoach(string userId)
        {
            var coach = await _coachs.FindAsync(databaseCoach =>
                databaseCoach.UserId == userId
            );

            return await coach.FirstOrDefaultAsync();
        }

        private bool CoachExist(string userId)
        {
            return _coachs.AsQueryable<Coach>().Any(coach =>
                coach.UserId == userId
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
