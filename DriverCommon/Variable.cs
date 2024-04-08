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
        switch (me)
        {
            case Variable.ConnectionString:
            case Variable.Connection:
            case Variable.Command:
            case Variable.Reader:
            case Variable.Rows:
                return me.ToString().FirstCharToLower();
            default:
                throw new ArgumentOutOfRangeException(nameof(me), me, null);
        }
    }
}