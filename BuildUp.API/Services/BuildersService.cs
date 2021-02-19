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

        public async Task<bool> SignFicheIntegrationAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null || builder.UserId != currentUserId)
            {
                throw new Exception("You don't have the permission to sign");
            }

            User user = await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == builder.UserId
            )).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("The user doesn't exist...");
            }

            Project project = await _projectsService.GetProjectAsync(builderId);

            if (project == null)
            {
                throw new Exception("The builder doesn't have project...");
            }

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

                return true;
            }

            return false;
        }

        public async Task UpdateBuilderFromAdminAsync(string builderId, BuilderUpdateModel builderUpdateModel)
        {
            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null)
            {
                throw new Exception("This builder doesn't exist");
            }

            User user = await GetUserFromAdminAsync(builderId);

            if (user == null)
            {
                throw new Exception("Their is no user for builder...");
            }

            await UpdateBuilder(builderId, builderUpdateModel);

            if (builder.Step == BuilderSteps.Preselected && builderUpdateModel.Step == BuilderSteps.AdminMeeting)
            {
                await _notificationService.NotifyPreselectionBuilder(user.Email, user.FirstName);
            }
            if (builder.Step == BuilderSteps.AdminMeeting && builderUpdateModel.Step == BuilderSteps.CoachMeeting)
            {
                await _notificationService.NotifyAdminMeetingValidatedBuilder(user.Email, user.FirstName);
            }
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
            Coach coach = await (await _coachs.FindAsync(databaseCoach =>
                databaseCoach.Id == coachId
            )).FirstOrDefaultAsync();

            if (coach == null)
            {
                throw new Exception("This coach doesn't exist");
            }

            User coachUser = await (await _users.FindAsync(databaseUser =>
                databaseUser.Id == coach.UserId
            )).FirstOrDefaultAsync();

            if (coachUser == null)
            {
                throw new Exception("Their is no user for this coach...");
            }

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

        public async Task<string> CreateMeetingReportAsync(string currentUserId, string builderId, CreateMeetingReportModel toCreate)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null)
            {
                throw new Exception("The user is not a coach in the database...");
            }

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null)
            {
                throw new Exception("The builder for the meeting report doesn't exist...");
            }

            if (builder.CoachId != coach.Id)
            {
                throw new UnauthorizedAccessException("You are not the coach of the builder");
            }

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
                databaseMeeting.BuilderId == builderId
            );

            return await meetingReports.ToListAsync();
        }

        public async Task<List<MeetingReport>> GetMeetingReportsFromCoachAsync(string currentUserId, string builderId)
        {
            Coach coach = await GetCoachFromUserId(currentUserId);

            if (coach == null)
            {
                throw new Exception("The user is not a coach in the database...");
            }

            Builder builder = await GetBuilderFromBuilderId(builderId);

            if (builder == null)
            {
                throw new Exception("The builder doesn't exist...");
            }

            if (builder.CoachId != coach.Id)
            {
                throw new Exception("You are not the coach of the builder");
            }

            return await GetMeetingReportsFromAdminAsync(builderId);
        }

        public async Task<List<MeetingReport>> GetMeetingReportsFromBuilderAsync(string currentUserId, string builderId)
        {
            Builder builder = await GetBuilderFromUserId(currentUserId);

            if (builder == null)
            {
                throw new Exception("The builder seems to not exist");
            }

            if (builder.Id != builderId)
            {
                throw new UnauthorizedAccessException("You can't view meetings report for this builder");
            }

            return await GetMeetingReportsFromAdminAsync(builderId);
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
