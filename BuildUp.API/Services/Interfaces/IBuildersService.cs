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

        Task<string> RegisterBuilderAsync(BuilderRegisterModel builderRegisterModel);

        Task AssignCoach(string coachId, string builderId);

        Task<List<Builder>> GetCandidatingBuildersAsync();
        Task<List<Builder>> GetActiveBuildersAsync();
    }
}
