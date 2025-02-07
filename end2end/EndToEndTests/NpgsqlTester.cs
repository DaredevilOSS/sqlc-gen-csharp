using NpgsqlExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

public partial class NpgsqlTester
{
    private static readonly Random Randomizer = new();

    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTables()
    {
        await QuerySql.TruncateAuthors();
        await QuerySql.TruncateCopyToTests();
        await QuerySql.TruncatePostgresTypes();

        new DateTime(2020, 11, 20, 10, 00, 00, 11);
    }
}