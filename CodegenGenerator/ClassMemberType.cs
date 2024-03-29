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
    public static string ToRealString(this ClassMemberType me)
    {
        return me switch
        {
            ClassMemberType.Sql => "Sql",
            ClassMemberType.Row => "Row",
            ClassMemberType.Args => "Args",
            ClassMemberType.Method => "Method",
            _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
        };
    }
    
    public static int GetClassMemberOrder(this ClassMemberType me)
    {
        return me switch
        {
            ClassMemberType.Sql => 1,
            ClassMemberType.Row => 2,
            ClassMemberType.Args => 3,
            ClassMemberType.Method => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
        };
    }
}