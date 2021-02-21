using BuildUp.API.Entities.Notification;
using BuildUp.API.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifieAccountCreationAsync(RegisterModel registerModel, string password);
        
        // Builders
        Task NotifyPreselectionBuilder(string email, string name);
        Task NotifyAdminMeetingValidatedBuilder(string email, string name);
        Task NotifyCoachAcceptBuilder(string email, string name);
        Task NotifySignedIntegrationPaperBuilder(string builderId, string email, string name);


        // Coachs
        Task NotifyPreselectionCoach(string email, string name);
        Task NotifyAcceptationCoach(string email);
        Task NotifySignedIntegrationPaperCoach(string coachId, string email, string name);

        Task NotifyBuilderChoosedCoach(string email);

        // Build-ons
        Task NotifyBuildOnReturningSubmited(string coachId, string coachMail);
        Task NotifyBuildonStepValidated(string builderMail);

        Task NotifyBuildOnReturningRefusedByCoach(string builderMail, string builderName, string reason);
        Task NotifyBuildOnReturningRefusedByAdmin(string builderMail, string builderName, string reason);
        
        // Admin
        Task NotifyBuilderCandidating();
        Task NotifyCoachCandidating();

        Task NotifyBuilderIntegrationSucess();
        Task NotifyCoachIntegrationSucess();

        // News
        Task CreateCoachNotification(string coachId, string content);
        Task<List<CoachNotification>> GetCoachNotificationsAsync(string coachId);
        Task MakeCoachNotificationReadAsync(string coachId, string notificationId);
    }
}
