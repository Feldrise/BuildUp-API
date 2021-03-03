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
using BuildUp.API.Entities.Pdf;
using BuildUp.API.Entities.Notification.CoachRequest;
using BuildUp.API.Models.MeetingReports;

namespace BuildUp.API.Services
{
    public class BuildersService : IBuildersService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Builder> _builders;
        private readonly IMongoCollection<Coach> _coachs;
        private readonly IMongoCollection<CoachRequest> _coachRequests;
        private readonly IMongoCollection<MeetingReport> _meetingRepors;

        private readonly IFormsService _formsService;
        private readonly IProjectsService _projectsService;
        private readonly IFilesService _filesService;
        private readonly IPdfService _pdfService;
        private readonly INotificationService _notificationService;

        public BuildersService(IMongoSettings mongoSettings, IFormsService formsService, IProjectsService projectsService, IFilesService filesService, IPdfService pdfService, INotificationService notificationService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _users = database.GetCollection<User>("users");
            _builders = database.GetCollection<Builder>("builders");
            _coachs = database.GetCollection<Coach>("coachs");
            _coachRequests = database.GetCollection<CoachRequest>("coach_requests");
            _meetingRepors = database.GetCollection<MeetingReport>("meeting_reports");

            _formsService = formsService;
            _projectsService = projectsService;
            _filesService = filesService;
            _pdfService = pdfService;
            _notificationService = notificationService;
        }

        // Getting the builder
        public Task<Builder> GetBuilderFromAdminAsync(string userId)
        {
            return GetBuilderFromUserId(userId);
        }

        public async Task<Builder> GetBuilderFromCoachAsync(string currentUserId, string userId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new UnauthorizedAccessException("The current user is not a coach");

            Builder builder = await GetBuilderFromUserId(userId);

            if (builder == null)
            {
                return null;
            }

            if (builder.CoachId != coach.Id) throw new UnauthorizedAccessException("You are not the coach of this builder");

            return (builder.CoachId == coach.Id) ? builder : null;
        }

        public async Task<Builder> GetBuilderFromBuilderAsync(string currentUserId, string userId)
        {
            Builder builder = await GetBuilderFromUserId(userId);

            if (builder == null)
            {
                return null;
            }

            if (builder.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the builder you want's to see info");

            return builder;
        }

        // Getting the user corresponding to the builder
        public async Task<User> GetUserFromAdminAsync(string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");

            return await GetUserFromId(builder.UserId);
        }

        public async Task<User> GetUserFromCoachAsync(string currentUserId, string builderId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new UnauthorizedAccessException("The current user is not a coach");

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.CoachId != coach.Id) throw new UnauthorizedAccessException("You are not the coach of this builder");

            return await GetUserFromId(builder.UserId);
        }

        public async Task<User> GetUserFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");

            if (builder.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the builder");

            return await GetUserFromId(builder.Id);
        }

        // Getting "specific" builder
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

        // Register the builder
        public async Task<string> RegisterBuilderAsync(BuilderRegisterModel builderRegisterModel)
        {
            if (!UserExist(builderRegisterModel.UserId)) throw new Exception("The user doesn't existe");
            if (BuilderExist(builderRegisterModel.UserId)) throw new Exception("The builder already exists");

            string builderId = await RegisterToDatabase(builderRegisterModel);

            await _formsService.RegisterFormToDatabseAsync(builderRegisterModel.UserId, builderRegisterModel.FormQAs);

            return builderId;

        }

        public Task<string> SubmitProjectAsync(ProjectSubmitModel projectSubmitModel)
        {
            return _projectsService.SubmitProjectAsync(projectSubmitModel);
        }

        // Updating the builder
        public async Task UpdateBuilderFromAdminAsync(string builderId, BuilderUpdateModel builderUpdateModel)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("This builder doesn't exist");

            User user = await GetUserFromAdminAsync(builderId);

            if (user == null) throw new Exception("Their is no user for builder...");

            await UpdateBuilder(builderId, builderUpdateModel);

