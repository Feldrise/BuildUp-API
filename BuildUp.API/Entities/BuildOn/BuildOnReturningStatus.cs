using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Entities.BuildOn
{
    public static class BuildOnReturningStatus
    {
        public const string Validated = "Validated";
        public const string Waiting = "Waiting";
        public const string WaitingAdmin = "WaitingAdmin";
        public const string WaitingCoach = "WaitingCoach";
        public const string Refused = "Refused";
    }
}
