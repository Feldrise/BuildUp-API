using BuildUp.API.Entities.BuildOn;
using BuildUp.API.Entities;
using BuildUp.API.Models.BuildOn;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildUp.API.Models;

namespace BuildUp.API.Services
{
    public class BuildOnsService : IBuildOnsService
    {
        private readonly IMongoCollection<BuildOn> _buildOns;
        private readonly IMongoCollection<BuildOnStep> _buildOnSteps;
        private readonly IMongoCollection<BuildOnReturning> _buildOnReturnings;

        private readonly IFilesService _filesService;
        private readonly IBuildersService _buildersService;
        private readonly ICoachsService _coachsService;
        private readonly IProjectsService _projectsService;
        private readonly INotificationService _notificationService;

        public BuildOnsService(IMongoSettings mongoSettings, IFilesService filesService, IBuildersService buildersService, ICoachsService coachsService, IProjectsService projectsService, INotificationService notificationService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _buildOns = database.GetCollection<BuildOn>("buildons");
            _buildOnSteps = database.GetCollection<BuildOnStep>("buildon_steps");
            _buildOnReturnings = database.GetCollection<BuildOnReturning>("buildon_returnings");

            _filesService = filesService;
            _buildersService = buildersService;
            _coachsService = coachsService;
            _projectsService = projectsService;
            _notificationService = notificationService;
        }

        // Getting build-ons and steps
        public async Task<List<BuildOn>> GetAllAsync()
        {
            return await (await _buildOns.FindAsync(databaseBuildOn =>
                true,
                new FindOptions<BuildOn>()
                {
                    Sort = Builders<BuildOn>.Sort.Ascending("Index")
                }
            )).ToListAsync();
        }

        public async Task<List<BuildOnStep>> GetAllStepsAsync(string buildonId)
        {
            return await (await _buildOnSteps.FindAsync(databaseBuildOnStep =>
                databaseBuildOnStep.BuildOnId == buildonId,
                new FindOptions<BuildOnStep>()
                {
                    Sort = Builders<BuildOnStep>.Sort.Ascending("Index")
                }
            )).ToListAsync();
        }

        // Updating build-ons and steps
        public async Task<List<BuildOn>> UpdateBuildOnsAsync(List<BuildOnManageModel> buildOnManageModels)
        {
            List<BuildOn> result = new List<BuildOn>();

            for (int i = 0; i < buildOnManageModels.Count; ++i)
            {
                if (buildOnManageModels[i].Id == null)
                {
                    result.Add(await CreateBuildOnAsync(i, buildOnManageModels[i]));
                }
                else
                {
                    result.Add(await UpdateBuildOnAsync(buildOnManageModels[i].Id, i, buildOnManageModels[i]));
                }
            }

            return result;
        }

        public async Task<List<BuildOnStep>> UpdateBuildOnStepsAsync(string buildOnId, List<BuildOnStepManageModel> buildOnStepManageModels)
        {
            List<BuildOnStep> result = new List<BuildOnStep>();

            for (int i = 0; i < buildOnStepManageModels.Count; ++i)
            {
                if (buildOnStepManageModels[i].Id == null)
                {
                    result.Add(await CreateBuildOnStepAsync(buildOnId, i, buildOnStepManageModels[i]));
                }
                else
                {
                    result.Add(await UpdateBuildOnStepAsync(buildOnId, buildOnStepManageModels[i].Id, i, buildOnStepManageModels[i]));
                }
            }

            return result;
        }

        // Deleting build-ons and steps
        public async Task DeleteBuildOnAsync(string buildonId)
        {
            await _buildOns.DeleteOneAsync(databaseBuildOn =>
                databaseBuildOn.Id == buildonId
            );
        }

        public async Task DeleteBuildOnStepAsync(string buildonStepId)
        {
            await _buildOnSteps.DeleteOneAsync(databaseBuildOnStep =>
                databaseBuildOnStep.Id == buildonStepId
            );
        }

