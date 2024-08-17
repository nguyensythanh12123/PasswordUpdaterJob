using Dapper;
using PasswordUpdaterJob.Dapper;
using PasswordUpdaterJob.Models;
using PasswordUpdaterJob.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordUpdaterJob.Services.Repositories
{
    public class UserRepository : IUser
    {
        private readonly IDapper _dapper;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IDapper dapper, ILogger<UserRepository> logger)
        {
            _dapper = dapper;
            _logger = logger;
        }

        public async Task<List<UsersModel>> FetchUsersWithOutdatedPasswords()
        {
            try
            {
                _logger.LogInformation("-------------Start UserRepository/FetchUsersWithOutdatedPasswords---------");
                string sql = "SELECT * FROM Users " +
                            "WHERE Status != 'REQUIRE_CHANGE_PWD' " +
                            "AND LastUpdatePwd < DATEADD(MONTH, -6, GETDATE())";
                var users = await _dapper.QueryAsync<UsersModel>(sql);
                return users.ToList();

            }catch (Exception ex)
            {
                _logger.LogError("UserRepository/FetchUsersWithOutdatedPasswords: {e}", ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateStatus(int userId, string status)
        {
            try
            {
                _logger.LogInformation("-------------Start UserRepository/UpdateStatus---------");
                DynamicParameters param = new();
                param.Add("@Id", userId);
                param.Add("@status", status);
                string sql = "UPDATE top (1) Users SET Status = @status WHERE Id = @Id";
                return await _dapper.ExecuteAsync(sql, param) > 0;

            }catch(Exception ex)
            {
                _logger.LogError("UserRepository/UpdateStatus: {e}", ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
