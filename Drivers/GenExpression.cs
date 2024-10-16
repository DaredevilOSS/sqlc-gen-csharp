using System;
using System.Linq;
using System.Text;

namespace SqlcGenCsharp.Drivers;

public class GenExpression(string expression, bool isAsync, bool isResource)
{
    private string Expression { get; } = expression;
    private bool IsAsync { get; } = isAsync;
    private bool IsResource { get; } = isResource;

    public string Generate(DotnetFramework dotnetFramework, GenExpression[] expressions)
    {
        if (!dotnetFramework.LatestDotnetSupported())
        {
            return "";
        }

        var generatedExpressions = string.Join(
            Environment.NewLine,
            expressions.Select(e => e.Generate(dotnetFramework))
        );
        return $"{Generate(dotnetFramework)}\n{generatedExpressions}";
    }

    public string Generate(DotnetFramework dotnetFramework)
    {
        return dotnetFramework.LatestDotnetSupported() ? AsStatement() : "";
    }

    private string AsStatement()
    {
        var sb = new StringBuilder();
        if (IsAsync)
            sb.Append("await ");
        if (IsResource)
            sb.Append("using ");
        sb.Append(Expression);
        return sb.ToString();
    }
}