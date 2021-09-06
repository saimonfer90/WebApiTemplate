using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using WebApiTemplate.Data;
using Xunit;

namespace WebApiTemplate.Test.Postgres
{
    public abstract class DbHandlerTest
    {
        protected string _engineName;

        protected readonly IConfigurationSection _connectionStrings;
        protected readonly IConfigurationSection _queries;
        protected readonly IConfigurationSection _envirnoment;

        protected DbHandler<NpgsqlConnection> _pgHandler;

        protected DbHandlerTest()
        {
            IConfiguration builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            _connectionStrings = builder
                .GetSection("Configuration:ConnectionStrings");

            _queries = builder.GetSection("Queries");
            _envirnoment = builder.GetSection("Configuration:Env");
        }

        protected async Task InitializeEnvironment()
        {
            var autoInit = Convert.ToBoolean(
                    _envirnoment.GetSection("Auto-init")?.Value
                    ?? throw new Exception("appsetting.json must contains a select count query in the section configuration:connectionStrings"));

            var tab = _envirnoment.GetSection("Table");

            if (!autoInit)
                return;

            try
            {
                var nonQuery = $"DROP TABLE {tab.Value}";

                await _pgHandler.ExecuteNonQueryAsync(nonQuery);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                /*if it does not exist, it goes further*/
                /*because "if exists" isn't in standard sql*/
            }

            try
            {
                var nonQuery = $"CREATE TABLE {tab.Value} " +
                        "(FIELD_1 varchar(30) PRIMARY KEY, " +
                        "FIELD_2 varchar(30), " +
                        "FIELD_3 varchar(30), " +
                        "FIELD_4 varchar(30), " +
                        "FIELD_5 varchar(30)) ";

                await _pgHandler.ExecuteNonQueryAsync(nonQuery);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Cannot create test table {tab.Value}");
                Debug.WriteLine(ex.Message);

                throw;
            }
        }

        protected async Task SingleInsertAsync()
        {
            int? result = -1;

            var query = _queries.GetSection("InsertQuery");

            Assert.True(query != null, "appsetting.json must contains a insert query in the section queries:insertQuery");

            try
            {
                result = await _pgHandler.ExecuteNonQueryAsync(query.Value,
                    new
                    {
                        p0 = "0",
                        p1 = "1",
                        p2 = "2",
                        p3 = "3",
                        p4 = "4"
                    })
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Assert.True(false, ex.Message);
            }

            Assert.True(result >= 0);
        }

        protected async Task<long> CountAsync()
        {
            long result = -1;

            var query = _queries.GetSection("CountQuery");

            Assert.True(query != null, "appsetting.json must contains a select count query in the section queries:countQuery");

            try
            {
                result = Convert.ToInt64((await _pgHandler
                   .ExecuteQueryAsync<dynamic>(query.Value))
                   ?.FirstOrDefault()
                   ?.FirstOrDefault());
            }
            catch (Exception ex)
            {
                Assert.True(false, ex.Message);

                return result;
            }

            Debug.WriteLine($"Count: {result}");

            Assert.True((int)result >= 0);

            return result;
        }

        protected async Task DeleteAsync()
        {
            var query = _queries.GetSection("DelAllQuery");

            Assert.True(query != null, "appsetting.json must contains a delete query in the section queries:delAllQuery");

            try
            {
                await _pgHandler.ExecuteNonQueryAsync(query.Value)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Assert.True(false, ex.Message);
            }

            Assert.True(true);
        }
    }
}