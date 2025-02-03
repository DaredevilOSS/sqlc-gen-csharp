using MySqlConnectorExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests;

[TestFixture]
public partial class MySqlConnectorTester
{
    private static readonly Random Randomizer = new();

    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.MySqlConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.TruncateAuthors();
        await QuerySql.TruncateCopyToTests();
    }

    [Test]
    public async Task TestCopyFrom()
    {
        const int batchSize = 100;
        var createAuthorBatchArgs = Enumerable.Range(0, batchSize)
            .Select(_ => GenerateRandom())
            .ToList();
        await QuerySql.CopyToTests(createAuthorBatchArgs);
        var countRows = QuerySql.CountCopyRows().Result!.Value.Cnt;
        ClassicAssert.AreEqual(batchSize, countRows);
        return;

        QuerySql.CopyToTestsArgs GenerateRandom()
        {
            return new QuerySql.CopyToTestsArgs
            {
                CVarchar = Randomizer.Next().ToString(),
                CInt = Randomizer.Next(),
                CDate = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(Randomizer.Next())),
                CTimestamp = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(Randomizer.Next()))
            };
        }
    }

    [Test]
    public async Task TestSliceIds()
    {
        var insertedId1 = await QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
        });
        var insertedId2 = await QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
        });
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
        });

        var actual = await QuerySql.SelectAuthorsWithSlice(new QuerySql.SelectAuthorsWithSliceArgs { Ids = [insertedId1, insertedId2] });

        ClassicAssert.AreEqual(2, actual.Count);
    }

    [Test]
    public async Task TestTwoSlices()
    {
        var insertedId1 = await QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
        });
        var insertedId2 = await QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs
        {
            Name = DataGenerator.BojackAuthor,
            Bio = DataGenerator.GenericQuote1
        });
        await QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs
        {
            Name = DataGenerator.GenericAuthor,
            Bio = DataGenerator.GenericQuote1
        });

        var actual = await QuerySql.SelectAuthorsWithTwoSlices(new QuerySql.SelectAuthorsWithTwoSlicesArgs { Ids = [insertedId1, insertedId2], Names = [DataGenerator.GenericAuthor] });

        ClassicAssert.AreEqual(1, actual.Count);
    }
}