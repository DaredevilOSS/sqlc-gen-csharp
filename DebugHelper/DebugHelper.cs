using System;
using System.IO;

namespace SqlcGenCsharp;

public sealed class DebugHelper
{
    private const string DebugFile = "debug.log";
    private static DebugHelper? _instance;
    
    private StreamWriter? Writer { get; }
    private bool Enabled { get; }
    
    private DebugHelper()
    {
        try
        {
            Writer = File.CreateText(DebugFile);
            Writer.AutoFlush = true;
            Enabled = true;
        }
        // TODO refactor - identify runtime via environment? something else?
        catch (DirectoryNotFoundException)
        {
            Enabled = false;
        }
    }
    
    public static DebugHelper Instance => _instance ??= new DebugHelper();

    public void Append(string message)
    {
        if (!Enabled) return;
        Writer!.WriteLine($"{DateTime.Now:u} {message}");
    }
}