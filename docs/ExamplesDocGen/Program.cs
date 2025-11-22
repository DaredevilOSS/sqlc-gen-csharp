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
        var codegenArray = (YamlSequenceNode)item["codegen"];
        var codegenObj = (YamlMappingNode)codegenArray.Children[0];

        var outputDirectory = codegenObj["out"].ToString();
        var projectName = outputDirectory.Replace("examples/", "");
        var testProject = projectName.Contains("Legacy") ? "EndToEndTestsLegacy" : "EndToEndTests";
        var testClassName = projectName.Replace("Example", "Tester");
        if (testClassName.Contains("Legacy"))
            testClassName = testClassName.Replace("Legacy", "");

        var optionsStr = StringifyOptions(codegenObj);

        return $"""
                <details>
                <summary>{projectName.Replace("Example", "")}</summary>
                
                ## Engine `{item["engine"]}`: [{projectName}]({outputDirectory})
                ### [Schema]({GetYamlFirstValue(item["schema"])}) | [Queries]({GetYamlFirstValue(item["queries"])}) | [End2End Test](end2end/{testProject}/{testClassName}.cs)
                ### Config
                ```yaml
                {optionsStr}```
                
                </details>
                """;
    }

    private static string GetYamlFirstValue(YamlNode node)
    {
        return node is YamlSequenceNode sequence
            ? sequence[0].ToString()
            : node.ToString();
    }

    private static string StringifyOptions(YamlMappingNode codegenObj)
    {
        if (!codegenObj.Children.ContainsKey(new YamlScalarNode("options")))
            return string.Empty;

        var yamlStream = new YamlStream();
        yamlStream.Documents.Add(new YamlDocument(codegenObj["options"]));
        using var writer = new StringWriter();
        yamlStream.Save(writer, false);
        return writer.ToString().Trim().TrimEnd('.');
    }
}