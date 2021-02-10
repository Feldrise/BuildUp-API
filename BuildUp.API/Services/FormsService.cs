using BuildUp.API.Entities.Form;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services
{
    public class FormsService : IFormsService
    {
        private readonly IMongoCollection<BuildupForm> _forms;
        private readonly IMongoCollection<BuildupFormQA> _formsQAs;

        public FormsService(IMongoSettings mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _forms = database.GetCollection<BuildupForm>("forms");
            _formsQAs = database.GetCollection<BuildupFormQA>("forms_qa");
        }

        public async Task RegisterFormToDatabseAsync(string userId, List<BuildupFormQA> qas)
        {
            BuildupForm newForm = new BuildupForm()
            {
                UserId = userId
            };

            await _forms.InsertOneAsync(newForm);

            for (int i = 0; i < qas.Count; ++i)
            {
                qas[i].FormId = newForm.Id;
                qas[i].Index = i;
            }

            await _formsQAs.InsertManyAsync(qas);
        }

        public async Task<List<BuildupFormQA>> GetFormQAsAsync(string userId)
        {
            BuildupForm form = await GetFormAsync(userId);

            if (form == null) return null;

            List<BuildupFormQA> formQAs = await (await _formsQAs.FindAsync(databaseQA =>
                databaseQA.FormId == form.Id,
                new FindOptions<BuildupFormQA>()
                {
                    Sort = Builders<BuildupFormQA>.Sort.Ascending("index")
                }
            )).ToListAsync();

            return formQAs;
        }

        public async Task<string> GetAnswerForQuestionAsync(string userId, string question)
        {
            BuildupForm form = await GetFormAsync(userId);

            if (form == null) return null;

            BuildupFormQA formQA = await (await _formsQAs.FindAsync(databaseFormQA =>
                databaseFormQA.FormId == form.Id &&
                databaseFormQA.Question == question
            )).FirstOrDefaultAsync();

            return formQA?.Answer;
        }

        private async Task<BuildupForm> GetFormAsync(string userId)
        {
            return await (await _forms.FindAsync(databaseForm =>
                databaseForm.UserId == userId
            )).FirstOrDefaultAsync();
        }
    }
}
