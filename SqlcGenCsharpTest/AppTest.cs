// using System.Diagnostics;
// using Microsoft.IO;
// using Plugin;
// using SqlcGenCsharp;
//
// namespace SqlcGenCsharpTest;
//
// [TestFixture]
// [TestOf(typeof(App))]
// public class AppTest
// {
//     [SetUp]
//     public void SetUp()
//     {
//         _memoryStreamManager = new RecyclableMemoryStreamManager();
//         _runTestSetupSqlc();
//     }
//
//     private static void _runTestSetupSqlc()
//     {
//         // var bashCommand = "sqlc -f examples/sqlc.test.yaml generate";
//         const string bashCommand = "sqlc --help";
//         var startInfo = new ProcessStartInfo
//         {
//             FileName = "/bin/bash",
//             Arguments = $"-c \"{bashCommand}\"",
//             RedirectStandardError = true,
//             RedirectStandardOutput = true,
//             UseShellExecute = false,
//             CreateNoWindow = true
//         };
//
//         using var process = Process.Start(startInfo)!;
//         var output = process.StandardOutput.ReadToEnd();
//         var error = process.StandardError.ReadToEnd();
//         Console.WriteLine($"sqlc output: {output}\nsqlc error: {error}");
//         process.WaitForExit();
//     }
//
//     private RecyclableMemoryStreamManager _memoryStreamManager = null!;
//
//     private static IEnumerable<TestCaseData> ExamplesTestData
//     {
//         get
//         {
//             // TODO implement
//             yield return new TestCaseData(1, 2, 3, 5);
//         }
//     }
//
//     [Test]
//     [TestCaseSource(nameof(ExamplesTestData))]
//     public void TestRun(GenerateRequest testRequest, GenerateResponse expectedResponse)
//     {
//         var originalStdIn = Console.In;
//         var originalStdOut = Console.Out;
//         try
//         {
//             using var inStreamReader = new StreamReader(testRequest.ToStream(_memoryStreamManager));
//             var outputStream = expectedResponse.ToStream(_memoryStreamManager);
//             using var outStreamWriter = new StreamWriter(outputStream);
//             Console.SetIn(inStreamReader);
//             Console.SetOut(outStreamWriter);
//
//             App.Run();
//             Assert.Multiple(() =>
//             {
//                 Assert.That(outStreamWriter.BaseStream.Position, Is.EqualTo(0),
//                     "Output stream was not correctly reset");
//                 Assert.That(outputStream.ContentEquals(expectedResponse.ToStream(_memoryStreamManager)),
//                     "Content of output stream does match the expected response");
//             });
//         }
//         finally
//         {
//             Console.SetIn(originalStdIn);
//             Console.SetOut(originalStdOut);
//         }
//     }
// }