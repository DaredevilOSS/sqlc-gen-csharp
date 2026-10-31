using System.Text.RegularExpressions;

namespace MssqlSyntaxMatrix;

public record CaseQuery(string Name, string Cmd, string Text);

// A case file holds a description, an optional schema and one or more sqlc queries:
//   -- @description What the case covers
//   -- @schema
//   CREATE TABLE ...
//   -- @queries
//   -- name: Foo :many
//   SELECT ...;
// cases/base.sql is prepended to the schema of every case.
public partial class TestCase
{
    public required string Id { get; init; }
    public required string Category { get; init; }
    public required string Description { get; init; }
    public required string Schema { get; init; }
    public required string QueriesText { get; init; }
    public required List<CaseQuery> Queries { get; init; }

    public static List<TestCase> LoadAll(string casesDir)
    {
        var baseSchema = File.ReadAllText(Path.Combine(casesDir, "base.sql"));
        return [.. Directory.GetFiles(casesDir, "*.sql", SearchOption.AllDirectories)
            .Where(f => Path.GetFileName(f) != "base.sql")
            .Order(StringComparer.Ordinal)
            .Select(f => Load(f, casesDir, baseSchema))];
    }

    private static TestCase Load(string file, string casesDir, string baseSchema)
    {
        var relative = Path.GetRelativePath(casesDir, file).Replace('\\', '/');
        var id = relative[..^".sql".Length];
        var description = new List<string>();
        var schema = new List<string>();
        var queries = new List<string>();
        List<string>? section = null;
        foreach (var line in File.ReadAllLines(file))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("-- @description"))
                description.Add(trimmed["-- @description".Length..].Trim());
            else if (trimmed == "-- @schema")
                section = schema;
            else if (trimmed == "-- @queries")
                section = queries;
            else
                section?.Add(line);
        }

        var queriesText = string.Join("\n", queries).Trim() + "\n";
        return new TestCase
        {
            Id = id,
            Category = id.Contains('/') ? id[..id.IndexOf('/')] : "misc",
            Description = string.Join(" ", description),
            // the base schema is its own batch, so a case can start with CREATE SCHEMA / VIEW / PROCEDURE
            Schema = (baseSchema.TrimEnd() + "\nGO\n\n" + string.Join("\n", schema)).Trim() + "\n",
            QueriesText = queriesText,
            Queries = SplitQueries(queriesText)
        };
    }

    private static List<CaseQuery> SplitQueries(string text)
    {
        var matches = NameRegex().Matches(text);
        var result = new List<CaseQuery>();
        for (var i = 0; i < matches.Count; i++)
        {
            var start = matches[i].Index + matches[i].Length;
            var end = i + 1 < matches.Count ? matches[i + 1].Index : text.Length;
            result.Add(new CaseQuery(matches[i].Groups[1].Value, matches[i].Groups[2].Value, text[start..end].Trim()));
        }
        return result;
    }

    [GeneratedRegex(@"^--\s*name:\s*(\w+)\s+(:\w+).*$", RegexOptions.Multiline)]
    private static partial Regex NameRegex();
}