        // Getting image for build-ons and steps
        public async Task<byte[]> GetImageForBuildOnAsync(string buildOnId)
        {
            BuildOn buildOn = await GetBuildOn(buildOnId);

            if (buildOn == null || buildOn.ImageId == null) { return null;  }

            return (await _filesService.GetFile(buildOn.ImageId)).Data;
        }

        public async Task<byte[]> GetImageForBuildOnStepAsync(string buildOnStepId)
        {
            BuildOnStep buildOnStep = await GetBuildOnStep(buildOnStepId);

            if (buildOnStep == null || buildOnStep.ImageId == null) { return null; }

            return (await _filesService.GetFile(buildOnStep.ImageId)).Data;
        }

        // Getting the proofs
        public async Task<List<BuildOnReturning>> GetReturningsFromAdmin(string projectId)
        {
            return await (await _buildOnReturnings.FindAsync(databaseReturning =>
                databaseReturning.ProjectId == projectId
            )).ToListAsync();
        }

        public async Task<List<BuildOnReturning>> GetReturningFromBuilder(string currentUserId, string projectId)
        {
            Builder builder = await _buildersService.GetBuilderFromAdminAsync(currentUserId);
            Project project = await _projectsService.GetProjectFromIdAsync(projectId);

            if (builder == null) throw new UnauthorizedAccessException("You are not a builder");
            if (project == null) throw new Exception("The project doesn't exist");
            if (builder.Id != project.BuilderId) throw new UnauthorizedAccessException("You are not the owner of this project");

            return await (await _buildOnReturnings.FindAsync(databaseReturning =>
                databaseReturning.ProjectId == projectId
            )).ToListAsync();
        }
        public async Task<List<BuildOnReturning>> GetReturningFromCoach(string currentUserId, string projectId)
        {
            Coach coach = await _coachsService.GetCoachFromAdminAsync(currentUserId);
            Project project = await _projectsService.GetProjectFromIdAsync(projectId);

            if (coach == null) throw new UnauthorizedAccessException("You are not a coach");
            if (project == null) throw new Exception("The project doesn't exist");

            Coach builderCoach = await _buildersService.GetCoachForBuilderFromAdminAsync(project.BuilderId);

            if (coach.Id != builderCoach.Id) throw new UnauthorizedAccessException("You are not the coach of this builder");

            return await (await _buildOnReturnings.FindAsync(databaseReturning =>
                databaseReturning.ProjectId == projectId
            )).ToListAsync();
        }

        // Getting proof file
        public async Task<FileModel> GetReturningFileFromAdmin(string buildOnReturningId)
        {
            var buildOnReturning = await GetReturning(buildOnReturningId);

            if (buildOnReturning == null || buildOnReturning.FileId == null)
            {
                return null;
            }

            return await _filesService.GetFile(buildOnReturning.FileId);
        }

        public async Task<FileModel> GetReturningFileFromCoach(string currentUserId, string buildOnReturningId)
        {
            var buildOnReturning = await GetReturning(buildOnReturningId);

            if (buildOnReturning == null || buildOnReturning.FileId == null)
            {
                return null;
            }

            Coach coach = await _coachsService.GetCoachFromAdminAsync(currentUserId);
            Project project = await _projectsService.GetProjectFromIdAsync(buildOnReturning.ProjectId);

            if (coach == null) throw new UnauthorizedAccessException("You are not a coach");
            if (project == null) throw new Exception("The project doesn't exist");

            Coach builderCoach = await _buildersService.GetCoachForBuilderFromAdminAsync(project.BuilderId);

            if (coach.Id != builderCoach.Id) throw new UnauthorizedAccessException("You are not the coach of this builder");

            return await _filesService.GetFile(buildOnReturning.FileId);
        }

        public async Task<FileModel> GetReturningFileFromBuilder(string currentUserId, string buildOnReturningId)
        {
            var buildOnReturning = await GetReturning(buildOnReturningId);

            if (buildOnReturning == null || buildOnReturning.FileId == null)
            {
                return null;
            }

            Builder builder = await _buildersService.GetBuilderFromAdminAsync(currentUserId);
            Project project = await _projectsService.GetProjectFromIdAsync(buildOnReturning.ProjectId);

            if (builder == null) throw new UnauthorizedAccessException("You are not a builder");
            if (project == null) throw new Exception("The project doesn't exist");
            if (builder.Id != project.BuilderId) throw new UnauthorizedAccessException("You are not the owner of this project");

            return await _filesService.GetFile(buildOnReturning.FileId);
        }

