using BuildUp.API.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface IFormsService
    {
        Task RegisterFormToDatabseAsync(string userId, List<BuildupFormQA> qas);
        Task<List<BuildupFormQA>> GetFormQAsAsync(string userId);

        Task<string> GetAnswerForQuestionAsync(string userId, string question);
    }
}
