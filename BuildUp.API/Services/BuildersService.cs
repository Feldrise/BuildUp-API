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
using BuildUp.API.Models;
using BuildUp.API.Models.Projects;

namespace BuildUp.API.Services
{
    public class BuildersService : IBuildersService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Builder> _builders;
        private readonly IMongoCollection<Coach> _coachs;

        private readonly IFormsService _formsService;
        private readonly IProjectsService _projectsService;
        private readonly IFilesService _filesService;

        public BuildersService(IMongoSettings mongoSettings, IFormsService formsService, IProjectsService projectsService, IFilesService filesService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>("users");
            _builders = database.GetCollection<Builder>("builders");
            _coachs = database.GetCollection<Coach>("coachs");

            _formsService = formsService;
            _projectsService = projectsService;
            _filesService = filesService;
        }

        public Task<Builder> GetBuilderFromAdminAsync(string userId)
        {
            return GetBuilderFromUserId(userId);
        }

        public async Task<Builder> GetBuilderFromCoachAsync(string currentUserId, string userId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new ArgumentException("The current user is not a coach", "currentUserId");

            Builder builder = await GetBuilderFromUserId(userId);

            if (builder == null)
            {
                return null;
            }

            return (builder.CoachId == coach.Id) ? builder : null;
        }

        public async Task<Builder> GetBuilderFromBuilderAsync(string currentUserId, string userId)
        {
            Builder builder = await GetBuilderFromUserId(userId);

            if (builder == null)
            {
                return null;
            }

            if (builder.UserId != currentUserId) throw new ArgumentException("The current user is not the builder he want's to see info", "currentUserId");

            return builder;
        }

