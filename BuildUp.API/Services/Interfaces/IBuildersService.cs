using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
using BuildUp.API.Models.Builders;

namespace BuildUp.API.Services.Interfaces
{
    public interface IBuildersService
    {
        Task<Builder> GetBuilderFromAdminAsync(string userId);
        Task<Builder> GetBuilderFromCoachAsync(string currentUserId, string userId);
        Task<Builder> GetBuilderFromBuilderAsync(string currentUserId, string userId);

        Task<User> GetUserFromAdminAsync(string builderId);
        Task<User> GetUserFromBuilderAsync(string currentUserId, string builderId);


        Task<Coach> GetCoachForBuilderFromAdminAsync(string builderId);
        Task<Coach> GetCoachForBuilderFromBuilderAsync(string currentUserId, string builderId);

        Task<List<BuildupFormQA>> GetBuilderFormFromAdminAsync(string builderId);
        Task<List<BuildupFormQA>> GetBuilderFormFromCoachAsync(string currentUserId, string builderId);
        Task<List<BuildupFormQA>> GetBuilderFormFromBuilderAsync(string currentUserId, string builderId);


        Task<string> RegisterBuilderAsync(BuilderRegisterModel builderRegisterModel);
        Task UpdateBuilderFromAdminAsync(string builderId, BuilderUpdateModel builderUpdateModel);

        Task UpdateBuilderFromBuilderAsync(string currentUserId, string builderId, BuilderUpdateModel builderUpdateModel);

        Task RefuseBuilderAsync(string builderId);
        Task AssignCoachAsync(string coachId, string builderId);

        Task<List<Builder>> GetCandidatingBuildersAsync();
        Task<List<Builder>> GetActiveBuildersAsync();
    }
}
