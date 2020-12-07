using BuildUp.API.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildUp.API.Services.Interfaces
{
    public interface IUsersService
    {
        Task<byte[]> GetProfilePicture(string userId);

        Task UpdateUserAsync(string userId, UserUpdateModel userUpdateModel);
    }
}
