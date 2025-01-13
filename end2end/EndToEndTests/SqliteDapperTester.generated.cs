using SqliteDapperExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    [TestFixture]
    public partial class SqliteDapperTester
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
            await this.QuerySql.DeleteAuthor(new QuerySql.DeleteAuthorArgs { Name = DataGenerator.BojackAuthor });
            var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = DataGenerator.BojackAuthor });
            ClassicAssert.IsNull(actual);
        }

        [Test]
        public async Task TestExecRows()
        {
            var createAuthorArgs = new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            };
            await this.QuerySql.CreateAuthor(createAuthorArgs);
            await this.QuerySql.CreateAuthor(createAuthorArgs);
            var updateAuthorsArgs = new QuerySql.UpdateAuthorsArgs
            {
                Bio = DataGenerator.GenericQuote1
            };
            var affectedRows = await this.QuerySql.UpdateAuthors(updateAuthorsArgs);
            ClassicAssert.AreEqual(2, affectedRows);
        }

        [Test]
        public async Task TestExecLastId()
        {
            var createAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
            {
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            };
            var insertedId = await QuerySql.CreateAuthorReturnId(createAuthorArgs);
            var getAuthorByIdArgs = new QuerySql.GetAuthorByIdArgs
            {
                Id = insertedId
            };
            var actual = await QuerySql.GetAuthorById(getAuthorByIdArgs);
            Assert.That(actual is { Name: DataGenerator.GenericAuthor, Bio: DataGenerator.GenericQuote1 });
        }
    }
}
