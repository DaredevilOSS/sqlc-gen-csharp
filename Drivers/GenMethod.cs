using System;
using System.Linq;

namespace SqlcGenCsharp.Drivers;

public class GenMethod(GenExpression[] generatedExpressions, DotnetFramework dotnetFramework)
{
    public string Generate()
    {
        if (dotnetFramework.LatestDotnetSupported())
        {
            return string.Join(
                Environment.NewLine,
                generatedExpressions.Select(e => e.Generate(dotnetFramework))
            );
        }
        return generatedExpressions[0].Generate(dotnetFramework, generatedExpressions.Skip(1).ToArray());
    }
}