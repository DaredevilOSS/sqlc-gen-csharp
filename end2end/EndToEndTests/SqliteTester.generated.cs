using SqliteExampleGen;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlcGenCsharpTests
{
    [TestFixture]
    public partial class SqliteTester
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

        [Test]
        public async Task TestJoinEmbed()
        {
            var createAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            };
            var bojackAuthorId = await QuerySql.CreateAuthorReturnId(createAuthorArgs);
            var createBookArgs = new QuerySql.CreateBookArgs
            {
                Name = DataGenerator.BojackBookTitle,
                AuthorId = bojackAuthorId
            };
            await QuerySql.CreateBook(createBookArgs);
            createAuthorArgs = new QuerySql.CreateAuthorReturnIdArgs
            {
                Name = DataGenerator.DrSeussAuthor,
                Bio = DataGenerator.DrSeussQuote
            };
            var drSeussAuthorId = await QuerySql.CreateAuthorReturnId(createAuthorArgs);
            createBookArgs = new QuerySql.CreateBookArgs
            {
                Name = DataGenerator.DrSeussBookTitle,
                AuthorId = drSeussAuthorId
            };
            await QuerySql.CreateBook(createBookArgs);
            var actual = await QuerySql.ListAllAuthorsBooks();
            Assert.That(actual is [{ Author: { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme, }, Book.Name: DataGenerator.BojackBookTitle, }, { Author: { Name: DataGenerator.DrSeussAuthor, Bio: DataGenerator.DrSeussQuote, }, Book.Name: DataGenerator.DrSeussBookTitle, }]);
        }

        [Test]
        public async Task TestSelfJoinEmbed()
        {
            var createAuthorArgs = new QuerySql.CreateAuthorArgs
            {
                Name = DataGenerator.BojackAuthor,
                Bio = DataGenerator.BojackTheme
            };
            await QuerySql.CreateAuthor(createAuthorArgs);
            await QuerySql.CreateAuthor(createAuthorArgs);
            var actual = await QuerySql.GetDuplicateAuthors();
            Assert.That(actual is [{ Author: { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme, }, Author2: { Name: DataGenerator.BojackAuthor, Bio: DataGenerator.BojackTheme, } }]);
            Assert.That(actual[0].Author.Id, Is.Not.EqualTo(actual[0].Author2.Id));
        }

        [Test]
        public async Task TestSliceIds()
        {
            var args = new QuerySql.CreateAuthorReturnIdArgs
            {
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            };
            var insertedId1 = await QuerySql.CreateAuthorReturnId(args);
            var insertedId2 = await QuerySql.CreateAuthorReturnId(args);
            await QuerySql.CreateAuthorReturnId(args);
            var actual = await QuerySql.SelectAuthorsWithSlice(new QuerySql.SelectAuthorsWithSliceArgs { Ids = [insertedId1, insertedId2] });
            ClassicAssert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task TestMultipleSlices()
        {
            var args = new QuerySql.CreateAuthorReturnIdArgs
            {
                Name = DataGenerator.GenericAuthor,
                Bio = DataGenerator.GenericQuote1
            };
            var insertedId1 = await QuerySql.CreateAuthorReturnId(args);
            var insertedId2 = await QuerySql.CreateAuthorReturnId(new QuerySql.CreateAuthorReturnIdArgs { Name = DataGenerator.BojackAuthor, Bio = DataGenerator.GenericQuote1 });
            await QuerySql.CreateAuthorReturnId(args);
            var actual = await QuerySql.SelectAuthorsWithTwoSlices(new QuerySql.SelectAuthorsWithTwoSlicesArgs { Ids = [insertedId1, insertedId2], Names = [DataGenerator.GenericAuthor] });
            ClassicAssert.AreEqual(1, actual.Count);
        }
    }
}
