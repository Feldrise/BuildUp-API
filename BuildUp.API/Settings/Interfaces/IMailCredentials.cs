using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Settings.Interfaces
{
    public interface IMailCredentials
    {
        public string Server { get; set; }
        public int Port { get; set; }

        public string User { get; set; }
        public string Password { get; set; }
    }
}
