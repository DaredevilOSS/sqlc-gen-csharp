using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EndToEndScaffold;

public static class Program
{
    public static void Main()
    {
        var testClassName = Environment.GetEnvironmentVariable("TEST_CLASS_NAME") ??
                            throw new Exception("env TEST_CLASS_NAME not set");
        var isLegacyDotnet = Environment.GetEnvironmentVariable("IS_LEGACY") == "true";
        var contents = GetFileContents(testClassName, isLegacyDotnet);
        Console.WriteLine(contents);
    }

    private static string GetFileContents(string testClassName, bool isLegacyDotnet)
    {
        if (!Config.FilesToGenerate.TryGetValue(testClassName, out var config))
            throw new ArgumentException($"No code to generate for input {testClassName}");

        var testsImplementation = string.Join("\n", config.TestTypes
            .Select(t =>
            {
                var testGen = Templates.TestImplementations[t];
                return isLegacyDotnet ? testGen.Legacy : testGen.Modern;
            })
            .ToList());
        var namespaceToTest = isLegacyDotnet ? config.LegacyTestNamespace : config.TestNamespace;
        return ParseCompilationUnit(
            $$"""
                 using {{namespaceToTest}};
                 using NUnit.Framework;
                 using NUnit.Framework.Legacy;
                 using System;
                 using System.Collections.Generic;
                 using System.Linq;
                 using System.Threading.Tasks;

                 namespace SqlcGenCsharpTests
                 {
                     [TestFixture]
                     public partial class {{testClassName}}
                     {
                        {{testsImplementation}}
                     }
                 }
                 """)
            .NormalizeWhitespace()
            .ToFullString();
    }
}