using BuildUp.API.Entities.Form;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
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
        private readonly IMongoCollection<BuildupFormQA> _formsQA;

        public FormsService(IMongoSettings mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _forms = database.GetCollection<BuildupForm>("forms");
            _formsQA = database.GetCollection<BuildupFormQA>("forms_qa");
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

            await _formsQA.InsertManyAsync(qas);
        }
    }
}
