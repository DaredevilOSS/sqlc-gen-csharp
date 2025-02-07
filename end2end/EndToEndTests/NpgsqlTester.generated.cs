using NpgsqlExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    [TestFixture]
    public partial class NpgsqlTester
    {
        [Test]
        public async Task TestOne()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
            var actual = await this.QuerySql.GetAuthor(new QuerySql.GetAuthorArgs { Name = DataGenerator.BojackAuthor });
            Assert.That(actual is { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme });
        }

        [Test]
        public async Task TestMany()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
            var actual = await this.QuerySql.ListAuthors();
            Assert.That(actual is [{ Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme }, { Name: DataGenerator.DrSeussAuthor, Bio: DataGenerator.DrSeussQuote }]);
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
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            var affectedRows = await this.QuerySql.UpdateAuthors(new QuerySql.UpdateAuthorsArgs { Bio = DataGenerator.BojackTheme });
            ClassicAssert.AreEqual(2, affectedRows);
        }

        [Test]
        public async Task TestExecLastId()
        {
            var genericId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            var actual = await QuerySql.GetAuthorById(new QuerySql.GetAuthorByIdArgs { Id = genericId });
            Assert.That(actual is { Name: DataGenerator.GenericAuthor, Bio: DataGenerator.GenericQuote1 });
        }

        [Test]
        public async Task TestJoinEmbed()
        {
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await QuerySql.CreateBook(new QuerySql.CreateBookArgs { Name = DataGenerator.BojackBookTitle, AuthorId = bojackId });
            var drSeussId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.DrSeussAuthor, Bio = DataGenerator.DrSeussQuote });
            await QuerySql.CreateBook(new QuerySql.CreateBookArgs { Name = DataGenerator.DrSeussBookTitle, AuthorId = drSeussId });
            var actual = await QuerySql.ListAllAuthorsBooks();
            Assert.That(actual is [{ Author: { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme }, Book.Name: DataGenerator.BojackBookTitle }, { Author: { Name: DataGenerator.DrSeussAuthor, Bio: DataGenerator.DrSeussQuote }, Book.Name: DataGenerator.DrSeussBookTitle }]);
        }

        [Test]
        public async Task TestSelfJoinEmbed()
        {
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            var actual = await QuerySql.GetDuplicateAuthors();
            Assert.That(actual is [{ Author: { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme }, Author2: { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme } }]);
            Assert.That(actual[0].Author.Id, Is.Not.EqualTo(actual[0].Author2.Id));
        }

        [Test]
        public async Task TestArray()
        {
            var genericId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            var actual = await QuerySql.GetAuthorsByIds(new QuerySql.GetAuthorsByIdsArgs { LongArr1 = [genericId, bojackId] });
            ClassicAssert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task TestMultipleArrays()
        {
            var genericId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            await this.QuerySql.CreateAuthor(new QuerySql.CreateAuthorArgs { Name = DataGenerator.GenericAuthor, Bio = DataGenerator.GenericQuote1 });
            var bojackId = await this.QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.BojackTheme });
            var actual = await QuerySql.GetAuthorsByIdsAndNames(new QuerySql.GetAuthorsByIdsAndNamesArgs { LongArr1 = [genericId, bojackId], StringArr2 = [DataGenerator.GenericAuthor] });
            ClassicAssert.AreEqual(1, actual.Count);
        }
    }
}
