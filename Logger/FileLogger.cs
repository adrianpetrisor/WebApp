using Microsoft.Extensions.Logging;
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

        var message = $"{DateTime.Now:u} [{logLevel}] {_categoryName}: {formatter(state, exception)}";
        Console.WriteLine(message);
        File.AppendAllText(_filePath, message + Environment.NewLine);
    }
}
