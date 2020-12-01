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
    public class ProjectsService : IProjectsService
    {
        private readonly IMongoCollection<Project> _projects;

        public ProjectsService(IMongoSettings mongoSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.ConnectionString);
            var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            _projects = database.GetCollection<Project>("projects");
        }

        public async Task<Project> GetProjectAsync(string builderId)
        {
            return await (await _projects.FindAsync(databaseProject =>
                databaseProject.BuilderId == builderId
            )).FirstOrDefaultAsync();
        }

        public async Task<string> SubmitProjectAsync(ProjectSubmitModel projectSubmitModel)
        {
            Project project = new Project()
            {
                BuilderId = projectSubmitModel.BuilderId,

                Name = projectSubmitModel.Name,
                Description = projectSubmitModel.Description,
                Keywords = projectSubmitModel.Keywords,
                Team = projectSubmitModel.Team,

                LaunchDate = projectSubmitModel.LaunchDate,
                IsLucratif = projectSubmitModel.IsLucratif,

                CurrentBuildOn = null,
                CurrentBuildOnStep = null

            };

            await _projects.InsertOneAsync(project);

            return project.Id;


        }
    }
}
