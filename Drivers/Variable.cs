namespace SqlcGenCsharp.Drivers;

public enum Variable
{
    Options,
    Config,
    ConnectionString,
    Transaction,
    DataSource,
    Connection,
    Command,

    Reader,
    Writer,
    Loader,
    CsvWriter,
    NullConverterFn,
    SetConverterFn,

    Args,
    QueryParams,
    TransformedSql,
    Row,
    Result,
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

    public static string AsFieldName(this Variable me)
    {
        return $"_{me.AsVarName()}";
    }
}