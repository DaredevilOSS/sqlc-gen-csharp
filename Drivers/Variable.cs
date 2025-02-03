namespace SqlcGenCsharp.Drivers;

public enum Variable
{
    Config,
    ConnectionString,
    Connection,
    Reader,
    Row,
    Writer,
    CsvWriter,
    Command,
    Result,
    Args,
    Loader,
    TransformedSql,
    DapperParams
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