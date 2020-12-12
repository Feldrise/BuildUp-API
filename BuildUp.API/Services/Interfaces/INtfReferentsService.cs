using BuildUp.API.Entities;
using BuildUp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface INtfReferentsService
    {
        Task<NtfReferent> GetOneAsync(string id);
        Task<List<NtfReferent>> GetAllAsync();

        Task<string> AddOneAsync(NtfReferentManageModel ntfReferentManageModel);
        Task UpdateOneAsync(string id, NtfReferentManageModel ntfReferentManageModel);
        Task DeleteReferentAsync(string id);
    }
}
