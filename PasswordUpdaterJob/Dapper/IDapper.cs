using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordUpdaterJob.Dapper
{
    public interface IDapper : IDisposable
    {
        Task<T> QuerySingleAsync<T>(string sql, DynamicParameters? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string sql, DynamicParameters? parameters = null, int? commandTimeout = null, CommandType commandType = CommandType.Text);
    }
}
