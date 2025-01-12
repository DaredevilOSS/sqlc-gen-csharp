using SqlcGenCsharp;
using YamlDotNet.RepresentationModel;

namespace DocsGenerator;

public static class ExamplesDocGen
{
    public static void Main()
    {
        var yamlContent = File.ReadAllText("sqlc.ci.yaml");
        var yaml = new YamlStream();
        yaml.Load(new StringReader(yamlContent));

        var root = (YamlMappingNode)yaml.Documents[0].RootNode;
        var sqlArray = (YamlSequenceNode)root.Children[new YamlScalarNode("sql")];

        var exampleNodes = sqlArray.Children.Select(ParseConfigNode).ToList();
        var contents = $"""
                        # Examples
                        {exampleNodes.JoinByNewLine()}
                        """;
        using var stdout = Console.OpenStandardOutput();
        Console.Write(contents);
    }

    private static string ParseConfigNode(YamlNode node)
    {
        var item = (YamlMappingNode)node;
        var queryFiles = item["queries"].ToString();
        var codegenArray = (YamlSequenceNode)item["codegen"];
        var firstCodegenObj = (YamlMappingNode)codegenArray.Children[0];

        var outputDirectory = firstCodegenObj["out"].ToString();
        var projectName = outputDirectory.Replace("examples/", "");
        var testProject = projectName.Contains("Legacy") ? "EndToEndTestsLegacy" : "EndToEndTests";
        var testClassName = projectName.Replace("Example", "Tester");
        if (testClassName.Contains("Legacy"))
            testClassName = testClassName.Replace("Legacy", "");

        var yamlStream = new YamlStream();
        var yamlDocument = new YamlDocument(firstCodegenObj["options"]);
        yamlStream.Documents.Add(yamlDocument);
        using var optionsWriter = new StringWriter();
        yamlStream.Save(optionsWriter, false);
        var optionsStr = optionsWriter.ToString().Trim().TrimEnd('.');

        return $"""
                ## Engine `{item["engine"]}`: [{projectName}]({outputDirectory})

                ### [Schema]({item["schema"]}) | [Queries]({queryFiles}) | [End2End Test]({testProject}/{testClassName}.cs)

                ### Config
                ```yaml
                {optionsStr}```
                """;
    }
}