        // Sending proof
        public async Task<string> SendReturningAsync(string currentUserId, string projectId, BuildOnReturningSubmitModel buildOnReturningSubmitModel)
        {
            // First we need basics checks
            Builder builder = await _buildersService.GetBuilderFromAdminAsync(currentUserId);

            if (builder == null) throw new UnauthorizedAccessException("You are not a builder");

            Coach coachForBuilder = await _buildersService.GetCoachForBuilderFromAdminAsync(builder.Id);
            var project = await _projectsService.GetProjectAsync(builder.Id);

            if (project == null) throw new Exception("The project doesn't exist");
            if (coachForBuilder == null) throw new Exception("This builder don't have a coach...");
            if (project.Id != projectId) throw new UnauthorizedAccessException("The project doesn't belong to you");

            User userForCoach = await _coachsService.GetUserFromAdminAsync(coachForBuilder.Id);

            if (userForCoach == null) throw new Exception("The coach doesn't have any user");

            // Then we register the returning
            string fileId = null;
            if (buildOnReturningSubmitModel.File != null && buildOnReturningSubmitModel.File.Length >= 1)
            {
                var filename = $"{projectId}_{buildOnReturningSubmitModel.FileName}";
                fileId = await _filesService.UploadFile(filename, buildOnReturningSubmitModel.File, false);
            }

            BuildOnReturning returning = new BuildOnReturning()
            {
                ProjectId = projectId,
                BuildOnStepId = buildOnReturningSubmitModel.BuildOnStepId,
                Type = buildOnReturningSubmitModel.Type,
                Status = BuildOnReturningStatus.Waiting,
                FileName = buildOnReturningSubmitModel.FileName,
                FileId = fileId,
                Comment = buildOnReturningSubmitModel.Comment
            };

            await _buildOnReturnings.InsertOneAsync(returning);

            // Now we need to notify the coach
            await _notificationService.NotifyBuildOnReturningSubmited(userForCoach.Email);

            return returning.Id;
        }

        // Accepting proof
        public async Task AcceptReturningFromAdmin(string projectId, string buildOnReturningId)
        {
            var project = await _projectsService.GetProjectFromIdAsync(projectId);

            if (project == null) throw new Exception("The project doesn't exist");

            var buildOnReturning = await GetReturning(buildOnReturningId);

            if (buildOnReturning == null) throw new Exception("The build-on returning doesn't exist");

            // We check if the returnging is waiting for coach validation or 
            // if it already has been provided
            if (buildOnReturning.Status == BuildOnReturningStatus.Waiting)
            {
                buildOnReturning.Status = BuildOnReturningStatus.WaitingCoach;
            }
            else
            {
                buildOnReturning.Status = BuildOnReturningStatus.Validated;
                await IncrementProjectBuildOnStep(project);
            }

            await _buildOnReturnings.ReplaceOneAsync(databaseReturning =>
                databaseReturning.Id == buildOnReturning.Id,
                buildOnReturning
            );

        }

        public async Task AcceptReturningFromCoach(string currentUserId, string projectId, string buildOnReturningId)
        {
            var project = await _projectsService.GetProjectFromIdAsync(projectId);

            if (project == null) throw new Exception("The project doesn't exist");

            var buildOnReturning = await GetReturning(buildOnReturningId);

            if (buildOnReturning == null) throw new Exception("The build-on returning doesn't exist");

            Coach coach = await _coachsService.GetCoachFromAdminAsync(currentUserId);

            if (coach == null) throw new UnauthorizedAccessException("You are not a coach");

            Coach builderCoach = await _buildersService.GetCoachForBuilderFromAdminAsync(project.BuilderId);

            if (coach.Id != builderCoach?.Id) throw new UnauthorizedAccessException("You are not the coach of the builder who submited the returning");

            // We check if the returnging is waiting for admin validation or 
            // if it already has been provided
            if (buildOnReturning.Status == BuildOnReturningStatus.Waiting)
            {
                buildOnReturning.Status = BuildOnReturningStatus.WaitingAdmin;
            }
            else
            {
                buildOnReturning.Status = BuildOnReturningStatus.Validated;
                await IncrementProjectBuildOnStep(project);
            }

            await _buildOnReturnings.ReplaceOneAsync(databaseReturning =>
                databaseReturning.Id == buildOnReturning.Id,
                buildOnReturning
            );

        }

