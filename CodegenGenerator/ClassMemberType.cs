namespace SqlcGenCsharp;

public enum ClassMemberType
{
    Row,
    Args,
    Sql
}

public static class ClassMemberTypeExtensions
{
    public static string ToRealString(this ClassMemberType me)
    {
        return me switch
        {
            ClassMemberType.Sql => "Sql",
            ClassMemberType.Row => "Row",
            ClassMemberType.Args => "Args",
            _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
        };
    }
}