using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Models.BuildOn
{
    public class BuildOnReturningRefusingModel
    {
        /// <summary>
        /// The reason why it is refused
        /// </summary>
        /// <example>Je crois que cette fois c'est toi qui n'a pas assez dormi Lucas ❤</example>
        public string Reason { get; set; }
    }
}
