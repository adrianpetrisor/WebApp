using Microsoft.Extensions.Logging;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;

    public FileLoggerProvider(string filePath)
    {
        _filePath = filePath;
        Console.WriteLine($"> FileLoggerProvider INIT with path: {_filePath}");
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, _filePath);
    }

    public void Dispose() { }
}
