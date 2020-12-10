using BuildUp.API.Entities;
using BuildUp.API.Models;
using BuildUp.API.Models.Projects;
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

        public async Task<Project> GetProjectFromIdAsync(string projectId)
        {
            return await (await _projects.FindAsync(databaseProject =>
                databaseProject.Id == projectId
            )).FirstOrDefaultAsync();
        }

        public async Task<string> SubmitProjectAsync(ProjectSubmitModel projectSubmitModel)
        {
            Project project = new Project()
            {
                BuilderId = projectSubmitModel.BuilderId,

                Name = projectSubmitModel.Name,
                Categorie = projectSubmitModel.Categorie,
                Description = projectSubmitModel.Description,
                Keywords = projectSubmitModel.Keywords,
                Team = projectSubmitModel.Team,

                LaunchDate = projectSubmitModel.LaunchDate,
                IsLucratif = projectSubmitModel.IsLucratif,
                IsDeclared = projectSubmitModel.IsDeclared,

                CurrentBuildOn = null,
                CurrentBuildOnStep = null

            };

            await _projects.InsertOneAsync(project);

            return project.Id;
        }

        public async Task UpdateProjectAsync(string projectId, ProjectUpdateModel projectUpdateModel)
        {
            var update = Builders<Project>.Update
                .Set(databaseProject => databaseProject.Name, projectUpdateModel.Name)
                .Set(databaseProject => databaseProject.Categorie, projectUpdateModel.Categorie)
                .Set(databaseProject => databaseProject.Description, projectUpdateModel.Description)
                .Set(databaseProject => databaseProject.Keywords, projectUpdateModel.Keywords)
                .Set(databaseProject => databaseProject.Team, projectUpdateModel.Team)
                .Set(databaseProject => databaseProject.LaunchDate, projectUpdateModel.LaunchDate)
                .Set(databaseProject => databaseProject.IsLucratif, projectUpdateModel.IsLucratif)
                .Set(databaseProject => databaseProject.IsDeclared, projectUpdateModel.IsDeclared);

            await _projects.UpdateOneAsync(databaseProject =>
               databaseProject.Id == projectId,
               update
           );
        }

        public async Task UpdateProjectBuildOnStep(string projectId, string newBuildOn, string newBuildOnStep)
        {
            var update = Builders<Project>.Update
                .Set(databaseProject => databaseProject.CurrentBuildOn, newBuildOn)
                .Set(databaseProject => databaseProject.CurrentBuildOnStep, newBuildOnStep);

            await _projects.UpdateOneAsync(databaseProject =>
               databaseProject.Id == projectId,
               update
           );
        }
    }
}
