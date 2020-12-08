using BuildUp.API.Entities;
using BuildUp.API.Models;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services
{
    public class NtfReferentsService : INtfReferentsService
    {
        private readonly IMongoCollection<NtfReferent> _ntfReferents;

        public NtfReferentsService(IMongoSettings mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _ntfReferents = database.GetCollection<NtfReferent>("ntf_referents");
        }

        public async Task<NtfReferent> GetOneAsync(string id)
        {
            return await (await _ntfReferents.FindAsync(databaseNtfReferent =>
                databaseNtfReferent.Id == id
            )).FirstOrDefaultAsync();
        }

        public async Task<List<NtfReferent>> GetAllAsync()
        {
            return await (await _ntfReferents.FindAsync(databaseNtfReferent =>
                true
            )).ToListAsync();
        }

        public async Task<string> AddOneAsync(NtfReferentManageModel ntfReferentManageModel)
        {
            NtfReferent ntfReferent = new NtfReferent()
            {
                Name = ntfReferentManageModel.Name,
                Email = ntfReferentManageModel.Email,
                DiscordTag = ntfReferentManageModel.DiscordTag
            };

            await _ntfReferents.InsertOneAsync(ntfReferent);

            return ntfReferent.Id;
        }

        public async Task UpdateOneAsync(string id, NtfReferentManageModel ntfReferentManageModel)
        {
            var update = Builders<NtfReferent>.Update
                .Set(dbNtdReferent => dbNtdReferent.Name, ntfReferentManageModel.Name)
                .Set(dbNtdReferent => dbNtdReferent.Email, ntfReferentManageModel.Email)
                .Set(dbNtdReferent => dbNtdReferent.DiscordTag, ntfReferentManageModel.DiscordTag);

            await _ntfReferents.UpdateOneAsync(databaseNtfReferent =>
               databaseNtfReferent.Id == id,
               update
            );
        }
    }
}
