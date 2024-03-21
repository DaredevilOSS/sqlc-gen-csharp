using MySql.Data.MySqlClient;

namespace GeneratedNamespace
{
    public static class query.sql
    {
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

        public record GetAuthorRow(long id, string name, string bio);
        public record ListAuthorsRow(long id, string name, string bio);
        public record CreateAuthorRow();
        public record DeleteAuthorRow();
        public record TestRow(byte[]? c_bit, int? c_tinyint, int? c_bool, int? c_boolean, int? c_smallint, int? c_mediumint, int? c_int, int? c_integer, long? c_bigint, long c_serial, string c_decimal, string c_dec, string c_numeric, string c_fixed, double? c_float, double? c_double, double? c_double_precision, string c_date, string c_time, string c_datetime, string c_timestamp, int? c_year, string c_char, string c_nchar, string c_national_char, string c_varchar, byte[]? c_binary, byte[]? c_varbinary, byte[]? c_tinyblob, string c_tinytext, byte[]? c_blob, string c_text, byte[]? c_mediumblob, string c_mediumtext, byte[]? c_longblob, string c_longtext, object? c_json);
    }
}