        // Refusing proof
        public async Task RefuseReturningFromAdmin(string buildOnReturningId, string reason)
        {
            var buildOnReturning = await GetReturning(buildOnReturningId);

            if (buildOnReturning == null) throw new Exception("It seems that the returning doesn't exist");

            Project project = await _projectsService.GetProjectFromIdAsync(buildOnReturning.ProjectId);

            if (project == null) throw new Exception("The project doesn't exist");

            User builderUser = await _buildersService.GetUserFromAdminAsync(project.BuilderId);

            if (builderUser == null) throw new Exception("Their is no user for this project...");

            var update = Builders<BuildOnReturning>.Update
                .Set(dbBuildOnReturnging => dbBuildOnReturnging.Status, BuildOnReturningStatus.Refused);

            await _buildOnReturnings.UpdateOneAsync(databaseBuildOnReturning =>
                databaseBuildOnReturning.Id == buildOnReturningId,
                update
            );

            await _notificationService.NotifyBuildOnReturningRefusedByAdmin(builderUser.Email, builderUser.FirstName, reason);
        }

        public async Task RefuseReturningFromCoach(string currentUserId, string buildOnReturningId, string reason)
        {
            var buildOnReturning = await GetReturning(buildOnReturningId);

            if (buildOnReturning == null) throw new Exception("It seems that the returning doesn't exist");

            Coach coach = await _coachsService.GetCoachFromAdminAsync(currentUserId);
            Project project = await _projectsService.GetProjectFromIdAsync(buildOnReturning.ProjectId);

            if (coach == null) throw new UnauthorizedAccessException("You are not a coach");
            if (project == null) throw new Exception("The project doesn't exist");

            Coach builderCoach = await _buildersService.GetCoachForBuilderFromAdminAsync(project.BuilderId);

            if(coach.Id != builderCoach?.Id) throw new UnauthorizedAccessException("You are not the coach of the builder who submited the returning");

            User builderUser = await _buildersService.GetUserFromAdminAsync(project.BuilderId);

            if (builderUser == null) throw new Exception("Their is no user for this project...");

            buildOnReturning.Status = BuildOnReturningStatus.Refused;

            await _buildOnReturnings.ReplaceOneAsync(databaseReturning =>
                databaseReturning.Id == buildOnReturning.Id,
                buildOnReturning
            );

            await _notificationService.NotifyBuildOnReturningRefusedByCoach(builderUser.Email, builderUser.FirstName, reason);
        }

        // Validating step
        public async Task ValidateBuildOnStepFromAdmin(string projectId, string buildOnStepId)
        {
            var project = await _projectsService.GetProjectFromIdAsync(projectId);

            if (project == null) throw new Exception("The project doesn't exist");

            BuildOnReturning returning = new BuildOnReturning()
            {
                ProjectId = projectId,
                BuildOnStepId = buildOnStepId,
                Type = BuildOnReturningType.Comment,
                Status = BuildOnReturningStatus.WaitingCoach,
                Comment = "Cette étape a été validé sans preuve"
            };

            await _buildOnReturnings.InsertOneAsync(returning);
        }

