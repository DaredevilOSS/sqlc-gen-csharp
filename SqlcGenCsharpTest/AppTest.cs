using sqlc_gen_csharp;
using Microsoft.IO;
using Plugin;

namespace SqlcGenCsharpTest
{
    [TestFixture]
    [TestOf(typeof(App))]
    public class AppTest
    {
        private RecyclableMemoryStreamManager _memoryStreamManager = null!;

        [SetUp]
        public void SetUp()
        {
            _memoryStreamManager = new RecyclableMemoryStreamManager();
        }
        
        private static IEnumerable<TestCaseData> ExamplesTestData
        {
            get
            {
                // TODO implement
                yield return new TestCaseData(null, null);
            }
        }

        [Test, TestCaseSource(nameof(ExamplesTestData))]
        public void TestRun(GenerateRequest testRequest, GenerateResponse expectedResponse)
        {
            var originalStdIn = Console.In;
            var originalStdOut = Console.Out;
            try
            {
                using var inStreamReader = new StreamReader(testRequest.ToStream(_memoryStreamManager));
                var outputStream = expectedResponse.ToStream(_memoryStreamManager);
                using var outStreamWriter = new StreamWriter(outputStream);
                Console.SetIn(inStreamReader);
                Console.SetOut(outStreamWriter);
                
                App.Run();
                Assert.That(outStreamWriter.BaseStream.Position, Is.EqualTo(0), "Output stream was not correctly reset");
                Assert.That(outputStream.ContentEquals(expectedResponse.ToStream(_memoryStreamManager)));
            }
            finally
            {
                Console.SetIn(originalStdIn);
                Console.SetOut(originalStdOut);
            }
        }
    }
}