namespace SqlcGenCsharp.Drivers;

public enum Variable
{
    ConnectionString,
    Connection,
    Reader,
    Row,
    Writer,
    Command,
    Result,
    Args
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