using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BuildUp.API.Entities;
using BuildUp.API.Entities.Form;
using BuildUp.API.Models;
using BuildUp.API.Models.Builders;
using BuildUp.API.Models.MeetingReports;
using BuildUp.API.Models.Projects;

namespace BuildUp.API.Services.Interfaces
{
    public interface IBuildersService
    {
        // Getting the builder
        Task<Builder> GetBuilderFromAdminAsync(string userId);
        Task<Builder> GetBuilderFromCoachAsync(string currentUserId, string userId);
        Task<Builder> GetBuilderFromBuilderAsync(string currentUserId, string userId);

        // Getting the user corresponding to the builder
        Task<User> GetUserFromAdminAsync(string builderId);
        Task<User> GetUserFromCoachAsync(string currentUserId, string builderId);
        Task<User> GetUserFromBuilderAsync(string currentUserId, string builderId);

        // Getting "specific" builder
        Task<List<Builder>> GetCandidatingBuildersAsync();
        Task<List<Builder>> GetActiveBuildersAsync();

        // Register the builder
        Task<string> RegisterBuilderAsync(BuilderRegisterModel builderRegisterModel);
        Task<string> SubmitProjectAsync(ProjectSubmitModel projectSubmitModel);

        // Updating the builder
        Task UpdateBuilderFromAdminAsync(string builderId, BuilderUpdateModel builderUpdateModel);

        Task UpdateBuilderFromBuilderAsync(string currentUserId, string builderId, BuilderUpdateModel builderUpdateModel);

        Task UpdateProjectFromAdmin(string projectId, ProjectUpdateModel projectUpdateModel);
        Task UpdateProjectFromBuilder(string currentUserId, string builderId, string projectId, ProjectUpdateModel projectUpdateModel);

        // Refusing the builder
        Task RefuseBuilderAsync(string builderId);

        // Assigning coach to the builder
        Task AssignCoachAsync(string coachId, string builderId);

        // Getting the builder's coach
        Task<Coach> GetCoachForBuilderFromAdminAsync(string builderId);
        Task<Coach> GetCoachForBuilderFromBuilderAsync(string currentUserId, string builderId);

        // Getting the builder's form
        Task<List<BuildupFormQA>> GetBuilderFormFromAdminAsync(string builderId);
        Task<List<BuildupFormQA>> GetBuilderFormFromCoachAsync(string currentUserId, string builderId);
        Task<List<BuildupFormQA>> GetBuilderFormFromBuilderAsync(string currentUserId, string builderId);

        // Getting the builder's project
        Task<Project> GetBuilderProjectFromAdminAsync(string builderId);
        Task<Project> GetBuilderProjectFromCoachAsync(string currentUserId, string builderId);
        Task<Project> GetBuilderProjectFromBuilderAsync(string currentUserId, string builderId);
        
        // Getting the builder's card
        Task<byte[]> GetBuilderCardAsync(string builderId);

        // Signging the PDF for the integration
        Task<bool> SignFicheIntegrationAsync(string currentUserId, string builderId);

        // Manage meeting reports
        Task<string> CreateMeetingReportAsync(string currentUserId, string builderId, CreateMeetingReportModel toCreate);
        Task<List<MeetingReport>> GetMeetingReportsFromAdminAsync(string builderId);
        Task<List<MeetingReport>> GetMeetingReportsFromCoachAsync(string currentUserId, string builderId);
        Task<List<MeetingReport>> GetMeetingReportsFromBuilderAsync(string currentUserId, string builderId);
    }
}
