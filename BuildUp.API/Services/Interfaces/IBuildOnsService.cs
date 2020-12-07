using BuildUp.API.Entities.BuildOn;
using BuildUp.API.Models.BuildOn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface IBuildOnsService
    {
        Task<List<BuildOn>> GetAllAsync();

        Task<byte[]> GetImageForBuildOnAsync(string buildOnId);

        Task<List<BuildOn>> UpdateBuildOnsAsync(List<BuildOnManageModel> buildOnManageModels);
        Task DeleteBuildOnAsync(string buildonId);

        // STEPS
        Task<List<BuildOnStep>> GetAllStepsAsync(string buildonId);
        Task<byte[]> GetImageForBuildOnStepAsync(string buildOnStepId);

        Task<List<BuildOnStep>> UpdateBuildOnStepsAsync(string buildOnId, List<BuildOnStepManageModel> buildOnStepManageModels);
        Task DeleteBuildOnStepAsync(string buildonStepId);
    }
}
