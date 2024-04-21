using System;
using System.IO;
using System.Threading.Tasks;

namespace SqlcGenCsharp;

public sealed class DebugHelper : IDisposable, IAsyncDisposable
{
    private const string DebugFile = "./debug.log";
    private static DebugHelper? _instance;

    private DebugHelper()
    {
        Writer = File.CreateText(DebugFile);
    }

    private StreamWriter Writer { get; }

    public static DebugHelper Instance => _instance ??= new DebugHelper();

    public async ValueTask DisposeAsync()
    {
        await Writer.DisposeAsync();
    }

    public void Dispose()
    {
        Writer.Dispose();
    }

    public void Append(string message)
    {
        Writer.Write($"{DateTime.Now:MM/dd/yyyy h:mm tt}{message}{Environment.NewLine}");
    }
}