﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Settings.Interfaces
{
    public interface IMailCredentials
    {
        public string Server { get; set; }
        public int Port { get; set; }

        public string BuilderUser { get; set; }
        public string BuilderPassword { get; set; }

        public string CoachUser { get; set; }
        public string CoachPassword { get; set; }
    }
}
