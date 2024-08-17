using PasswordUpdaterJob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordUpdaterJob.Services.Interfaces
{
    public interface IUser
    {
        Task<bool> UpdateStatus(int  userId, string status);
        Task<List<UsersModel>> FetchUsersWithOutdatedPasswords();
    }
}
