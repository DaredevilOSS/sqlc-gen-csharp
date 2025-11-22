namespace EndToEndScaffold;

public static class Consts
{
    public const string UnknownRecordValuePlaceholder = "#UnknownValue#";
    public const string UnknownNullableIndicatorPlaceholder = "#UnknownNullableIndicator#";

    public const long BojackId = 1111;
    public const string BojackAuthor = "\"Bojack Horseman\"";
    public const string BojackTheme = "\"Back in the 90s he was in a very famous TV show\"";
    public const string BojackBookTitle = "\"One Trick Pony\"";

    public const long DrSeussId = 2222;
    public const string DrSeussAuthor = "\"Dr. Seuss\"";
    public const string DrSeussQuote = "\"You'll miss the best things if you keep your eyes shut\"";
    public const string DrSeussBookTitle = "\"How the Grinch Stole Christmas!\"";

    public const string GenericAuthor = "\"Albert Einstein\"";
    public const string GenericQuote1 = "\"Quote that everyone always attribute to Einstein\"";
    public const string GenericQuote2 = "\"Only 2 things are infinite, the universe and human stupidity\"";

    public static readonly string CreateBojackAuthor = $$"""
         await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs 
         {
             Id = {{BojackId}},
             Name = {{BojackAuthor}},
             Bio = {{BojackTheme}}
         });
         """;

    public const string CreateBojackAuthorWithId = $$"""
         var bojackId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs 
         {
             Name = {{BojackAuthor}},
             Bio = {{BojackTheme}}
         });
         """;

    public const string CreateBookByBojack = $$"""
           var bojackBookId = await QuerySql.CreateBookAsync(new QuerySql.CreateBookArgs
           {
               Name = {{BojackBookTitle}},
               AuthorId = bojackId
           });
           """;

    public static readonly string CreateDrSeussAuthor = $$"""
          await this.QuerySql.CreateAuthorAsync(new QuerySql.CreateAuthorArgs 
          {
              Id = {{DrSeussId}},
              Name = {{DrSeussAuthor}},
              Bio = {{DrSeussQuote}}
          });
          """;

    public const string CreateDrSeussAuthorWithId = $$"""
      var drSeussId = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs
      {
          Name = {{DrSeussAuthor}},
          Bio = {{DrSeussQuote}}
      });
      """;

    public const string CreateBookByDrSeuss = $$"""
        var drSeussBookId = await QuerySql.CreateBookAsync(new QuerySql.CreateBookArgs
        {
            AuthorId = drSeussId,
            Name = {{DrSeussBookTitle}}
        });
        """;

    public const string CreateFirstGenericAuthor = $$"""
         var id1 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs
         {
             Name = {{GenericAuthor}},
             Bio = {{GenericQuote1}}
         });
         """;

    public const string CreateSecondGenericAuthor = $$"""
          var id2 = await this.QuerySql.CreateAuthorReturnIdAsync(new QuerySql.CreateAuthorReturnIdArgs
          {
              Name = {{GenericAuthor}},
              Bio = {{GenericQuote2}}
          });
          """;
}