using BuildUp.API.Entities;
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

        Task<string> RegisterCoachAsync(CoachRegisterModel builderRegisterModel);

        Task<List<Coach>> GetCandidatingCoachsAsync();
        Task<List<Coach>> GetActiveCoachsAsync();
    }
}
