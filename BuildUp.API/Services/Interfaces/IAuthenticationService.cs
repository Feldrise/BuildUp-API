using BuildUp.API.Entities;
using BuildUp.API.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> LoginAsync(string username, string password);
        
        Task<string> RegisterAsync(RegisterModel userRegister);
        Task<string> RegisterAdminAsync(RegisterModel userRegister);
        Task<string> RegisterWithFormAsync(FormRegisterModel formRegister);


    }
}
