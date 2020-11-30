using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
using BuildUp.API.Models.Coachs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface ICoachsService
    {
        Task<Coach> GetCoachFromAdminAsync(string userId);
        Task<Coach> GetCoachFromCoachAsync(string currentUserId, string userId);
        Task<List<Coach>> GetAllCoachsAsync();

        Task<List<Builder>> GetBuildersFromAdminAsync(string coachId);
        Task<List<Builder>> GetBuildersFromCoachAsync(string currentUserId, string coachId);

        Task<List<BuildupFormQA>> GetCoachFormFromAdminAsync(string coachId);
        Task<List<BuildupFormQA>> GetCoachFormFromCoachAsync(string currentUserId, string coachId);

        Task<string> RegisterCoachAsync(CoachRegisterModel builderRegisterModel);

        Task UpdateCoachFromAdminAsync(string coachId, CoachUpdateModel coachUpdateModel);
        Task UpdateCoachFromCoachAsync(string currentUserId, string coachId, CoachUpdateModel coachUpdateModel);

        Task RefuseCoachAsync(string coachId);

        Task<List<Coach>> GetCandidatingCoachsAsync();
        Task<List<Coach>> GetActiveCoachsAsync();
    }
}
