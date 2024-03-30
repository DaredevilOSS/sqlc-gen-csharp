namespace SqlcGenCsharp.Drivers;

public enum GeneratedMember
{
    ConnectionString, // string const
    Connection // Connection variable
}

public static class GeneratedMembersExtensions
{
    public static string GetNameAsConst(this GeneratedMember me)
    {
        Func<GeneratedMember, string> getVariableName = _ => me.ToString().FirstCharToUpper();
        getVariableName = getVariableName.Memoize();
        return getVariableName(me);
    }
    
    public static string GetNameAsVar(this GeneratedMember me)
    {
        Func<GeneratedMember, string> getVariableName = _ => me.ToString().FirstCharToLower();
        getVariableName = getVariableName.Memoize();
        return getVariableName(me);
    }
}