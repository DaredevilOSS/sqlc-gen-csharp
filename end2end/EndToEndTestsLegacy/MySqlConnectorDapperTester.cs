using MySqlConnectorDapperLegacyExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EndToEndTests
{
    public partial class MySqlConnectorDapperTester
    {
        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.DeleteAllAuthors();
            await QuerySql.TruncateExtendedBios();
            await QuerySql.TruncateMysqlNumericTypes();
            await QuerySql.TruncateMysqlStringTypes();
            await QuerySql.TruncateMysqlDatetimeTypes();
            await QuerySql.TruncateMysqlBinaryTypes();
        }
    }
}