namespace SqlcGenCsharp.Drivers;

public enum Variable
{
    ConnectionString,
    Connection,
    Reader,
    Command,
    Rows
}

public static class VariablesExtensions
{
    public static string Name(this Variable me)
    {
        return me.ToString().FirstCharToLower();
    }
}