            // Only admins are supposed to be able to change the steps
            // Since we don't want to spam, we only check notifications
            // on admin side
            if (builderUpdateModel.Status == BuilderStatus.Deleted)
            {
                await _notificationService.NotifyRefusedBuilder(user.Email, user.FirstName);
            }
            if (builder.Step == BuilderSteps.Preselected && builderUpdateModel.Step == BuilderSteps.AdminMeeting)
            {
                await _notificationService.NotifyPreselectionBuilder(user.Email, user.FirstName);
            }
            if (builder.Step != BuilderSteps.CoachMeeting && builderUpdateModel.Step == BuilderSteps.CoachMeeting)
            {
                await _notificationService.NotifyAdminMeetingValidatedBuilder(user.Email, user.FirstName);
            }
        }

        public async Task UpdateBuilderFromBuilderAsync(string currentUserId, string builderId, BuilderUpdateModel builderUpdateModel)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the builder you prettend to be");

            await UpdateBuilder(builderId, builderUpdateModel);
        }

        public async Task UpdateProjectFromAdmin(string projectId, ProjectUpdateModel projectUpdateModel)
        {
            await _projectsService.UpdateProjectAsync(projectId, projectUpdateModel);
        }

        public async Task UpdateProjectFromBuilder(string currentUserId, string builderId, string projectId, ProjectUpdateModel projectUpdateModel)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");
            if (builder.UserId != currentUserId) throw new UnauthorizedAccessException("The current user is not the builder he want's to see the project");

            await _projectsService.UpdateProjectAsync(projectId, projectUpdateModel);
        }

        // Refuse the builder
        public async Task RefuseBuilderAsync(string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("This builder doesn't exist");

            User builderUser = await GetUserFromId(builder.UserId);
                
            if (builderUser == null) throw new Exception("Their is no user for this builder...");

            var update = Builders<Builder>.Update
                .Set(databaseBuilder => databaseBuilder.Status, BuilderStatus.Deleted)
                .Set(databaseBuilder => databaseBuilder.Step, BuilderSteps.Abandoned);

            await _builders.UpdateOneAsync(databaseBuilder =>
                databaseBuilder.Id == builderId,
                update
            );

            await _notificationService.NotifyRefusedBuilder(builderUser.Email, builderUser.FirstName);
        }

        // Assigning coach to the builder
        public async Task AssignCoachAsync(string coachId, string builderId)
        {
            Coach coach = await GetCoachFromId(coachId);

            if (coach == null) throw new Exception("This coach doesn't exist");

            User coachUser = await GetUserFromId(coach.UserId);
                
            if (coachUser == null) throw new Exception("Their is no user for this coach...");

            var update = Builders<Builder>.Update
                .Set(dbBuilder => dbBuilder.CoachId, coachId);

            await _builders.UpdateOneAsync(databaseBuilder =>
                databaseBuilder.Id == builderId,
                update
            );

            await _coachRequests.InsertOneAsync(new CoachRequest()
            {
                BuilderId = builderId,
                CoachId = coachId,
                Status = CoachRequestStatus.Waiting
            });

            await _notificationService.NotifyBuilderChoosedCoach(coachUser.Email);
        }

        // Getting the builder's coach
        public async Task<Coach> GetCoachForBuilderFromAdminAsync(string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");

            return await GetCoachFromId(builder.CoachId);
        }

        public async Task<Coach> GetCoachForBuilderFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");
            if (builder.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the builder");

            return await GetCoachFromId(builder.CoachId);
        }

        // Getting the builder's form
        public async Task<List<BuildupFormQA>> GetBuilderFormFromAdminAsync(string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");

            return await _formsService.GetFormQAsAsync(builder.UserId);
        }

        public async Task<List<BuildupFormQA>> GetBuilderFormFromCoachAsync(string currentUserId, string builderId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new UnauthorizedAccessException("You are not a coach");

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");
            if (builder.CoachId != coach.Id) throw new UnauthorizedAccessException("You are not the coach of this builder");

            return await _formsService.GetFormQAsAsync(builder.UserId);
        }

        public async Task<List<BuildupFormQA>> GetBuilderFormFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");
            if (builder.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the builder you want to see forms");

            return await _formsService.GetFormQAsAsync(builder.UserId);
        }

        // Getting the builder's project
        public async Task<Project> GetBuilderProjectFromAdminAsync(string builderId)
        {
            return await _projectsService.GetProjectAsync(builderId);
        }

        public async Task<Project> GetBuilderProjectFromCoachAsync(string currentUserId, string builderId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new UnauthorizedAccessException("You are not a coach");

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");
            if (builder.CoachId != coach.Id) throw new UnauthorizedAccessException("You are not the coach of this builder");

            return await _projectsService.GetProjectAsync(builderId);
        }

        public async Task<Project> GetBuilderProjectFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");
            if (builder.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the builder you want to see project");

            return await _projectsService.GetProjectAsync(builderId);
        }

        // Getting the builder's card
        public async Task<byte[]> GetBuilderCardAsync(string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.BuilderCardId == null) { return null; }

            return (await _filesService.GetFile(builder.BuilderCardId)).Data;
        }

        // Signging the PDF for the integration
        public async Task<bool> SignFicheIntegrationAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist");
            if (builder.UserId != currentUserId) throw new UnauthorizedAccessException("You are not the builder you want to sign for");

            User user = await GetUserFromId(builder.UserId);

            if (user == null) throw new Exception("The user doesn't exist...");

            Project project = await _projectsService.GetProjectAsync(builderId);

            if (project == null) throw new Exception("The builder doesn't have project...");

            PdfIntegrationBuilder pdfIntegrationBuilder = new PdfIntegrationBuilder()
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

                Situation = builder.Situation,

                Keywords = await _formsService.GetAnswerForQuestionAsync(currentUserId, "Vos proches vous présentent comme quelqu’un :"),
                Accroche = await _formsService.GetAnswerForQuestionAsync(currentUserId, "Donnez une phrase d'accroche pour vous"),
                Expectation = await _formsService.GetAnswerForQuestionAsync(currentUserId, "Pourquoi souhaitez-vous intégrer le programme Build Up ?"),
                Objectifs = await _formsService.GetAnswerForQuestionAsync(currentUserId, "Quels objectifs souhaitez-vous atteindre au bout des 3 mois de programme ?"),

                ProjectDomaine = project.Categorie,
                ProjectName = project.Name,
                ProjectLaunchDate = project.LaunchDate,
                ProjectDescription = project.Description,
                ProjectTeam = project.Team
            };

            if (_pdfService.SignBuilderIntegration(builderId, pdfIntegrationBuilder))
            {
                var update = Builders<Builder>.Update
                    .Set(dbBuilder => dbBuilder.HasSignedFicheIntegration, true);

                await _builders.UpdateOneAsync(databaseBuilder =>
                   databaseBuilder.Id == builderId,
                   update
                );

                await _notificationService.NotifySignedIntegrationPaperBuilder(builderId, user.Email, user.FirstName);

                return true;
            }

            return false;
        }

        // Manage meeting reports
        public async Task<string> CreateMeetingReportAsync(string currentUserId, string builderId, CreateMeetingReportModel toCreate)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new Exception("You are not a coach");

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder for the meeting report doesn't exist...");
            if (builder.CoachId != coach.Id) throw new UnauthorizedAccessException("You are not the coach of the builder");

            MeetingReport meetingReport = new MeetingReport()
            {
                BuilderId = builderId,
                CoachId = coach.Id,

                Date = DateTime.Now,
                NextMeetingDate = toCreate.NextMeetingDate,

                Objectif = toCreate.Objectif,
                Evolution = toCreate.Evolution,
                WhatsNext = toCreate.WhatsNext,
                Comments = toCreate.Comments
            };

            await _meetingRepors.InsertOneAsync(meetingReport);

            return meetingReport.Id;
        }

        public async Task<List<MeetingReport>> GetMeetingReportsFromAdminAsync(string builderId)
        {
            var meetingReports = await _meetingRepors.FindAsync(databaseMeeting =>
                databaseMeeting.BuilderId == builderId,
                new FindOptions<MeetingReport>()
                {
                    Sort = Builders<MeetingReport>.Sort.Descending("Date")
                }
            );

            return await meetingReports.ToListAsync();
        }

        public async Task<List<MeetingReport>> GetMeetingReportsFromCoachAsync(string currentUserId, string builderId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null) throw new Exception("You are not a coach");

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null) throw new Exception("The builder doesn't exist...");

            if (builder.CoachId != coach.Id) throw new UnauthorizedAccessException("You are not the coach of the builder");

            return await GetMeetingReportsFromAdminAsync(builderId);
        }

        public async Task<List<MeetingReport>> GetMeetingReportsFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromUserId(currentUserId);

            if (builder == null) throw new Exception("You are not a builder");

            if (builder.Id != builderId)
            {
                throw new UnauthorizedAccessException("You can't view meetings report for this builder");
            }

            return await GetMeetingReportsFromAdminAsync(builderId);
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

        private async Task<User> GetUserFromId(string id)
        {
            var user = await _users.FindAsync(databaseUser =>
                databaseUser.Id == id
            );

            return await user.FirstOrDefaultAsync();
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
