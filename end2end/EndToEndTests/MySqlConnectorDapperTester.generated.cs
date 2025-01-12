using MySqlConnectorDapperExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    [TestFixture]
    public partial class MySqlConnectorDapperTester
    {
        [Test]
        public async Task TestOne()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
            var actualAuthor = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = DataGenerator.BojackAuthor });
            ClassicAssert.IsNotNull(actualAuthor);
            Assert.That(actualAuthor is { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme });
        }

        [Test]
        public async Task TestMany()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
            var actualAuthors = await this.QuerySql.ListAuthors();
            ClassicAssert.AreEqual(2, actualAuthors.Count);
            Assert.That(actualAuthors is [{ Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme }, { Name: DataGenerator.DrSeussAuthor, Bio: DataGenerator.DrSeussQuote }]);
        }

        [Test]
        public async Task TestExec()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
            var deleteAuthorArgs = new QuerySql.DeleteAuthorArgs
            {
                Name = DataGenerator.BojackAuthor
            };
            await this.QuerySql.DeleteAuthor(deleteAuthorArgs);
            var authorRows = await this.QuerySql.ListAuthors();
            ClassicAssert.AreEqual(1, authorRows.Count);
            Assert.That(authorRows[0] is { Name: DataGenerator.DrSeussAuthor, Bio: DataGenerator.DrSeussQuote });
        }

        [Test]
        public async Task TestExecRows()
        {
            var bojackCreateAuthorArgs = new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            };
            await this.QuerySql.CreateAuthor(bojackCreateAuthorArgs);
            await this.QuerySql.CreateAuthor(bojackCreateAuthorArgs);
            var updateAuthorsArgs = new QuerySql.UpdateAuthorsArgs
            {
                Bio = DataGenerator.GenericQuote2
            };
            var affectedRows = await this.QuerySql.UpdateAuthors(updateAuthorsArgs);
            ClassicAssert.AreEqual(2, affectedRows);
        }
    }
}
