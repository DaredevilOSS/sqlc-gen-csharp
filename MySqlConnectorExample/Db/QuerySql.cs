using System;
using System.Threading.Tasks;
using MySqlConnector;

namespace GeneratedNamespace
{
    public static class QuerySql
    {
        private const string ConnectionString = "server=localhost;user=root;database=mydb;port=3306;password=";
        private const string GetAuthorSql = "SELECT id, name, bio FROM authors\nWHERE id = ? LIMIT 1";
        public readonly record struct GetAuthorRow(long Id, string Name, string Bio);
        public readonly record struct GetAuthorArgs(long Id);
        public static async Task<GetAuthorRow?> GetAuthor(GetAuthorArgs args)
        {
            await using var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            await using var command = new MySqlCommand(GetAuthorSql, connection);
            command.Parameters.AddWithValue("@id", args.Id);
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new GetAuthorRow
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Bio = reader.GetString(2)
                };
            }

            return null;
        }

        private const string ListAuthorsSql = "SELECT id, name, bio FROM authors\nORDER BY name";
        public readonly record struct ListAuthorsRow(long Id, string Name, string Bio);
        public async Task<List<ListAuthorsRow>> ListAuthors()
        {
            return Task.FromResult<string>("Placeholder for actual implementation");
        }

        private const string CreateAuthorSql = "INSERT INTO authors (\n  name, bio\n) VALUES (\n  ?, ? \n)";
        public readonly record struct CreateAuthorArgs(string Name, string Bio);
        public async Task<void> CreateAuthor(CreateAuthorArgs args)
        {
            await client.QueryAsync(queryParameters);
        }

        private const string DeleteAuthorSql = "DELETE FROM authors\nWHERE id = ?";
        public readonly record struct DeleteAuthorArgs(long Id);
        public async Task<void> DeleteAuthor(DeleteAuthorArgs args)
        {
            await client.QueryAsync(queryParameters);
        }

        private const string TestSql = "SELECT c_bit, c_tinyint, c_bool, c_boolean, c_smallint, c_mediumint, c_int, c_integer, c_bigint, c_serial, c_decimal, c_dec, c_numeric, c_fixed, c_float, c_double, c_double_precision, c_date, c_time, c_datetime, c_timestamp, c_year, c_char, c_nchar, c_national_char, c_varchar, c_binary, c_varbinary, c_tinyblob, c_tinytext, c_blob, c_text, c_mediumblob, c_mediumtext, c_longblob, c_longtext, c_json FROM node_mysql_types\nLIMIT 1";
        public readonly record struct TestRow(byte[]? C_bit, int? C_tinyint, int? C_bool, int? C_boolean, int? C_smallint, int? C_mediumint, int? C_int, int? C_integer, long? C_bigint, long C_serial, string C_decimal, string C_dec, string C_numeric, string C_fixed, double? C_float, double? C_double, double? C_double_precision, string C_date, string C_time, string C_datetime, string C_timestamp, int? C_year, string C_char, string C_nchar, string C_national_char, string C_varchar, byte[]? C_binary, byte[]? C_varbinary, byte[]? C_tinyblob, string C_tinytext, byte[]? C_blob, string C_text, byte[]? C_mediumblob, string C_mediumtext, byte[]? C_longblob, string C_longtext, object? C_json);
        public static async Task<TestRow?> Test()
        {
            await using var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            await using var command = new MySqlCommand(TestSql, connection);
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TestRow
                {
                    C_bit = reader.GetBytes(0),
                    C_tinyint = reader.GetInt32(1),
                    C_bool = reader.GetInt32(2),
                    C_boolean = reader.GetInt32(3),
                    C_smallint = reader.GetInt32(4),
                    C_mediumint = reader.GetInt32(5),
                    C_int = reader.GetInt32(6),
                    C_integer = reader.GetInt32(7),
                    C_bigint = reader.GetInt64(8),
                    C_serial = reader.GetInt64(9),
                    C_decimal = reader.GetString(10),
                    C_dec = reader.GetString(11),
                    C_numeric = reader.GetString(12),
                    C_fixed = reader.GetString(13),
                    C_float = reader.GetDouble(14),
                    C_double = reader.GetDouble(15),
                    C_double_precision = reader.GetDouble(16),
                    C_date = reader.GetString(17),
                    C_time = reader.GetString(18),
                    C_datetime = reader.GetString(19),
                    C_timestamp = reader.GetString(20),
                    C_year = reader.GetInt32(21),
                    C_char = reader.GetString(22),
                    C_nchar = reader.GetString(23),
                    C_national_char = reader.GetString(24),
                    C_varchar = reader.GetString(25),
                    C_binary = reader.GetBytes(26),
                    C_varbinary = reader.GetBytes(27),
                    C_tinyblob = reader.GetBytes(28),
                    C_tinytext = reader.GetString(29),
                    C_blob = reader.GetBytes(30),
                    C_text = reader.GetString(31),
                    C_mediumblob = reader.GetBytes(32),
                    C_mediumtext = reader.GetString(33),
                    C_longblob = reader.GetBytes(34),
                    C_longtext = reader.GetString(35),
                    C_json = reader.GetString(36)
                };
            }

            return null;
        }
    }
}