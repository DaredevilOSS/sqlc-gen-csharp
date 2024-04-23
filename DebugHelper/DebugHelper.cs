using System;
using System.IO;

namespace SqlcGenCsharp;

public sealed class DebugHelper
{
    private const string DebugFile = "debug.log";
    private static DebugHelper? _instance;
    private StreamWriter Writer { get; }

    private DebugHelper()
    {
        Writer = File.CreateText(DebugFile);
        Writer.AutoFlush = true;
    }

    public static DebugHelper Instance => _instance ??= new DebugHelper();

    public void Append(string message)
    {
        Writer.WriteLine($"{DateTime.Now:u} {message}");
    }
}