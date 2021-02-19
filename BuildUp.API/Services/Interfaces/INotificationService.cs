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
        
        // Coachs
        Task NotifyPreselectionCoach(string email, string name);
        Task NotifyAcceptationCoach(string email);
        Task NotifySignedIntegrationPaperCoach(string coachId, string email, string name);

        Task NotifyBuilderChoosedCoach(string email);

        Task<List<CoachNotification>> GetCoachNotificationsAsync(string coachId);
        Task MakeCoachNotificationReadAsync(string coachId, string notificationId);
    }
}