        public async Task ValidateBuildOnStepFromCoach(string currentUserId, string projectId, string buildOnStepId)
        {
            var project = await _projectsService.GetProjectFromIdAsync(projectId);

            if (project == null) throw new Exception("The project doesn't exist");

            Coach coach = await _coachsService.GetCoachFromAdminAsync(currentUserId);

            if (coach == null) throw new UnauthorizedAccessException("You are not a coach");

            Coach builderCoach = await _buildersService.GetCoachForBuilderFromAdminAsync(project.BuilderId);

            if (coach.Id != builderCoach?.Id) throw new UnauthorizedAccessException("You are not the coach of the builder you wan't to validate step");

            BuildOnReturning returning = new BuildOnReturning()
            {
                ProjectId = projectId,
                BuildOnStepId = buildOnStepId,
                Type = BuildOnReturningType.Comment,
                Status = BuildOnReturningStatus.WaitingAdmin,
                Comment = "Cette étape a été validé sans preuve"
            };

            await _buildOnReturnings.InsertOneAsync(returning);
        }

        private async Task<BuildOn> GetBuildOn(string buildOnId)
        {
            var buildOn = await _buildOns.FindAsync(databaseBuildOn =>
                databaseBuildOn.Id == buildOnId
            );

            return await buildOn.FirstOrDefaultAsync();
        }

        private async Task<BuildOnStep> GetBuildOnStep(string buildOnStepId)
        {
            var buildOnStep = await _buildOnSteps.FindAsync(databaseBuildOnStep =>
                databaseBuildOnStep.Id == buildOnStepId
            );

            return await buildOnStep.FirstOrDefaultAsync();
        }

        private async Task<BuildOnReturning> GetReturning(string returningId)
        {
            var returning = await _buildOnReturnings.FindAsync(databaseReturning =>
                databaseReturning.Id == returningId
            );

            return await returning.FirstOrDefaultAsync();
        }

        private async Task<BuildOn> CreateBuildOnAsync(int index, BuildOnManageModel buildOnManageModel)
        {
            BuildOn buildOn = new BuildOn()
            {
                Index = index,

                Name = buildOnManageModel.Name,
                Description = buildOnManageModel.Description
            };

            await _buildOns.InsertOneAsync(buildOn);

            // We directly update the build-on to save the image
            buildOn = await UpdateBuildOnAsync(buildOn.Id, index, buildOnManageModel);

            return buildOn;
        }

        private async Task<BuildOnStep> CreateBuildOnStepAsync(string buildOnId, int index, BuildOnStepManageModel buildOnStepManageModel)
        {
            BuildOnStep buildOnStep = new BuildOnStep()
            {
                BuildOnId = buildOnId,
                Index = index,

                Name = buildOnStepManageModel.Name,
                Description = buildOnStepManageModel.Description,
                ReturningType = buildOnStepManageModel.ReturningType,
                ReturningDescription = buildOnStepManageModel.ReturningDescription,
                ReturningLink = buildOnStepManageModel.ReturningLink
            };

            await _buildOnSteps.InsertOneAsync(buildOnStep);

            // We directly update the build-on step to save the image
            buildOnStep = await UpdateBuildOnStepAsync(buildOnId, buildOnStep.Id, index, buildOnStepManageModel);

            return buildOnStep;
        }

        private async Task<BuildOn> UpdateBuildOnAsync(string buildOnId, int index, BuildOnManageModel buildOnManageModel)
        {
            var update = Builders<BuildOn>.Update
                .Set(dbBuildOn => dbBuildOn.Index, index)
                .Set(dbBuildOn => dbBuildOn.Name, buildOnManageModel.Name)
                .Set(dbBuildOn => dbBuildOn.Description, buildOnManageModel.Description);

            string fileId = "";

            if (buildOnManageModel.Image != null && buildOnManageModel.Image.Length >= 1)
            {
                fileId = await _filesService.UploadFile($"buildon_{buildOnId}", buildOnManageModel.Image);
                update = update.Set(dbBuildOn => dbBuildOn.ImageId, fileId);
            }

            await _buildOns.UpdateOneAsync(databaseBuildOn =>
                databaseBuildOn.Id == buildOnId,
                update
            );

            return new BuildOn()
            {
                Id = buildOnId,
                ImageId = fileId,

                Name = buildOnManageModel.Name,
                Description = buildOnManageModel.Description,
            };
        }

