using Microsoft.Data.Sqlite;
using NUnit.Framework;
using SqliteAsyncEnumerableExampleGen;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EndToEndTests;

[TestFixture]
public class SqliteAsyncEnumerableTester
{
    private QuerySql QuerySql { get; } = new(
        Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv)!);

    [TearDown]
    public async Task EmptyTestsTable()
    {
        await QuerySql.DeleteAllAuthorsAsync();
    }

    private async Task SeedAuthors(QuerySql querySql)
    {
        await querySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs(1, "Bravo", "b"));
        await querySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs(2, "Alpha", null));
        await querySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs(3, "Charlie", "c"));
    }

    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
    {
        var result = new List<T>();
        await foreach (var item in source)
            result.Add(item);
        return result;
    }

    [Test]
    public async Task TestListAuthorsStreamsRows()
    {
        await SeedAuthors(QuerySql);

        var actual = await ToListAsync(QuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs(0, 10)));

        Assert.That(actual.ConvertAll(a => a.Name), Is.EqualTo(new[] { "Alpha", "Bravo", "Charlie" }));
        Assert.That(actual[0].Bio, Is.Null);
        Assert.That(actual[1].Bio, Is.EqualTo("b"));
    }

    [Test]
    public async Task TestListAuthorsEmpty()
    {
        var actual = await ToListAsync(QuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs(0, 10)));
        Assert.That(actual, Is.Empty);
    }

    [Test]
    public async Task TestListAuthorsCanStopEarly()
    {
        await SeedAuthors(QuerySql);

        await foreach (var author in QuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs(0, 10)))
        {
            Assert.That(author.Name, Is.EqualTo("Alpha"));
            break;
        }
    }

    [Test]
    public async Task TestListAuthorsInTransaction()
    {
        using var connection = new SqliteConnection(
            Environment.GetEnvironmentVariable(EndToEndCommon.SqliteConnectionStringEnv)!);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        var transactionalQuerySql = QuerySql.WithTransaction(transaction);
        await SeedAuthors(transactionalQuerySql);

        var actual = await ToListAsync(transactionalQuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs(0, 10)));

        Assert.That(actual, Has.Count.EqualTo(3));
        transaction.Rollback();
        Assert.That(await ToListAsync(QuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs(0, 10))), Is.Empty);
    }

    [Test]
    public async Task TestListAuthorsHonoursCancellation()
    {
        await SeedAuthors(QuerySql);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.CatchAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in QuerySql.ListAuthorsAsync(new QuerySql.ListAuthorsArgs(0, 10), cts.Token)) { }
        });
    }
}