        public async Task<User> GetUserFromAdminAsync(string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");

            return await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == builder.UserId
            )).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserFromCoachAsync(string currentUserId, string builderId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new ArgumentException("The current user is not a coach", "currentUserId");

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.CoachId != coach.Id)
            {
                throw new Exception("You can't get user for this builder");
            }

            return await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == builder.UserId
            )).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");

            if (builder.UserId != currentUserId)
            {
                throw new Exception("You can't get user for this builder");
            }

            return await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == builder.UserId
            )).FirstOrDefaultAsync();
        }

        public async Task<byte[]> GetBuilderCardAsync(string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.BuilderCardId == null) { return null; }

            return (await _filesService.GetFile(builder.BuilderCardId)).Data;
        }

        public async Task<Coach> GetCoachForBuilderFromAdminAsync(string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null)
            {
                return null;
            }

            return await GetCoachFromId(builder.CoachId);
        }

        public async Task<Coach> GetCoachForBuilderFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.UserId != currentUserId)
            {
                throw new Exception("This builder can view this coach");
            }

            return await GetCoachFromId(builder.CoachId);
        }

        public async Task<List<BuildupFormQA>> GetBuilderFormFromAdminAsync(string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null)
            {
                return null;
            }

            return await _formsService.GetFormQAsAsync(builder.UserId);
        }

        public async Task<List<BuildupFormQA>> GetBuilderFormFromCoachAsync(string currentUserId, string builderId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new ArgumentException("The current user is not a coach", "currentUserId");

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.CoachId != coach.Id)
            {
                return null;
            }

            return await _formsService.GetFormQAsAsync(builder.UserId);
        }

        public async Task<List<BuildupFormQA>> GetBuilderFormFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null)
            {
                return null;
            }

            if (builder.UserId != currentUserId) throw new ArgumentException("The current user is not the builder he want's to see form answers", "currentUserId");

            return await _formsService.GetFormQAsAsync(builder.UserId);
        }

        public async Task<Project> GetBuilderProjectFromAdminAsync(string builderId)
        {
            return await _projectsService.GetProjectAsync(builderId);
        }

        public async Task<Project> GetBuilderProjectFromCoachAsync(string currentUserId, string builderId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new ArgumentException("The current user is not a coach", "currentUserId");

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.CoachId != coach.Id)
            {
                return null;
            }

            return await _projectsService.GetProjectAsync(builderId);
        }

        public async Task<Project> GetBuilderProjectFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null)
            {
                return null;
            }

            if (builder.UserId != currentUserId) throw new ArgumentException("The current user is not the builder he want's to see the project", "currentUserId");

            return await _projectsService.GetProjectAsync(builderId);
        }

        public async Task UpdateProjectFromAdmin(string projectId, ProjectUpdateModel projectUpdateModel)
        {
            await _projectsService.UpdateProjectAsync(projectId, projectUpdateModel);
        }
        public async Task UpdateProjectFromBuilder(string currentUserId, string builderId, string projectId, ProjectUpdateModel projectUpdateModel)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null)
            {
                throw new Exception("The builder doesn't exist, you may not be authorized to permorm actions");
            }
            
            if (builder.UserId != currentUserId) throw new ArgumentException("The current user is not the builder he want's to see the project", "currentUserId");

            await _projectsService.UpdateProjectAsync(projectId, projectUpdateModel);
        }


        public async Task<string> RegisterBuilderAsync(BuilderRegisterModel builderRegisterModel)
        {
            if (!UserExist(builderRegisterModel.UserId)) throw new ArgumentException("The user doesn't existe", "builderRegisterModel.UserId");
            if (BuilderExist(builderRegisterModel.UserId)) throw new Exception("The builder already exists");

            string builderId = await RegisterToDatabase(builderRegisterModel);

            await _formsService.RegisterFormToDatabseAsync(builderRegisterModel.UserId, builderRegisterModel.FormQAs);

            return builderId;

        }

        public Task<string> SubmitProjectAsync(ProjectSubmitModel projectSubmitModel)
        {
            return _projectsService.SubmitProjectAsync(projectSubmitModel);
        }

        public async Task UpdateBuilderFromAdminAsync(string builderId, BuilderUpdateModel builderUpdateModel)
        {
            await UpdateBuilder(builderId, builderUpdateModel);
        }
        
        public async Task UpdateBuilderFromBuilderAsync(string currentUserId, string builderId, BuilderUpdateModel builderUpdateModel)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.UserId != currentUserId)
            {
                throw new Exception("This builder can't be update by current user");
            }

            await UpdateBuilder(builderId, builderUpdateModel);
        }

        public async Task RefuseBuilderAsync(string builderId)
        {
            var update = Builders<Builder>.Update
                .Set(databaseBuilder => databaseBuilder.Status, BuilderStatus.Deleted)
                .Set(databaseBuilder => databaseBuilder.Step, BuilderSteps.Abandoned);

            await _builders.UpdateOneAsync(databaseBuilder =>
                databaseBuilder.Id == builderId,
                update
            );
        }

        public async Task AssignCoachAsync(string coachId, string builderId)
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
            var candidatingBuilders = await (await _builders.FindAsync(databaseBuilder =>
                databaseBuilder.Status == BuilderStatus.Candidating
            )).ToListAsync();

            return candidatingBuilders;
        }

        public async Task<List<Builder>> GetActiveBuildersAsync()
        {
            var activeBuilders = await (await _builders.FindAsync(databaseBuilder =>
                databaseBuilder.Status == BuilderStatus.Validated
            )).ToListAsync();

            return activeBuilders;
        }

        private async Task<string> RegisterToDatabase(BuilderRegisterModel builderRegisterModel)
        {
            Builder databaseBuilder = new Builder()
            {
                UserId = builderRegisterModel.UserId,
                CandidatingDate = DateTime.Now,
                Status = BuilderStatus.Candidating,
                Step = BuilderSteps.Preselected,

                Situation = builderRegisterModel.Situation,
                Description = builderRegisterModel.Description
            };

            await _builders.InsertOneAsync(databaseBuilder);

            return databaseBuilder.Id;
        }

        private async Task UpdateBuilder(string id, BuilderUpdateModel builderUpdateModel)
        {
            var update = Builders<Builder>.Update
                .Set(dbBuilder => dbBuilder.CoachId, builderUpdateModel.CoachId)
                .Set(dbBuilder => dbBuilder.NtfReferentId, builderUpdateModel.NtfReferentId)
                .Set(dbBuilder => dbBuilder.Status, builderUpdateModel.Status)
                .Set(dbBuilder => dbBuilder.Step, builderUpdateModel.Step)
                .Set(dbBuilder => dbBuilder.Situation, builderUpdateModel.Situation)
                .Set(dbBuilder => dbBuilder.Description, builderUpdateModel.Description);

            string fileId = "";

            if (builderUpdateModel.BuilderCard != null && builderUpdateModel.BuilderCard.Length >= 1)
            {
                fileId = await _filesService.UploadFile($"buildercard_{id}", builderUpdateModel.BuilderCard);
                update = update.Set(dbBuilder => dbBuilder.BuilderCardId, fileId);
            }

            if (builderUpdateModel.ProgramEndDate != DateTime.MinValue)
            {
                update = update.Set(dbBuilder => dbBuilder.ProgramEndDate, builderUpdateModel.ProgramEndDate);
            }
            else
            {
                update = update.Set(dbBuilder => dbBuilder.ProgramEndDate, DateTime.Now.AddMonths(3));
            }

            await _builders.UpdateOneAsync(databaseBuilder =>
               databaseBuilder.Id == id,
               update
            );
        }

        private async Task<Builder> GetBuilderFromUserId(string userId)
        {
            var builder = await _builders.FindAsync(databaseBuilder =>
                databaseBuilder.UserId == userId
            );

            return await builder.FirstOrDefaultAsync();
        }

        private async Task<Builder> GetBuilderFromBuilderId(string builderId)
        {
            var builder = await _builders.FindAsync(databaseBuilder =>
                databaseBuilder.Id == builderId
            );

            return await builder.FirstOrDefaultAsync();
        }

        private async Task<Coach> GetCoachFromUserId(string userId)
        {
            var coach = await _coachs.FindAsync(databaseCoach =>
                databaseCoach.UserId == userId
            );

            return await coach.FirstOrDefaultAsync();
        }

        private async Task<Coach> GetCoachFromId(string id)
        {
            var coach = await _coachs.FindAsync(databaseCoach =>
                databaseCoach.Id == id
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
