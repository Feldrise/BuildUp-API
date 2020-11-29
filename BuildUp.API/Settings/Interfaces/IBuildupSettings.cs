using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Settings.Interfaces
{
    public interface IBuildupSettings
    {
        string ApiSecret { get; set; }
    }
}
