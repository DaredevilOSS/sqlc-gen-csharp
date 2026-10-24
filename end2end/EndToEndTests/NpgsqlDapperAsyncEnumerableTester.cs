using Npgsql;
using NpgsqlDapperAsyncEnumerableExampleGen;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EndToEndTests;

[TestFixture]
public class NpgsqlDapperAsyncEnumerableTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.TruncateAuthorsAsync();
    }

    private static async Task SeedAuthors(QuerySql querySql)
    {
        await querySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 1, Name = "Bravo", Bio = "b" });
        await querySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 2, Name = "Alpha", Bio = null });
        await querySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs { Id = 3, Name = "Charlie", Bio = "c" });
    }

    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
    {
        var result = new List<T>();
        await foreach (var item in source)
            result.Add(item);
        return result;
    }

    private static QuerySql.ListAuthorsArgs AllAuthors => new() { Offset = 0, Limit = 10 };

    [Test]
    public async Task TestListAuthorsStreamsRows()
    {
        await SeedAuthors(QuerySql);

        var actual = await ToListAsync(QuerySql.ListAuthorsAsync(AllAuthors));

        Assert.That(actual.ConvertAll(a => a.Name), Is.EqualTo(new[] { "Alpha", "Bravo", "Charlie" }));
        Assert.That(actual[0].Bio, Is.Null);
        Assert.That(actual[1].Bio, Is.EqualTo("b"));
    }

    [Test]
    public async Task TestListAuthorsEmpty()
    {
        Assert.That(await ToListAsync(QuerySql.ListAuthorsAsync(AllAuthors)), Is.Empty);
    }

    [Test]
    public async Task TestListAuthorsInTransaction()
    {
        using var connection = new NpgsqlConnection(
            Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv)!);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();
        var transactionalQuerySql = QuerySql.WithTransaction(transaction);
        await SeedAuthors(transactionalQuerySql);

        var actual = await ToListAsync(transactionalQuerySql.ListAuthorsAsync(AllAuthors));

        Assert.That(actual, Has.Count.EqualTo(3));
        await transaction.RollbackAsync();
        Assert.That(await ToListAsync(QuerySql.ListAuthorsAsync(AllAuthors)), Is.Empty);
    }

    [Test]
    public async Task TestListAuthorsHonoursCancellation()
    {
        await SeedAuthors(QuerySql);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.CatchAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in QuerySql.ListAuthorsAsync(AllAuthors, cts.Token)) { }
        });
    }
}