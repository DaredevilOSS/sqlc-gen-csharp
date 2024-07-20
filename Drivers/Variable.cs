namespace SqlcGenCsharp.Drivers;

public enum Variable
{
    ConnectionString,
    Connection,
    Reader,
    Row,
    Writer,
    Command,
    Result
}

public static class VariablesExtensions
{
    public static string Name(this Variable me)
    {
        return me.ToString().FirstCharToLower();
    }
}