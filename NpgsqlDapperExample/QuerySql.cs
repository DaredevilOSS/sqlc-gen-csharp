// auto-generated by sqlc - do not edit
// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace NpgsqlDapperExampleGen;
public class QuerySql(string connectionString)
{
    private const string GetAuthorSql = "SELECT id, name, bio, created FROM authors WHERE name = @name LIMIT 1";
    public class GetAuthorRow
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Bio { get; set; }
        public DateTime Created { get; set; }
    };
    public class GetAuthorArgs
    {
        public string Name { get; set; }
    };
    public async Task<GetAuthorRow?> GetAuthor(GetAuthorArgs args)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var result = await connection.QueryFirstOrDefaultAsync<GetAuthorRow?>(GetAuthorSql, new { name = args.Name });
            return result;
        }
    }

    private const string ListAuthorsSql = "SELECT id, name, bio, created FROM authors ORDER BY name";
    public class ListAuthorsRow
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Bio { get; set; }
        public DateTime Created { get; set; }
    };
    public async Task<List<ListAuthorsRow>> ListAuthors()
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var results = await connection.QueryAsync<ListAuthorsRow>(ListAuthorsSql);
            return results.AsList();
        }
    }

    private const string CreateAuthorSql = "INSERT INTO authors (name, bio) VALUES (@name, @bio) RETURNING id, name, bio, created";
    public class CreateAuthorRow
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Bio { get; set; }
        public DateTime Created { get; set; }
    };
    public class CreateAuthorArgs
    {
        public string Name { get; set; }
        public string? Bio { get; set; }
    };
    public async Task<CreateAuthorRow?> CreateAuthor(CreateAuthorArgs args)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var result = await connection.QueryFirstOrDefaultAsync<CreateAuthorRow?>(CreateAuthorSql, new { name = args.Name, bio = args.Bio });
            return result;
        }
    }

    private const string DeleteAuthorSql = "DELETE FROM authors WHERE name = @name";
    public class DeleteAuthorArgs
    {
        public string Name { get; set; }
    };
    public async Task DeleteAuthor(DeleteAuthorArgs args)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.ExecuteAsync(DeleteAuthorSql, new { name = args.Name });
        }
    }

    private const string TruncateAuthorsSql = "TRUNCATE TABLE authors";
    public async Task TruncateAuthors()
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.ExecuteAsync(TruncateAuthorsSql);
        }
    }

    private const string UpdateAuthorsSql = "UPDATE authors  SET  bio  =  @bio  WHERE  bio  IS  NOT  NULL  ";  
    public class UpdateAuthorsArgs
    {
        public string? Bio { get; set; }
    };
    public async Task<long> UpdateAuthors(UpdateAuthorsArgs args)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            return await connection.ExecuteAsync(UpdateAuthorsSql, new { bio = args.Bio });
        }
    }

    private const string CopyToTestsSql = "COPY copy_tests (c_int, c_varchar, c_date, c_timestamp) FROM STDIN (FORMAT BINARY)";
    public class CopyToTestsArgs
    {
        public int C_int { get; set; }
        public string C_varchar { get; set; }
        public DateTime C_date { get; set; }
        public DateTime C_timestamp { get; set; }
    };
    public async Task CopyToTests(List<CopyToTestsArgs> args)
    {
        {
            await using var ds = NpgsqlDataSource.Create(connectionString);
            var connection = ds.CreateConnection();
            await connection.OpenAsync();
            await using var writer = await connection.BeginBinaryImportAsync(CopyToTestsSql);
            foreach (var row in args)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(row.C_int, NpgsqlDbType.Integer);
                await writer.WriteAsync(row.C_varchar, NpgsqlDbType.Varchar);
                await writer.WriteAsync(row.C_date, NpgsqlDbType.Date);
                await writer.WriteAsync(row.C_timestamp, NpgsqlDbType.Timestamp);
            }

            await writer.CompleteAsync();
            await connection.CloseAsync();
        }
    }

    private const string CountCopyRowsSql = "SELECT COUNT(1) AS cnt FROM copy_tests";
    public class CountCopyRowsRow
    {
        public long Cnt { get; set; }
    };
    public async Task<CountCopyRowsRow?> CountCopyRows()
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var result = await connection.QueryFirstOrDefaultAsync<CountCopyRowsRow?>(CountCopyRowsSql);
            return result;
        }
    }

    private const string TestSql = "SELECT c_bit, c_smallint, c_boolean, c_integer, c_bigint, c_serial, c_decimal, c_numeric, c_real, c_double_precision, c_date, c_time, c_timestamp, c_char, c_varchar, c_character_varying, c_bytea, c_text, c_json FROM node_postgres_types LIMIT 1";
    public class TestRow
    {
        public byte[]? C_bit { get; set; }
        public int? C_smallint { get; set; }
        public bool? C_boolean { get; set; }
        public int? C_integer { get; set; }
        public long? C_bigint { get; set; }
        public int? C_serial { get; set; }
        public float? C_decimal { get; set; }
        public float? C_numeric { get; set; }
        public float? C_real { get; set; }
        public float? C_double_precision { get; set; }
        public DateTime? C_date { get; set; }
        public string? C_time { get; set; }
        public DateTime? C_timestamp { get; set; }
        public string? C_char { get; set; }
        public string? C_varchar { get; set; }
        public string? C_character_varying { get; set; }
        public byte[]? C_bytea { get; set; }
        public string? C_text { get; set; }
        public object? C_json { get; set; }
    };
    public async Task<TestRow?> Test()
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            var result = await connection.QueryFirstOrDefaultAsync<TestRow?>(TestSql);
            return result;
        }
    }
}