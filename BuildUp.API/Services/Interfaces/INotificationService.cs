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
        Task NotifyPreselectionBuilder(string email, string fullName);
    }
}
