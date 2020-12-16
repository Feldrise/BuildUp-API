using BuildUp.API.Entities.BuildOn;
using BuildUp.API.Models;
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

        // Proofs
        Task<FileModel> GetReturningFileFromAdmin(string buildOnReturningId);

        Task RefuseReturningFromAdmin(string buildOnReturningId);
        Task AcceptReturningFromAdmin(string projectId, string buildOnReturningId);
        Task ValidateBuildOnStepFromAdmin(string projectId, string buildOnStepId);


        Task<List<BuildOnReturning>> GetReturningsFromAdmin(string projectId);
        Task<List<BuildOnReturning>> GetReturningFromBuilder(string currentUserId, string projectId);
        Task<List<BuildOnReturning>> GetReturningFromCoach(string currentUserId, string projectId);

        Task<string> SendReturningAsync(string currentUserId, string projectId, BuildOnReturningSubmitModel buildOnReturningSubmitModel);
    }
}
