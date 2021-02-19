using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
using BuildUp.API.Entities.Notification;
using BuildUp.API.Entities.Notification.CoachRequest;
using BuildUp.API.Models.Coachs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface ICoachsService
    {
        // Getting the coach
        Task<Coach> GetCoachFromAdminAsync(string userId);
        Task<Coach> GetCoachFromCoachAsync(string currentUserId, string userId);
        Task<List<Coach>> GetAllCoachsAsync();

        // Getting the user corresponding to the coach
        Task<User> GetUserFromAdminAsync(string coachId);
        Task<User> GetUserFromCoachAsync(string currentUserId, string coachId);
        Task<User> GetUserFromBuilderAsync(string currentUserId, string coachId);

        // Getting "specific" coachs
        Task<List<Coach>> GetCandidatingCoachsAsync();
        Task<List<Coach>> GetActiveCoachsAsync();
        Task<List<AvailableCoachModel>> GetAvailableCoachAsync();

        // Register a coach
        Task<string> RegisterCoachAsync(CoachRegisterModel builderRegisterModel);

        // Updating coach
        Task UpdateCoachFromAdminAsync(string coachId, CoachUpdateModel coachUpdateModel);
        Task UpdateCoachFromCoachAsync(string currentUserId, string coachId, CoachUpdateModel coachUpdateModel);

        // Refusing the coach
        Task RefuseCoachAsync(string coachId);

        // Getting the coach's builders
        Task<List<Builder>> GetBuildersFromAdminAsync(string coachId);
        Task<List<Builder>> GetBuildersFromCoachAsync(string currentUserId, string coachId);

        // Getting the coach's form
        Task<List<BuildupFormQA>> GetCoachFormFromAdminAsync(string coachId);
        Task<List<BuildupFormQA>> GetCoachFormFromCoachAsync(string currentUserId, string coachId);

        // Getting the coach's card
        Task<byte[]> GetCoachCardAsync(string coachId);


        // Signging the PDF for the integration
        Task<bool> SignFicheIntegrationAsync(string currentUserId, string coachId);

        // Manage coach requests
        Task<List<CoachRequest>> GetCoachRequestsAsync(string currentUserId, string coachId);
        Task AcceptCoachRequestAsync(string currentUserId, string coachId, string requestId);
        Task RefuseCoachRequestAsync(string currentUserId, string coachId, string requestId);
    }
}
