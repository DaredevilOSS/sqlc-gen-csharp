// auto-generated by sqlc - do not edit
// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace NpgsqlExampleGen;
public class QuerySql(string connectionString)
{
    private const string GetAuthorSql = "SELECT id, name, bio, created FROM authors WHERE name = @name LIMIT 1";
    public readonly record struct GetAuthorRow(long Id, string Name, string? Bio, DateTime Created);
    public readonly record struct GetAuthorArgs(string Name);
    public async Task<GetAuthorRow?> GetAuthor(GetAuthorArgs args)
    {
        {
            await using var connection = NpgsqlDataSource.Create(connectionString);
            await using var command = connection.CreateCommand(GetAuthorSql);
            command.Parameters.AddWithValue("@name", args.Name);
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new GetAuthorRow
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Bio = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Created = reader.GetDateTime(3)
                };
            }

            return null;
        }
    }

    private const string ListAuthorsSql = "SELECT id, name, bio, created FROM authors ORDER BY name";
    public readonly record struct ListAuthorsRow(long Id, string Name, string? Bio, DateTime Created);
    public async Task<List<ListAuthorsRow>> ListAuthors()
    {
        {
            await using var connection = NpgsqlDataSource.Create(connectionString);
            await using var command = connection.CreateCommand(ListAuthorsSql);
            var reader = await command.ExecuteReaderAsync();
            var result = new List<ListAuthorsRow>();
            while (await reader.ReadAsync())
            {
                result.Add(new ListAuthorsRow { Id = reader.GetInt64(0), Name = reader.GetString(1), Bio = reader.IsDBNull(2) ? null : reader.GetString(2), Created = reader.GetDateTime(3) });
            }

            return result;
        }
    }

    private const string CreateAuthorSql = "INSERT INTO authors (name, bio) VALUES (@name, @bio) RETURNING id, name, bio, created";
    public readonly record struct CreateAuthorRow(long Id, string Name, string? Bio, DateTime Created);
    public readonly record struct CreateAuthorArgs(string Name, string? Bio);
    public async Task<CreateAuthorRow?> CreateAuthor(CreateAuthorArgs args)
    {
        {
            await using var connection = NpgsqlDataSource.Create(connectionString);
            await using var command = connection.CreateCommand(CreateAuthorSql);
            command.Parameters.AddWithValue("@name", args.Name);
            command.Parameters.AddWithValue("@bio", args.Bio!);
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CreateAuthorRow
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Bio = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Created = reader.GetDateTime(3)
                };
            }

            return null;
        }
    }

    private const string DeleteAuthorSql = "DELETE FROM authors WHERE name = @name";
    public readonly record struct DeleteAuthorArgs(string Name);
    public async Task DeleteAuthor(DeleteAuthorArgs args)
    {
        {
            await using var connection = NpgsqlDataSource.Create(connectionString);
            await using var command = connection.CreateCommand(DeleteAuthorSql);
            command.Parameters.AddWithValue("@name", args.Name);
            await command.ExecuteScalarAsync();
        }
    }

    private const string TruncateAuthorsSql = "TRUNCATE TABLE authors";
    public async Task TruncateAuthors()
    {
        {
            await using var connection = NpgsqlDataSource.Create(connectionString);
            await using var command = connection.CreateCommand(TruncateAuthorsSql);
            await command.ExecuteScalarAsync();
        }
    }

    private const string UpdateAuthorsSql = "UPDATE authors  SET  bio  =  @bio  WHERE  bio  IS  NOT  NULL  ";  
    public readonly record struct UpdateAuthorsArgs(string? Bio);
    public async Task<long> UpdateAuthors(UpdateAuthorsArgs args)
    {
        {
            await using var connection = NpgsqlDataSource.Create(connectionString);
            await using var command = connection.CreateCommand(UpdateAuthorsSql);
            command.Parameters.AddWithValue("@bio", args.Bio!);
            return await command.ExecuteNonQueryAsync();
        }
    }

    private const string CopyToTestsSql = "COPY copy_tests (c_int, c_varchar, c_date, c_timestamp) FROM STDIN (FORMAT BINARY)";
    public readonly record struct CopyToTestsArgs(int C_int, string C_varchar, DateTime C_date, DateTime C_timestamp);
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
    public readonly record struct CountCopyRowsRow(long Cnt);
    public async Task<CountCopyRowsRow?> CountCopyRows()
    {
        {
            await using var connection = NpgsqlDataSource.Create(connectionString);
            await using var command = connection.CreateCommand(CountCopyRowsSql);
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CountCopyRowsRow
                {
                    Cnt = reader.GetInt64(0)
                };
            }

            return null;
        }
    }

    private const string TestSql = "SELECT c_bit, c_smallint, c_boolean, c_integer, c_bigint, c_serial, c_decimal, c_numeric, c_real, c_double_precision, c_date, c_time, c_timestamp, c_char, c_varchar, c_character_varying, c_bytea, c_text, c_json FROM node_postgres_types LIMIT 1";
    public readonly record struct TestRow(byte[]? C_bit, int? C_smallint, bool? C_boolean, int? C_integer, long? C_bigint, int? C_serial, float? C_decimal, float? C_numeric, float? C_real, float? C_double_precision, DateTime? C_date, string? C_time, DateTime? C_timestamp, string? C_char, string? C_varchar, string? C_character_varying, byte[]? C_bytea, string? C_text, object? C_json);
    public async Task<TestRow?> Test()
    {
        {
            await using var connection = NpgsqlDataSource.Create(connectionString);
            await using var command = connection.CreateCommand(TestSql);
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TestRow
                {
                    C_bit = reader.IsDBNull(0) ? null : Utils.GetBytes(reader, 0),
                    C_smallint = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    C_boolean = reader.IsDBNull(2) ? null : reader.GetBoolean(2),
                    C_integer = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    C_bigint = reader.IsDBNull(4) ? null : reader.GetInt64(4),
                    C_serial = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    C_decimal = reader.IsDBNull(6) ? null : reader.GetFloat(6),
                    C_numeric = reader.IsDBNull(7) ? null : reader.GetFloat(7),
                    C_real = reader.IsDBNull(8) ? null : reader.GetFloat(8),
                    C_double_precision = reader.IsDBNull(9) ? null : reader.GetFloat(9),
                    C_date = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                    C_time = reader.IsDBNull(11) ? null : reader.GetString(11),
                    C_timestamp = reader.IsDBNull(12) ? null : reader.GetDateTime(12),
                    C_char = reader.IsDBNull(13) ? null : reader.GetString(13),
                    C_varchar = reader.IsDBNull(14) ? null : reader.GetString(14),
                    C_character_varying = reader.IsDBNull(15) ? null : reader.GetString(15),
                    C_bytea = reader.IsDBNull(16) ? null : Utils.GetBytes(reader, 16),
                    C_text = reader.IsDBNull(17) ? null : reader.GetString(17),
                    C_json = reader.IsDBNull(18) ? null : reader.GetString(18)
                };
            }

            return null;
        }
    }
}