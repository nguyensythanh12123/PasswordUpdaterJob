using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordUpdaterJob.Dapper
{
    public class DapperRepository : IDapper
    {
        private readonly IDbConnection _dbConnection;

        public DapperRepository(string connectionString)
        {
            _dbConnection = new SqlConnection(connectionString);
        }
        public void Dispose()
        {
            _dbConnection?.Dispose();
        }

        public async Task<int> ExecuteAsync(string sql, DynamicParameters parameters, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            _dbConnection.Open();
            using (var transaction = _dbConnection.BeginTransaction()) // Begin transaction
            {
                try
                {
                    var result = await _dbConnection.ExecuteAsync(sql, parameters, transaction, commandTimeout, commandType);
                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
                finally { _dbConnection.Close(); }
            }

        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters parameters, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            return await _dbConnection.QueryAsync<T>(sql, parameters, transaction, commandTimeout, commandType);
        }

        public async Task<T> QuerySingleAsync<T>(string sql, DynamicParameters parameters, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, parameters, transaction, commandTimeout, commandType);
        }
    }
}
