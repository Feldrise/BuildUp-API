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
        // Getting build-ons and steps
        Task<List<BuildOn>> GetAllAsync();
        Task<List<BuildOnStep>> GetAllStepsAsync(string buildonId);

        Task<BuildOnStep> GetBuildOnStepAsync(string buildOnStepId);
        Task<BuildOnStep> GetFirstBuildOnStepAsync();

        // Updating build-ons and steps
        Task<List<BuildOn>> UpdateBuildOnsAsync(List<BuildOnManageModel> buildOnManageModels);
        Task<List<BuildOnStep>> UpdateBuildOnStepsAsync(string buildOnId, List<BuildOnStepManageModel> buildOnStepManageModels);

        // Deleting build-ons and steps
        Task DeleteBuildOnAsync(string buildonId);
        Task DeleteBuildOnStepAsync(string buildonStepId);

        // Getting image for build-ons and steps
        Task<byte[]> GetImageForBuildOnAsync(string buildOnId);

        Task<byte[]> GetImageForBuildOnStepAsync(string buildOnStepId);

        // Getting the proofs
        Task<List<BuildOnReturning>> GetReturningsFromAdmin(string projectId);
        Task<List<BuildOnReturning>> GetReturningFromBuilder(string currentUserId, string projectId);
        Task<List<BuildOnReturning>> GetReturningFromCoach(string currentUserId, string projectId);

        // Getting proof file
        Task<FileModel> GetReturningFileFromAdmin(string buildOnReturningId);
        Task<FileModel> GetReturningFileFromCoach(string currentUserId, string buildOnReturningId);
        Task<FileModel> GetReturningFileFromBuilder(string currentUserId, string buildOnReturningId);

        // Sending proof
        Task<string> SendReturningAsync(string currentUserId, string projectId, BuildOnReturningSubmitModel buildOnReturningSubmitModel);

        // Accepting proofs
        Task AcceptReturningFromAdmin(string projectId, string buildOnReturningId);
        Task AcceptReturningFromCoach(string currentUserId, string projectId, string buildOnReturningId);

        // Refusing proofs
        Task RefuseReturningFromAdmin(string buildOnReturningId, string reason);
        Task RefuseReturningFromCoach(string currentUserId, string buildOnReturningId, string reason);
        
        // Validating step
        Task ValidateBuildOnStepFromAdmin(string projectId, string buildOnStepId);
        Task ValidateBuildOnStepFromCoach(string currentUserId, string projectId, string buildOnStepId);

    }
}
