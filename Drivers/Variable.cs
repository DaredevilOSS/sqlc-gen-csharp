namespace SqlcGenCsharp.Drivers;

public enum Variable
{
    Config,
    ConnectionString,
    Connection,
    Command,

    Reader,
    Writer,
    Loader,
    CsvWriter,

    Args,
    QueryParams,
    SqlText,
    Row,
    Result
}

public static class VariablesExtensions
{
    public static string AsVarName(this Variable me)
    {
        return me.ToString().ToCamelCase();
    }

    public static string AsPropertyName(this Variable me)
    {
        return me.ToString().ToPascalCase();
    }
}