        private async Task<BuildOnStep> UpdateBuildOnStepAsync(string buildOnId, string buildonStepId, int index, BuildOnStepManageModel buildOnStepManageModel)
        {
            var update = Builders<BuildOnStep>.Update
                .Set(dbBuildOnStep => dbBuildOnStep.Index, index)
                .Set(dbBuildOnStep => dbBuildOnStep.Name, buildOnStepManageModel.Name)
                .Set(dbBuildOnStep => dbBuildOnStep.Description, buildOnStepManageModel.Description)
                .Set(dbBuildOnStep => dbBuildOnStep.ReturningType, buildOnStepManageModel.ReturningType)
                .Set(dbBuildOnStep => dbBuildOnStep.ReturningDescription, buildOnStepManageModel.ReturningDescription)
                .Set(dbBuildOnStep => dbBuildOnStep.ReturningLink, buildOnStepManageModel.ReturningLink);

            string fileId = "";

            if (buildOnStepManageModel.Image != null && buildOnStepManageModel.Image.Length >= 1)
            {
                fileId = await _filesService.UploadFile($"buildonstep_{buildonStepId}", buildOnStepManageModel.Image);
                update = update.Set(dbBuildOnStep => dbBuildOnStep.ImageId, fileId);
            }

            await _buildOnSteps.UpdateOneAsync(databaseBuildOnStep =>
                databaseBuildOnStep.Id == buildonStepId,
                update
            );

            return new BuildOnStep()
            {
                Id = buildonStepId,
                BuildOnId = buildOnId,
                ImageId = fileId,

                Name = buildOnStepManageModel.Name,
                Description = buildOnStepManageModel.Description,
                ReturningType = buildOnStepManageModel.ReturningType,
                ReturningDescription = buildOnStepManageModel.ReturningDescription,
                ReturningLink = buildOnStepManageModel.ReturningLink
            };
        }

        private async Task IncrementProjectBuildOnStep(Project project)
        {
            // We need the Builder for the future notification
            User builderUser = await _buildersService.GetUserFromAdminAsync(project.BuilderId);

            if (builderUser == null) throw new Exception("The builder of this project doesn't exist...");

            List<BuildOn> buildOns = await GetAllAsync();

            if (buildOns.Count < 1) throw new Exception("Build-up have no build-ons yet...");

            string newBuildOnId;
            string newBuildOnStepId;

            if (project.CurrentBuildOn == null)
            {
                newBuildOnId = buildOns.First().Id;

                var buildOnSteps = await GetAllStepsAsync(buildOns.First().Id);

                if (buildOnSteps.Count < 1) return;

                newBuildOnStepId = buildOnSteps[1].Id;
            }
            else
            {
                BuildOn currentBuildOn = await GetBuildOn(project.CurrentBuildOn);
                BuildOnStep currentStep = await GetBuildOnStep(project.CurrentBuildOnStep);

                List<BuildOnStep> currentBuildOnSteps = await GetAllStepsAsync(currentBuildOn.Id);

                // Case 1: we are at the last step of the current build-on
                if (currentStep.Index == currentBuildOnSteps.Count - 1)
                {
                    // If we are at the last build-on, no need to continue
                    if (currentBuildOn.Index == buildOns.Count - 1)
                    {
                        newBuildOnId = null;
                        newBuildOnStepId = null;
                    }
                    else
                    {
                        BuildOn newBuildOn = buildOns[currentBuildOn.Index + 1];
                        List<BuildOnStep> newSteps = await GetAllStepsAsync(newBuildOn.Id);

                        if (newSteps.Count < 1) return;

                        newBuildOnId = newBuildOn.Id;
                        newBuildOnStepId = newSteps.First().Id;
                    }
                }
                // Case 2: we wan't the next from the current build-on
                else
                {
                    newBuildOnId = currentBuildOn.Id;
                    newBuildOnStepId = currentBuildOnSteps[currentStep.Index + 1].Id;
                }
            }

            await _projectsService.UpdateProjectBuildOnStep(project.Id, newBuildOnId, newBuildOnStepId);
            await _notificationService.NotifyBuildonStepValidated(builderUser.Email);
        }

    }
}
