using MySql.Data.MySqlClient;

namespace GeneratedNamespace
{
    public static class QuerySql
    {
        private const string GetAuthorSql = "SELECT id, name, bio FROM authors\nWHERE id = ? LIMIT 1";
        private const string ListAuthorsSql = "SELECT id, name, bio FROM authors\nORDER BY name";
        private const string CreateAuthorSql = "INSERT INTO authors (\n  name, bio\n) VALUES (\n  ?, ? \n)";
        private const string DeleteAuthorSql = "DELETE FROM authors\nWHERE id = ?";
        private const string TestSql = "SELECT c_bit, c_tinyint, c_bool, c_boolean, c_smallint, c_mediumint, c_int, c_integer, c_bigint, c_serial, c_decimal, c_dec, c_numeric, c_fixed, c_float, c_double, c_double_precision, c_date, c_time, c_datetime, c_timestamp, c_year, c_char, c_nchar, c_national_char, c_varchar, c_binary, c_varbinary, c_tinyblob, c_tinytext, c_blob, c_text, c_mediumblob, c_mediumtext, c_longblob, c_longtext, c_json FROM node_mysql_types\nLIMIT 1";
        public async Task GetAuthor()
        {
            var connection = new MySqlConnection();
        }

        public async Task ListAuthors()
        {
            var connection = new MySqlConnection();
        }

        public async Task CreateAuthor()
        {
            var connection = new MySqlConnection();
        }

        public async Task DeleteAuthor()
        {
            var connection = new MySqlConnection();
        }

        public async Task Test()
        {
            var connection = new MySqlConnection();
        }
    }

    public record GetAuthorRow(long id, string name, string bio);
    public record ListAuthorsRow(long id, string name, string bio);
    public record TestRow(byte[]? c_bit, int? c_tinyint, int? c_bool, int? c_boolean, int? c_smallint, int? c_mediumint, int? c_int, int? c_integer, long? c_bigint, long c_serial, string c_decimal, string c_dec, string c_numeric, string c_fixed, double? c_float, double? c_double, double? c_double_precision, string c_date, string c_time, string c_datetime, string c_timestamp, int? c_year, string c_char, string c_nchar, string c_national_char, string c_varchar, byte[]? c_binary, byte[]? c_varbinary, byte[]? c_tinyblob, string c_tinytext, byte[]? c_blob, string c_text, byte[]? c_mediumblob, string c_mediumtext, byte[]? c_longblob, string c_longtext, object? c_json);
}