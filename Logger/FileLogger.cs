using Microsoft.Extensions.Logging;
using System;
using System.IO;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    private readonly string _categoryName;

    public FileLogger(string categoryName, string filePath)
    {
        _categoryName = categoryName;
        _filePath = filePath;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var dateTime = DateTime.Now.ToString("dd-MM-yyyy - HH:mm:ss");

        var message = $"(host: {_categoryName}) :: {formatter(state, exception)}";

        string color = logLevel switch
        {
            LogLevel.Information => "\x1b[32m",
            LogLevel.Warning => "\x1b[33m",
            LogLevel.Error => "\x1b[31m",
            _ => "\x1b[0m",
        };

        string resetColor = "\x1b[0m";

        Console.ForegroundColor = GetConsoleColorFromLogLevel(logLevel);

        Console.WriteLine($"{resetColor}({dateTime}) {color}({logLevel}){resetColor} {message}");
        Console.ResetColor();

        File.AppendAllText(_filePath, message + Environment.NewLine);
    }

    private static ConsoleColor GetConsoleColorFromLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Information => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => ConsoleColor.Gray,
        };
    }
}
