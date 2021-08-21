using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Serilog;

namespace WebApiTemplate.Data
{
    public class DbHandler<ConnectionInstance> where ConnectionInstance : IDbConnection, new()
    {
        /// <summary>
        /// ILogger instance for logging
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">System logger, if null is not used by class</param>
        /// <param name="dbConnection">connection</param>
        public DbHandler(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Run generic query, async version
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// /// <returns>Results list</returns>
        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query, bool throwException = true)
        {
            List<T> result = new();

            try
            {
                using var connection = new ConnectionInstance();

                result.AddRange(await connection.QueryAsync<T>(query)
                    .ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                _logger?.Error($"Not able to execute the query: {query}");

                _logger?.Error(ex.Message);
                _logger?.Error(ex.StackTrace);

                if (throwException)
                    throw;
            }

            return result;
        }

        /// <summary>
        /// Performs a single parameterized non query, async version
        /// </summary>
        /// <param name="nonQuery">Non query to execute, example: "INSERT INTO TABLE_NAME (ID, DESC) VALUES (@p0, @p1)"</param>
        /// <param name="parameters">Parameters to replace, example: [ 1, "hello" ]</param>
        /// <param name="isolationLevel">Set the transaction lock</param>
        /// <returns>Number of affected rows</returns>
        public async Task<int> ExecuteNonQueryAsync(string nonQuery, bool throwException = true, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            int affected = 0;
            IDbTransaction transaction = null;
            try
            {
                using var connection = new ConnectionInstance();

                transaction = connection.BeginTransaction(isolationLevel);

                affected = await connection.ExecuteAsync(nonQuery)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.Error($"Not able to execute the nonQuery: {nonQuery}");

                _logger?.Error(ex.Message);
                _logger?.Error(ex.StackTrace);

                try
                {
                    _logger?.Information("Starting to perform the rollback..");
                    transaction?.Rollback();
                }
                catch (Exception ex1)
                {
                    _logger?.Error("Rollback is failed");

                    _logger?.Error(ex1.Message);
                    _logger?.Error(ex1.StackTrace);
                }

                if (throwException)
                    throw;
            }
            finally
            {
                transaction?.Dispose();
            }

            return affected;
        }
    }
}