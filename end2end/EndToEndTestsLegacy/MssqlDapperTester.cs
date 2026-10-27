using MssqlDapperLegacyExampleGen;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EndToEndTests
{
    public partial class MssqlDapperTester
    {
        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.MssqlConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTables()
        {
            await QuerySql.DeleteAllAuthorsAsync();
            await QuerySql.TruncateNotesAsync();
            await QuerySql.TruncateExtendedBiosAsync();
            await QuerySql.TruncateMssqlNumericTypesAsync();
            await QuerySql.TruncateMssqlStringTypesAsync();
            await QuerySql.TruncateMssqlDatetimeTypesAsync();
            await QuerySql.TruncateMssqlBinaryTypesAsync();
            await QuerySql.TruncateMssqlSpatialTypesAsync();
            await QuerySql.TruncateMssqlNodaTypesAsync();
        }
    }
}