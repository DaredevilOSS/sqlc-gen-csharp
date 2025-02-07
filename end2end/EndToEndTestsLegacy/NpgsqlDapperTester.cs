using NpgsqlDapperLegacyExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    public partial class NpgsqlDapperTester
    {
        private QuerySql QuerySql { get; } = new QuerySql(
            Environment.GetEnvironmentVariable(EndToEndCommon.PostgresConnectionStringEnv));

        [TearDown]
        public async Task EmptyTestsTable()
        {
            await QuerySql.TruncateAuthors();
            await QuerySql.TruncateNodePostgresTypes();
        }

        [Test]
        public async Task TestNodePostgresType()
        {
            var nodePostgresTypeArgs = new QuerySql.InsertNodePostgresTypeArgs
            {
                CBigint = 1,
                CReal = 1.0f,
                CNumeric = 1,
                CSerial = 1,
                CSmallint = 1,
                CDecimal = 1,
                CDate = DateTime.Now,
                CTimestamp = DateTime.Now,
                CBoolean = true,
                CChar = "a",
                CInteger = 1,
                CText = "ab",
                CVarchar = "abc",
                CCharacterVarying = "abcd",
                CTextArray = new string[] { "a", "b" },
                CIntegerArray = new int[] { 1, 2 }
            };
            var insertedId = await QuerySql.InsertNodePostgresType(nodePostgresTypeArgs);

            var actual = await QuerySql.GetNodePostgresType(new QuerySql.GetNodePostgresTypeArgs
            {
                Id = insertedId
            });

            ClassicAssert.IsNotNull(actual);
            Assert.That(Equals(actual, new QuerySql.GetNodePostgresTypeRow
            {
                CBigint = 1,
                CReal = 1.0f,
                CSerial = 1,
                CNumeric = 1,
                CDecimal = 1,
                CSmallint = 1,
                CBoolean = true,
                CChar = "a",
                CInteger = 1,
                CText = "ab",
                CVarchar = "abc",
                CCharacterVarying = "abcd",
                CTextArray = new string[] { "a", "b" },
                CIntegerArray = new int[] { 1, 2 }
            }));
        }

        private static bool Equals(QuerySql.GetNodePostgresTypeRow x, QuerySql.GetNodePostgresTypeRow y)
        {
            return x.CSmallint.Equals(y.CSmallint) &&
                x.CBoolean.Equals(y.CBoolean) &&
                x.CInteger.Equals(y.CInteger) &&
                x.CBigint.Equals(y.CBigint) &&
                x.CSerial.Equals(y.CSerial) &&
                x.CDecimal.Equals(y.CDecimal) &&
                x.CNumeric.Equals(y.CNumeric) &
                x.CReal.Equals(y.CReal) &&
                x.CChar.Equals(y.CChar) &&
                x.CVarchar.Equals(y.CVarchar) &&
                x.CCharacterVarying.Equals(y.CCharacterVarying) &&
                x.CText.Equals(y.CText) &&
                x.CTextArray.SequenceEqual(y.CTextArray) &&
                x.CIntegerArray.SequenceEqual(y.CIntegerArray);
        }
    }
}