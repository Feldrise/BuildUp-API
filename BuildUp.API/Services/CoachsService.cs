using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
using BuildUp.API.Entities.Pdf;
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
        private readonly IMongoCollection<Builder> _builders;

        private readonly IFormsService _formsService;
        private readonly IFilesService _filesService;
        private readonly IPdfService _pdfService;

        public CoachsService(IMongoSettings mongoSettings, IFormsService formsService, IFilesService filesService, IPdfService pdfService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>("users");
            _coachs = database.GetCollection<Coach>("coachs");
            _builders = database.GetCollection<Builder>("builders");

            _formsService = formsService;
            _filesService = filesService;
            _pdfService = pdfService;
        }

        public Task<Coach> GetCoachFromAdminAsync(string userId)
        {
            return GetCoachFromUserId(userId);
        }

        public async Task<Coach> GetCoachFromCoachAsync(string currentUserId, string userId)
        {
            Coach coach = await GetCoachFromUserId(userId);

            if (coach == null)
            {
                return null;
            }

            if (coach.UserId != currentUserId) throw new ArgumentException("The current user is not the coach he want's to see info", "currentUserId");

            return coach;
        }

        public async Task<List<Coach>> GetAllCoachsAsync()
        {
            return await (await _coachs.FindAsync(databaseBuilder => true)).ToListAsync();
        }

        public async Task<byte[]> GetCoachCardAsync(string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null || coach.CoachCardId == null) { return null; }

            return (await _filesService.GetFile(coach.CoachCardId)).Data;
        }


        public async Task<User> GetUserFromAdminAsync(string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null) throw new Exception("The coach doesn't exist");

            return await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == coach.UserId
            )).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserFromBuilderAsync(string currentUserId, string coachId)
        {
            Builder builder = await GetBuilderFromUserId(currentUserId);

            if (builder == null) throw new Exception("The builder for the current user doesn't exist");
            
            if (builder.CoachId != coachId)
            {
                throw new Exception("You can't get user for this coach");
            }

            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null) throw new Exception("The coach doesn't exist");

            return await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == coach.UserId
            )).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserFromCoachAsync(string currentUserId, string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null) throw new Exception("The coach doesn't exist");

            if (coach.UserId != currentUserId)
            {
                throw new Exception("You can't get user for this coach");
            }

            return await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == coach.UserId
            )).FirstOrDefaultAsync();
        }

        public async Task<List<Builder>> GetBuildersFromAdminAsync(string coachId)
        {
            return await (await _builders.FindAsync(databaseUser =>
                databaseUser.CoachId == coachId
            )).ToListAsync();
        }

        public async Task<List<Builder>> GetBuildersFromCoachAsync(string currentUserId, string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach.UserId != currentUserId) throw new Exception("This user can't see the coach builders");

            return await (await _builders.FindAsync(databaseUser =>
                databaseUser.CoachId == coachId
            )).ToListAsync();
        }

        public async Task<List<BuildupFormQA>> GetCoachFormFromAdminAsync(string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null)
            {
                return null;
            }

            return await _formsService.GetFormQAsAsync(coach.UserId);
        }

        public async Task<List<BuildupFormQA>> GetCoachFormFromCoachAsync(string currentUserId, string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null)
            {
                return null;
            }

            if (coach.UserId != currentUserId) throw new ArgumentException("The current user is not the coach he want's to see form answers", "currentUserId");

            return await _formsService.GetFormQAsAsync(coach.UserId);
        }

        public async Task<string> RegisterCoachAsync(CoachRegisterModel coachRegisterModel)
        {
            if (!UserExist(coachRegisterModel.UserId)) throw new ArgumentException("The user doesn't existe", "coachRegisterModel.UserId");
            if (CoachExist(coachRegisterModel.UserId)) throw new Exception("The coach already exists");

            string coachId = await RegisterToDatabase(coachRegisterModel);

            await _formsService.RegisterFormToDatabseAsync(coachRegisterModel.UserId, coachRegisterModel.FormQAs);

            return coachId;

        }

        public async Task<bool> SignFicheIntegrationAsync(string currentUserId, string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null || coach.UserId != currentUserId)
            {
                throw new Exception("You don't have the permission to sign");
            }

            User user = await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == coach.UserId
            )).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("The user doesn't exist...");
            }

            PdfIntegrationCoach pdfIntegrationCoach = new PdfIntegrationCoach()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthdate = user.Birthdate,
                BirthPlace = user.BirthPlace,

                Email = user.Email,
                Phone = user.Phone,

                City = user.City,
                PostalCode = user.PostalCode,
                Address = user.Address,

                Situation = coach.Situation,

                Keywords = await _formsService.GetAnswerForQuestionAsync(currentUserId, "Quelles sont les mots clés qui vous définissent ?"),
                Experience = await _formsService.GetAnswerForQuestionAsync(currentUserId, "Quels sont vos expériences ?"),
                Accroche = await _formsService.GetAnswerForQuestionAsync(currentUserId, "Quel est votre phrase d'accroche ?"),
                IdealBuilder = await _formsService.GetAnswerForQuestionAsync(currentUserId, "Quel serait le Builder idéal pour vous ?"),
                Objectifs = await _formsService.GetAnswerForQuestionAsync(currentUserId, "Quels objectifs souhaitez-vous que votre Builder atteignent au bout des 3 mois ?")
            };

            if (_pdfService.SignCoachIntegration(coachId, pdfIntegrationCoach))
            {
                var update = Builders<Coach>.Update
                    .Set(dbCoach => dbCoach.HasSignedFicheIntegration, true);

                await _coachs.UpdateOneAsync(databaseCoach =>
                   databaseCoach.Id == coachId,
                   update
                );

                return true;
            }

            return false;
        }

        public async Task UpdateCoachFromAdminAsync(string coachId, CoachUpdateModel coachUpdateModel)
        {
            await UpdateCoach(coachId, coachUpdateModel);
        }

        public async Task UpdateCoachFromCoachAsync(string currentUserId, string coachId, CoachUpdateModel coachUpdateModel)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null || coach.UserId != currentUserId)
            {
                throw new Exception("This coach can't be update by current user");
            }

            await UpdateCoach(coachId, coachUpdateModel);
        }

        public async Task RefuseCoachAsync(string coachId)
        {
            var update = Builders<Coach>.Update
                .Set(databaseCoach => databaseCoach.Status, CoachStatus.Deleted)
                .Set(databaseCoach => databaseCoach.Step, CoachSteps.Stopped);

            await _coachs.UpdateOneAsync(databaseCoach =>
                databaseCoach.Id == coachId,
                update
            );
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
                databaseCoach.Status == CoachStatus.Validated
            )).ToListAsync();

            return activeCoachs;
        }

        public async Task<List<AvailableCoachModel>> GetAvailableCoachAsync()
        {
            var activeCoachs = await (await _coachs.FindAsync(databaseCoach =>
                databaseCoach.Status == CoachStatus.Validated
            )).ToListAsync();

            List<AvailableCoachModel> availableCoachModels = new List<AvailableCoachModel>();

            foreach (Coach activeCoach in activeCoachs)
            {
                User user = await GetUserFromAdminAsync(activeCoach.Id);

                if (user == null)
                {
                    throw new Exception("The coach seems to be linked to no user...");
                }

                AvailableCoachModel model = new AvailableCoachModel()
                {
                    Id = activeCoach.Id,

                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,

                    Email = user.Email,
                    DiscordTag = user.DiscordTag,

                    Description = activeCoach.Description,

                    Competences = await _formsService.GetAnswerForQuestionAsync(user.Id, "Quelles sont vos compétences ?"),
                    Perspectives = await _formsService.GetAnswerForQuestionAsync(user.Id, "Quelles sont, selon vous, les principales perspectives pour qu’un projet fonctionne ?"),
                    CoachDefinition = await _formsService.GetAnswerForQuestionAsync(user.Id, "Comment définissez-vous le rôle de Coach ?")
                };

                availableCoachModels.Add(model);
            }

            return availableCoachModels;

        }

        private async Task<string> RegisterToDatabase(CoachRegisterModel coachRegisterModel)
        {
            Coach databaseCoach = new Coach()
            {
                UserId = coachRegisterModel.UserId,
                CandidatingDate = DateTime.Now,
                Status = CoachStatus.Candidating,
                Step = CoachSteps.Preselected,

                Situation = coachRegisterModel.Situation,
                Description = coachRegisterModel.Description
            };

            await _coachs.InsertOneAsync(databaseCoach);

            return databaseCoach.Id;
        }

        private async Task UpdateCoach(string id, CoachUpdateModel coachUpdateModel)
        {
            var update = Builders<Coach>.Update
               .Set(dbCoach => dbCoach.Status, coachUpdateModel.Status)
               .Set(dbCoach => dbCoach.Step, coachUpdateModel.Step)
               .Set(dbCoach => dbCoach.Situation, coachUpdateModel.Situation)
               .Set(dbCoach => dbCoach.Description, coachUpdateModel.Description);

            string fileId = "";

            if (coachUpdateModel.CoachCard != null && coachUpdateModel.CoachCard.Length >= 1)
            {
                fileId = await _filesService.UploadFile($"coachcar_{id}", coachUpdateModel.CoachCard);
                update = update.Set(dbCoach => dbCoach.CoachCardId, fileId);
            }

            await _coachs.UpdateOneAsync(databaseCoach =>
               databaseCoach.Id == id,
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

        private async Task<Coach> GetCoachFromUserId(string userId)
        {
            var coach = await _coachs.FindAsync(databaseCoach =>
                databaseCoach.UserId == userId
            );

            return await coach.FirstOrDefaultAsync();
        }

        private async Task<Coach> GetCoachFromCoachId(string coachId)
        {
            var coach = await _coachs.FindAsync(databaseCoach =>
                databaseCoach.Id == coachId
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
