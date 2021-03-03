using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
using BuildUp.API.Entities.Notification.CoachRequest;
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
        private readonly IMongoCollection<CoachRequest> _coachRequests;

        private readonly IFormsService _formsService;
        private readonly IFilesService _filesService;
        private readonly IPdfService _pdfService;
        private readonly INotificationService _notificationService;

        public CoachsService(IMongoSettings mongoSettings, IFormsService formsService, IFilesService filesService, IPdfService pdfService, INotificationService notificationService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>("users");
            _coachs = database.GetCollection<Coach>("coachs");
            _builders = database.GetCollection<Builder>("builders");
            _coachRequests = database.GetCollection<CoachRequest>("coach_requests");

            _formsService = formsService;
            _filesService = filesService;
            _pdfService = pdfService;
            _notificationService = notificationService;
        }

        // Getting the coach
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

            if (coach.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the user corresponding to the coach");

            return coach;
        }

        public async Task<List<Coach>> GetAllCoachsAsync()
        {
            return await (await _coachs.FindAsync(databaseBuilder => true)).ToListAsync();
        }

        // Getting the user corresponding to the coach
        public async Task<User> GetUserFromAdminAsync(string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null) throw new Exception("The coach doesn't exist");

            return await GetUserFromId(coach.UserId);
        }

        public async Task<User> GetUserFromCoachAsync(string currentUserId, string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null) throw new Exception("The coach doesn't exist");
            if (coach.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the user corresponding this coach");

            return await GetUserFromId(coach.UserId);
        }

        public async Task<User> GetUserFromBuilderAsync(string currentUserId, string coachId)
        {
            Builder builder = await GetBuilderFromUserId(currentUserId);

            if (builder == null) throw new Exception("The builder for the current user doesn't exist");
            if (builder.CoachId != coachId) throw new UnauthorizedAccessException("You are not the builder of this coach");

            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null) throw new Exception("The coach doesn't exist");

            return await GetUserFromId(coach.UserId);
        }

        // Getting "specific" coachs
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

                if (user == null) continue;

                AvailableCoachModel model = new AvailableCoachModel()
                {
                    Id = activeCoach.Id,

                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,

                    Email = user.Email,
                    DiscordTag = user.DiscordTag,

                    Situation = activeCoach.Situation,
                    Description = activeCoach.Description,

                    Competences = await _formsService.GetAnswerForQuestionAsync(user.Id, "Quelles sont vos compétences clés ?") ?? "Inconnue",
                    Questions = new List<string>()
                    {
                        "Comment définis-tu le rôle de Coach ?",
                        "Pourquoi souhaites-tu devenir Coach ?",
                        "Qu’est-ce qui t'incite à proposer ton accompagnement ?",
                        "Combien d’heures par semaine peux-tu accorder à un Builder ?",
                        "Es-tu prêt à faire preuve de patience, d’écoute et de bienveillance à l’égard des Builders ?",
                        "Quel serait le Builder idéal pour toi ?",
                        "C'est ton moment. Dis au Builder pourquoi il doit te choisir te et pas un autre Coach."
                    },

                    Answers = new List<string>()
                    {
                        await _formsService.GetAnswerForQuestionAsync(user.Id, "Comment définis-tu le rôle de Coach ?") ?? "Inconnue",
                        await _formsService.GetAnswerForQuestionAsync(user.Id, "Pourquoi souhaites-tu devenir Coach ?") ?? "Inconnue",
                        await _formsService.GetAnswerForQuestionAsync(user.Id, "Qu’est-ce qui t'incite à proposer ton accompagnement ?") ?? "Inconnue",
                        await _formsService.GetAnswerForQuestionAsync(user.Id, "Combien d’heures par semaine peux-tu accorder à un Builder ?") ?? "Inconnue",
                        await _formsService.GetAnswerForQuestionAsync(user.Id, "Es-tu prêt à faire preuve de patience, d’écoute et de bienveillance à l’égard des Builders ?") ?? "Inconnue",
                        await _formsService.GetAnswerForQuestionAsync(user.Id, "Quel serait le Builder idéal pour toi ?") ?? "Inconnue",
                        await _formsService.GetAnswerForQuestionAsync(user.Id, "C'est ton moment. Dis au Builder pourquoi il doit te choisir toi et pas un autre Coach.") ?? "Inconnue"
                    }
                };

                availableCoachModels.Add(model);
            }

            return availableCoachModels;

        }

        // Register the coach
        public async Task<string> RegisterCoachAsync(CoachRegisterModel coachRegisterModel)
        {
            if (!UserExist(coachRegisterModel.UserId)) throw new Exception("The user doesn't existe");
            if (CoachExist(coachRegisterModel.UserId)) throw new Exception("The coach already exists");

            string coachId = await RegisterToDatabase(coachRegisterModel);

            await _formsService.RegisterFormToDatabseAsync(coachRegisterModel.UserId, coachRegisterModel.FormQAs);

            return coachId;

        }

        // Updating the coach
        public async Task UpdateCoachFromAdminAsync(string coachId, CoachUpdateModel coachUpdateModel)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null) throw new Exception("This coach doesn't exist");

            User user = await GetUserFromAdminAsync(coachId);

            if (user == null) throw new Exception("Their is no user for this coach...");

            await UpdateCoach(coachId, coachUpdateModel);

            // Only admins are supposed to be able to change the steps
            // Since we don't want to spam, we only check notifications
            // on admin side
            if (coachUpdateModel.Status == CoachStatus.Deleted)
            {
                await _notificationService.NotifyRefusedCoach(user.Email, user.FirstName);
            }
            if (coach.Step == CoachSteps.Preselected && coachUpdateModel.Step == CoachSteps.Meeting)
            {
                await _notificationService.NotifyPreselectionCoach(user.Email, user.FirstName);
            }
            if (coach.Step != CoachSteps.Signing && coachUpdateModel.Step == CoachSteps.Signing)
            {
                await _notificationService.NotifyAcceptationCoach(user.Email);
            }
        }

        public async Task UpdateCoachFromCoachAsync(string currentUserId, string coachId, CoachUpdateModel coachUpdateModel)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null || coach.UserId != currentUserId) throw new UnauthorizedAccessException("You are trying to update an other coach than you");

            await UpdateCoach(coachId, coachUpdateModel);
        }

        // Refusing the coach
        public async Task RefuseCoachAsync(string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null) throw new Exception("This coach doesn't exist");

            User coachUser = await GetUserFromId(coach.UserId);
                
            if (coachUser == null) throw new Exception("Their is no user for this coach...");

            var update = Builders<Coach>.Update
                .Set(databaseCoach => databaseCoach.Status, CoachStatus.Deleted)
                .Set(databaseCoach => databaseCoach.Step, CoachSteps.Stopped);

            await _coachs.UpdateOneAsync(databaseCoach =>
                databaseCoach.Id == coachId,
                update
            );

            await _notificationService.NotifyRefusedCoach(coachUser.Email, coachUser.FirstName);
        }

        // Getting the coach's builders
        public async Task<List<Builder>> GetBuildersFromAdminAsync(string coachId)
        {
            return await (await _builders.FindAsync(databaseUser =>
                databaseUser.CoachId == coachId
            )).ToListAsync();
        }

        public async Task<List<Builder>> GetBuildersFromCoachAsync(string currentUserId, string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the coach you prettend to be");

            return await (await _builders.FindAsync(databaseUser =>
                databaseUser.CoachId == coachId
            )).ToListAsync();
        }

        // Getting the coach's form
        public async Task<List<BuildupFormQA>> GetCoachFormFromAdminAsync(string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null) throw new Exception("The coach doesn't exist");

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

        // Getting the coach's card
        public async Task<byte[]> GetCoachCardAsync(string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null || coach.CoachCardId == null) { return null; }

            return (await _filesService.GetFile(coach.CoachCardId)).Data;
        }

        // Signging the PDF for the integration
        public async Task<bool> SignFicheIntegrationAsync(string currentUserId, string coachId)
        {
            Coach coach = await GetCoachFromCoachId(coachId);

            if (coach == null || coach.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the user corresponding to the coach");

            User user = await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == coach.UserId
            )).FirstOrDefaultAsync();

            if (user == null) throw new Exception("The user doesn't exist...");

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

                await _notificationService.NotifySignedIntegrationPaperCoach(coachId, user.Email, user.FirstName);

                return true;
            }

            return false;
        }

        public async Task<List<CoachRequest>> GetCoachRequestsAsync(string currentUserId, string coachId)
        {
            return await (await _coachRequests.FindAsync(databaseRequest =>
                databaseRequest.CoachId == coachId &&
                databaseRequest.Status == CoachRequestStatus.Waiting
            )).ToListAsync();
        }

        public async Task AcceptCoachRequestAsync(string currentUserId, string coachId, string requestId)
        {
            CoachRequest request = await GetCoachRequest(requestId);

            if (request == null) throw new Exception("The request seems to not exist anymore");

            if (request.CoachId != coachId) throw new UnauthorizedAccessException("You are not the coach for this request");

            var update = Builders<CoachRequest>.Update
               .Set(dbRequest => dbRequest.Status, CoachRequestStatus.Accepted);

            await _coachRequests.UpdateOneAsync(databaseRequest =>
               databaseRequest.Id == requestId,
               update
            );

            var builderUpdate = Builders<Builder>.Update
               .Set(dbBuilder => dbBuilder.Step, BuilderSteps.Signing);

            await _builders.UpdateOneAsync(databaseBuilder =>
               databaseBuilder.Id == request.BuilderId,
               builderUpdate
            );

            // We need to send a notification
            Builder builder = await GetBuilderFromBuilderId(request.BuilderId);

            if (builder == null) throw new Exception("This builder doesn't exist");

            User builderUser = await GetUserFromId(builder.UserId);

            if (builderUser == null) throw new Exception("Their is no user for this builder...");

            await _notificationService.NotifyCoachAcceptBuilder(builderUser.Email, builderUser.FirstName);
        }

        public async Task RefuseCoachRequestAsync(string currentUserId, string coachId, string requestId)
        {
            CoachRequest request = await GetCoachRequest(requestId);

            if (request == null) throw new Exception("The request seems to not exist anymore");

            if (request.CoachId != coachId) throw new UnauthorizedAccessException("You are not the coach for this request");

            var update = Builders<CoachRequest>.Update
               .Set(dbRequest => dbRequest.Status, CoachRequestStatus.Refused);

            await _coachRequests.UpdateOneAsync(databaseRequest =>
               databaseRequest.Id == requestId,
               update
            );

            var builderUpdate = Builders<Builder>.Update
               .Set(dbBuilder => dbBuilder.CoachId, null);

            await _builders.UpdateOneAsync(databaseBuilder =>
               databaseBuilder.Id == request.BuilderId,
               builderUpdate
            );
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

        private async Task<User> GetUserFromId(string userId)
        {
            var user = await _users.FindAsync(databaseUser =>
                databaseUser.Id == userId
            );

            return await user.FirstOrDefaultAsync();
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

        private async Task<CoachRequest> GetCoachRequest(string requestId)
        {
            var request = await _coachRequests.FindAsync(databaseRequest =>
                databaseRequest.Id == requestId
            );

            return await request.FirstOrDefaultAsync();
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
