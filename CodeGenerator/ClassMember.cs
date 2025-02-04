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
    public static string Name(this ClassMember me, string name)
    {
        return me switch
        {
            ClassMember.Sql => $"{name}Sql",
            ClassMember.Row => $"{name}Row",
            ClassMember.Args => $"{name}Args",
            ClassMember.Model => name.ToModelName(),
            _ => throw new ArgumentOutOfRangeException(nameof(me), me, null)
        };
    }
}