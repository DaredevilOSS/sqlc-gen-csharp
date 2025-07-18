using EndToEndScaffold.Templates;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace EndToEndScaffold;

public static class Program
{
    private static readonly Dictionary<KnownTestType, TestImpl> TestImplementations =
        new List<Dictionary<KnownTestType, TestImpl>>
        {
            AnnotationTests.TestImplementations,
            MacroTests.TestImplementations,
            MySqlTests.TestImplementations,
            PostgresTests.TestImplementations,
            SqliteTests.TestImplementations
        }
            .SelectMany(d => d)
            .ToDictionary();

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
            .Select(t => GetTestImplementation(testClassName, isLegacyDotnet, t))
            .ToList());

        var optionalUsingPostgresTypes = config.TestNamespace.Contains("Npgsql") ? "using NpgsqlTypes;" : string.Empty;
        var optionalUsingSystemNet = config.TestNamespace.Contains("Npgsql") ? "using System.Net;" : string.Empty;
        var optionalUsingSystemNetNetworkInformation = config.TestNamespace.Contains("Npgsql") ? "using System.Net.NetworkInformation;" : string.Empty;
        var namespaceToTest = isLegacyDotnet ? config.LegacyTestNamespace : config.TestNamespace;
        var optionalUsingSystemTextJson = config.TestNamespace.Contains("MySqlConnector") || config.TestNamespace.Contains("Npgsql") ? "using System.Text.Json;" : string.Empty;

        return ParseCompilationUnit(
            $$"""
                 using {{namespaceToTest}};
                 {{optionalUsingPostgresTypes}}
                 {{optionalUsingSystemNet}}
                 {{optionalUsingSystemNetNetworkInformation}}
                 {{optionalUsingSystemTextJson}}
                 using NUnit.Framework;
                 using NUnit.Framework.Legacy;
                 using System;
                 using System.Collections.Generic;
                 using System.Linq;
                 using System.Threading.Tasks;

                 namespace EndToEndTests
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

    private static bool RecordsAreInUse(string testClassName, bool isLegacyDotnet)
    {
        if (isLegacyDotnet)
            return false;
        return !testClassName.Contains("Dapper");
    }

    private static string GetTestImplementation(string testClassName, bool isLegacyDotnet, KnownTestType testType)
    {
        var testGen = TestImplementations[testType];
        var impl = testGen.Impl
            .Replace(Consts.UnknownRecordValuePlaceholder,
                RecordsAreInUse(testClassName, isLegacyDotnet) ? ".Value" : string.Empty)
            .Replace(Consts.UnknownNullableIndicatorPlaceholder,
                isLegacyDotnet ? string.Empty : "?");
        return impl;
    }
}