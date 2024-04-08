namespace SqlcGenCsharp;

public enum ClassMemberType
{
    Row,
    Args,
    Sql,
    Method
}

public static class ClassMemberTypeExtensions
{
    public static string Name(this ClassMemberType me)
    {
        return me switch
        {
            ClassMemberType.Sql => "Sql",
            ClassMemberType.Row => "Row",
            ClassMemberType.Args => "Args",
            ClassMemberType.Method => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
        };
    }
}