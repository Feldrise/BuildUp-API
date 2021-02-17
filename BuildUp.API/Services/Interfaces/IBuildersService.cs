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
        Task<Builder> GetBuilderFromAdminAsync(string userId);
        Task<Builder> GetBuilderFromCoachAsync(string currentUserId, string userId);
        Task<Builder> GetBuilderFromBuilderAsync(string currentUserId, string userId);

        Task<byte[]> GetBuilderCardAsync(string builderId);

        Task<User> GetUserFromAdminAsync(string builderId);
        Task<User> GetUserFromCoachAsync(string currentUserId, string builderId);
        Task<User> GetUserFromBuilderAsync(string currentUserId, string builderId);

        Task<Coach> GetCoachForBuilderFromAdminAsync(string builderId);
        Task<Coach> GetCoachForBuilderFromBuilderAsync(string currentUserId, string builderId);

        Task<List<BuildupFormQA>> GetBuilderFormFromAdminAsync(string builderId);
        Task<List<BuildupFormQA>> GetBuilderFormFromCoachAsync(string currentUserId, string builderId);
        Task<List<BuildupFormQA>> GetBuilderFormFromBuilderAsync(string currentUserId, string builderId);

        Task<Project> GetBuilderProjectFromAdminAsync(string builderId);
        Task<Project> GetBuilderProjectFromCoachAsync(string currentUserId, string builderId);
        Task<Project> GetBuilderProjectFromBuilderAsync(string currentUserId, string builderId);

        Task UpdateProjectFromAdmin(string projectId, ProjectUpdateModel projectUpdateModel);
        Task UpdateProjectFromBuilder(string currentUserId, string builderId, string projectId, ProjectUpdateModel projectUpdateModel);

        Task<string> RegisterBuilderAsync(BuilderRegisterModel builderRegisterModel);
        Task<string> SubmitProjectAsync(ProjectSubmitModel projectSubmitModel);
        Task<bool> SignFicheIntegrationAsync(string currentUserId, string builderId);

        Task UpdateBuilderFromAdminAsync(string builderId, BuilderUpdateModel builderUpdateModel);

        Task UpdateBuilderFromBuilderAsync(string currentUserId, string builderId, BuilderUpdateModel builderUpdateModel);

        Task RefuseBuilderAsync(string builderId);
        Task AssignCoachAsync(string coachId, string builderId);

        Task<List<Builder>> GetCandidatingBuildersAsync();
        Task<List<Builder>> GetActiveBuildersAsync();

        Task<string> CreateMeetingReportAsync(string currentUserId, CreateMeetingReportModel toCreate);
        Task<List<MeetingReport>> GetMeetingReportsFromAdminAsync(string builderId);
        Task<List<MeetingReport>> GetMeetingReportsFromCoachAsync(string currentUserId, string builderId);
        Task<List<MeetingReport>> GetMeetingReportsFromBuilderAsync(string currentUserId, string builderId);
    }
}
