﻿using BuildUp.API.Entities;
using BuildUp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface IProjectsService
    {
        Task<Project> GetProjectAsync(string builderId);

        Task<string> SubmitProjectAsync(ProjectSubmitModel projectSubmitModel);
    }
}