using BuildUp.API.Entities.BuildOn;
using BuildUp.API.Models.BuildOn;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services
{
    public class BuildOnsService : IBuildOnsService
    {
        private readonly IMongoCollection<BuildOn> _buildOns;
        private readonly IMongoCollection<BuildOnStep> _buildOnSteps;

        private readonly IFilesService _filesService;

        public BuildOnsService(IMongoSettings mongoSettings, IFilesService filesService)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _buildOns = database.GetCollection<BuildOn>("buildons");
            _buildOnSteps = database.GetCollection<BuildOnStep>("buildon_steps");

            _filesService = filesService;
        }

        public async Task<List<BuildOn>> GetAllAsync()
        {
            return await (await _buildOns.FindAsync(databaseBuildOn =>
                true,
                new FindOptions<BuildOn>()
                {
                    Sort = Builders<BuildOn>.Sort.Ascending("index")
                }
            )).ToListAsync();
        }

        public async Task<byte[]> GetImageForBuildOnAsync(string buildOnId)
        {
            BuildOn buildOn = await GetBuildOn(buildOnId);

            if (buildOn == null || buildOn.ImageId == null) { return null;  }

            return await _filesService.GetFile(buildOn.ImageId);
        }
        public async Task<byte[]> GetImageForBuildOnStepAsync(string buildOnStepId)
        {
            BuildOnStep buildOnStep = await GetBuildOnStep(buildOnStepId);

            if (buildOnStep == null || buildOnStep.ImageId == null) { return null; }

            return await _filesService.GetFile(buildOnStep.ImageId);
        }

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

        public async Task DeleteBuildOnAsync(string buildonId)
        {
            await _buildOns.DeleteOneAsync(databaseBuildOn =>
                databaseBuildOn.Id == buildonId
            );
        }

        public async Task<List<BuildOnStep>> GetAllStepsAsync(string buildonId)
        {
            return await (await _buildOnSteps.FindAsync(databaseBuildOnStep =>
                databaseBuildOnStep.BuildOnId == buildonId,
                new FindOptions<BuildOnStep>()
                {
                    Sort = Builders<BuildOnStep>.Sort.Ascending("index")
                }
            )).ToListAsync();
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

        public async Task DeleteBuildOnStepAsync(string buildonStepId)
        {
            await _buildOnSteps.DeleteOneAsync(databaseBuildOnStep =>
                databaseBuildOnStep.Id == buildonStepId
            );
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
            buildOn = await UpdateBuildOnAsync(buildOn.Id, index, buildOnManageModel);

            return buildOn;
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
            buildOnStep = await UpdateBuildOnStepAsync(buildOnId, buildOnStep.Id, index, buildOnStepManageModel);

            return buildOnStep;
        }

        private async Task<BuildOnStep> UpdateBuildOnStepAsync(string buildOnId, string buildonStepId, int index, BuildOnStepManageModel buildOnStepManageModel)
        {
            var update = Builders<BuildOnStep>.Update
                .Set(dbBuildOnStep => dbBuildOnStep.Index, index)
                .Set(dbBuildOnStep => dbBuildOnStep.Name, buildOnStepManageModel.Name)
                .Set(dbBuildOnStep => dbBuildOnStep.Description, buildOnStepManageModel.Description)
                .Set(dbBuildOnStep => dbBuildOnStep.ReturningType, buildOnStepManageModel.ReturningDescription)
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

    }
}
