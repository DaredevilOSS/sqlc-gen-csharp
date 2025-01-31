// auto-generated by sqlc - do not edit
// ReSharper disable UseAwaitUsing
// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace NpgsqlDapperLegacyExampleGen
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dapper;
    using Npgsql;
    using NpgsqlTypes;

    public class QuerySql
    {
        public QuerySql(string connectionString)
        {
            this.ConnectionString = connectionString;
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        private string ConnectionString { get; }

        private const string GetAuthorSql = "SELECT id, name, bio, created FROM authors WHERE name = @name LIMIT 1";
        public class GetAuthorRow
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Bio { get; set; }
            public DateTime Created { get; set; }
        };
        public class GetAuthorArgs
        {
            public string Name { get; set; }
        };
        public async Task<GetAuthorRow> GetAuthor(GetAuthorArgs args)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetAuthorRow>(GetAuthorSql, new { name = args.Name });
                return result;
            }
        }

        private const string ListAuthorsSql = "SELECT id, name, bio, created FROM authors ORDER BY name";
        public class ListAuthorsRow
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Bio { get; set; }
            public DateTime Created { get; set; }
        };
        public async Task<List<ListAuthorsRow>> ListAuthors()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
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
            public string Bio { get; set; }
            public DateTime Created { get; set; }
        };
        public class CreateAuthorArgs
        {
            public string Name { get; set; }
            public string Bio { get; set; }
        };
        public async Task<CreateAuthorRow> CreateAuthor(CreateAuthorArgs args)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<CreateAuthorRow>(CreateAuthorSql, new { name = args.Name, bio = args.Bio });
                return result;
            }
        }

        private const string CreateAuthorReturnIdSql = "INSERT INTO authors (name, bio) VALUES (@name, @bio) RETURNING id";
        public class CreateAuthorReturnIdRow
        {
            public long Id { get; set; }
        };
        public class CreateAuthorReturnIdArgs
        {
            public string Name { get; set; }
            public string Bio { get; set; }
        };
        public async Task<long> CreateAuthorReturnId(CreateAuthorReturnIdArgs args)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                return await connection.QuerySingleAsync<long>(CreateAuthorReturnIdSql, new { name = args.Name, bio = args.Bio });
            }
        }

        private const string GetAuthorByIdSql = "SELECT id, name, bio, created FROM authors WHERE id = @id LIMIT 1";
        public class GetAuthorByIdRow
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Bio { get; set; }
            public DateTime Created { get; set; }
        };
        public class GetAuthorByIdArgs
        {
            public long Id { get; set; }
        };
        public async Task<GetAuthorByIdRow> GetAuthorById(GetAuthorByIdArgs args)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetAuthorByIdRow>(GetAuthorByIdSql, new { id = args.Id });
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
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.ExecuteAsync(DeleteAuthorSql, new { name = args.Name });
            }
        }

        private const string TruncateAuthorsSql = "TRUNCATE TABLE authors";
        public async Task TruncateAuthors()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.ExecuteAsync(TruncateAuthorsSql);
            }
        }

        private const string UpdateAuthorsSql = "UPDATE authors  SET  bio  =  @bio  WHERE  bio  IS  NOT  NULL  ";  
        public class UpdateAuthorsArgs
        {
            public string Bio { get; set; }
        };
        public async Task<long> UpdateAuthors(UpdateAuthorsArgs args)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                return await connection.ExecuteAsync(UpdateAuthorsSql, new { bio = args.Bio });
            }
        }

        private const string SelectAuthorsWithSliceSql = "SELECT id, name, bio, created FROM authors WHERE id = ANY(@longArr_1::BIGINT[])";
        public class SelectAuthorsWithSliceRow
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Bio { get; set; }
            public DateTime Created { get; set; }
        };
        public class SelectAuthorsWithSliceArgs
        {
            public long[] LongArr1 { get; set; }
        };
        public async Task<List<SelectAuthorsWithSliceRow>> SelectAuthorsWithSlice(SelectAuthorsWithSliceArgs args)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var results = await connection.QueryAsync<SelectAuthorsWithSliceRow>(SelectAuthorsWithSliceSql, new { longArr_1 = args.LongArr1 });
                return results.AsList();
            }
        }

        private const string TruncateCopyToTestsSql = "TRUNCATE TABLE copy_tests";
        public async Task TruncateCopyToTests()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.ExecuteAsync(TruncateCopyToTestsSql);
            }
        }

        private const string TruncateNodePostgresTypesSql = "TRUNCATE TABLE node_postgres_types";
        public async Task TruncateNodePostgresTypes()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.ExecuteAsync(TruncateNodePostgresTypesSql);
            }
        }

        private const string CopyToTestsSql = "COPY copy_tests (c_int, c_varchar, c_date, c_timestamp) FROM STDIN (FORMAT BINARY)";
        public class CopyToTestsArgs
        {
            public int CInt { get; set; }
            public string CVarchar { get; set; }
            public DateTime CDate { get; set; }
            public DateTime CTimestamp { get; set; }
        };
        public async Task CopyToTests(List<CopyToTestsArgs> args)
        {
            using (var ds = NpgsqlDataSource.Create(ConnectionString))
            {
                var connection = ds.CreateConnection();
                await connection.OpenAsync();
                using (var writer = await connection.BeginBinaryImportAsync(CopyToTestsSql))
                {
                    foreach (var row in args)
                    {
                        await writer.StartRowAsync();
                        await writer.WriteAsync(row.CInt, NpgsqlDbType.Integer);
                        await writer.WriteAsync(row.CVarchar, NpgsqlDbType.Varchar);
                        await writer.WriteAsync(row.CDate, NpgsqlDbType.Date);
                        await writer.WriteAsync(row.CTimestamp, NpgsqlDbType.Timestamp);
                    }

                    await writer.CompleteAsync();
                }

                await connection.CloseAsync();
            }
        }

        private const string CountCopyRowsSql = "SELECT COUNT(1) AS cnt FROM copy_tests";
        public class CountCopyRowsRow
        {
            public long Cnt { get; set; }
        };
        public async Task<CountCopyRowsRow> CountCopyRows()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<CountCopyRowsRow>(CountCopyRowsSql);
                return result;
            }
        }

        private const string InsertNodePostgresTypeSql = "INSERT INTO node_postgres_types (c_smallint, c_boolean, c_integer, c_bigint, c_serial, c_decimal, c_numeric, c_real, c_date, c_timestamp, c_char, c_varchar, c_character_varying, c_text, c_text_array, c_integer_array) VALUES ( @c_smallint , @c_boolean, @c_integer, @c_bigint, @c_serial, @c_decimal, @c_numeric, @c_real, @c_date, @c_timestamp, @c_char, @c_varchar, @c_character_varying, @c_text, @c_text_array, @c_integer_array ) RETURNING  id  "; 
        public class InsertNodePostgresTypeRow
        {
            public long Id { get; set; }
        };
        public class InsertNodePostgresTypeArgs
        {
            public int? CSmallint { get; set; }
            public bool? CBoolean { get; set; }
            public int? CInteger { get; set; }
            public long? CBigint { get; set; }
            public int? CSerial { get; set; }
            public float? CDecimal { get; set; }
            public float? CNumeric { get; set; }
            public float? CReal { get; set; }
            public DateTime? CDate { get; set; }
            public DateTime? CTimestamp { get; set; }
            public string CChar { get; set; }
            public string CVarchar { get; set; }
            public string CCharacterVarying { get; set; }
            public string CText { get; set; }
            public string[] CTextArray { get; set; }
            public int[] CIntegerArray { get; set; }
        };
        public async Task<long> InsertNodePostgresType(InsertNodePostgresTypeArgs args)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                return await connection.QuerySingleAsync<long>(InsertNodePostgresTypeSql, new { c_smallint = args.CSmallint, c_boolean = args.CBoolean, c_integer = args.CInteger, c_bigint = args.CBigint, c_serial = args.CSerial, c_decimal = args.CDecimal, c_numeric = args.CNumeric, c_real = args.CReal, c_date = args.CDate, c_timestamp = args.CTimestamp, c_char = args.CChar, c_varchar = args.CVarchar, c_character_varying = args.CCharacterVarying, c_text = args.CText, c_text_array = args.CTextArray, c_integer_array = args.CIntegerArray });
            }
        }

        private const string GetNodePostgresTypeSql = "SELECT c_smallint, c_boolean, c_integer, c_bigint, c_serial, c_decimal, c_numeric, c_real, c_date, c_timestamp, c_char, c_varchar, c_character_varying, c_text, c_text_array, c_integer_array FROM node_postgres_types WHERE id = @id LIMIT 1";
        public class GetNodePostgresTypeRow
        {
            public int? CSmallint { get; set; }
            public bool? CBoolean { get; set; }
            public int? CInteger { get; set; }
            public long? CBigint { get; set; }
            public int? CSerial { get; set; }
            public float? CDecimal { get; set; }
            public float? CNumeric { get; set; }
            public float? CReal { get; set; }
            public DateTime? CDate { get; set; }
            public DateTime? CTimestamp { get; set; }
            public string CChar { get; set; }
            public string CVarchar { get; set; }
            public string CCharacterVarying { get; set; }
            public string CText { get; set; }
            public string[] CTextArray { get; set; }
            public int[] CIntegerArray { get; set; }
        };
        public class GetNodePostgresTypeArgs
        {
            public long Id { get; set; }
        };
        public async Task<GetNodePostgresTypeRow> GetNodePostgresType(GetNodePostgresTypeArgs args)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                var result = await connection.QueryFirstOrDefaultAsync<GetNodePostgresTypeRow>(GetNodePostgresTypeSql, new { id = args.Id });
                return result;
            }
        }
    }
}