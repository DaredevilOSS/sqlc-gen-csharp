using System;

namespace SqlcGenCsharp;

public enum ClassMember
{
    Row,
    Args,
    Sql,
    Model
}

public static class ClassMemberTypeExtensions
{
    public static string Name(this ClassMember me)
    {
        return me switch
        {
            ClassMember.Sql => "Sql",
            ClassMember.Row => "Row",
            ClassMember.Args => "Args",
            ClassMember.Model => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
        };
    }
}