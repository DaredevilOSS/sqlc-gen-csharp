namespace SqlcGenCsharp.Drivers.Common;

public enum Variables
{
    ConnectionString,
    Connection,
    Reader,
    Rows
}

public static class VariablesExtensions
{
    // TODO should be refactored to naming per variable
    public static string GetNameAsConst(this Variables me)
    {
        Func<Variables, string> getVariableName = _ => me.ToString().FirstCharToUpper();
        getVariableName = getVariableName.Memoize();
        return getVariableName(me);
    }

    public static string GetNameAsVar(this Variables me)
    {
        Func<Variables, string> getVariableName = _ => me.ToString().FirstCharToLower();
        getVariableName = getVariableName.Memoize();
        return getVariableName(me);
    }
}