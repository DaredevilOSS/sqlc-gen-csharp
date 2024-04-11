using System;
using System.IO;

namespace SqlcGenCsharp;

public static class DebugHelper
{
    private const string DebugFile = "./debug.log";

    public static void Append(string message)
    {
        File.AppendAllText(
            DebugFile,
            $"{DateTime.Now:MM/dd/yyyy h:mm tt}{message}{Environment.NewLine}");
    }
}