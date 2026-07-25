namespace SqlcGenCsharp.Drivers.Generators;

// Owns every way an optional CancellationToken shows up in generated code.
// When disabled, every fragment is empty / a pass-through, so output is unchanged.
public class CancellationGen(bool enabled)
{
    private static string TokenName => Variable.CancellationToken.AsVarName();

    // Trailing optional parameter for a generated method signature.
    public string MethodParameter()
    {
        return enabled ? $"CancellationToken {TokenName} = default" : string.Empty;
    }

    // Trailing optional parameter appended after an existing parameter list, e.g. (List<T> args{TrailingMethodParameter()}).
    public string TrailingMethodParameter()
    {
        return enabled ? $", CancellationToken {TokenName} = default" : string.Empty;
    }

    // Sole argument for an otherwise parameterless async call, e.g. ReadAsync(Argument()).
    public string Argument()
    {
        return enabled ? TokenName : string.Empty;
    }

    // Trailing argument appended after existing call arguments, e.g. WriteAsync(value{TrailingArgument()}).
    public string TrailingArgument()
    {
        return enabled ? $", {TokenName}" : string.Empty;
    }

    // Wraps flat Dapper call arguments in a CommandDefinition carrying the token; pass-through when disabled.
    public string WrapDapperArgs(string flatArgs)
    {
        return enabled ? $"new CommandDefinition({flatArgs}, cancellationToken: {TokenName})" : flatArgs;
    }
}