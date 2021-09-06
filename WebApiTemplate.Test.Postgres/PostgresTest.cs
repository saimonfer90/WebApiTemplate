using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using WebApiTemplate.Data;
using WebApiTemplate.Test.Postgres;
using Xunit;

namespace BulkSqlLoader.XUnitTest
{
    public class PostgreSqlTest : DbHandlerTest
    {
        public PostgreSqlTest()
        {
            var connectionString = _connectionStrings.GetSection("Postgres").Value;

            IDbConnection conn = new NpgsqlConnection(connectionString);

            _pgHandler = new DbHandler<NpgsqlConnection>(null, connectionString);

            try
            {
                _ = base.InitializeEnvironment();
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Cannot intitialize test for {_engineName}: {ex.Message}");
            }
        }

        [Fact]
        internal async Task PostgresSingleInsertAsync()
            => await base.SingleInsertAsync();

        [Fact]
        internal async Task PostgresCountAsync()
            => await base.CountAsync();

        [Fact]
        internal async Task PostgresDeleteAsync()
            => await base.DeleteAsync();
    }
}