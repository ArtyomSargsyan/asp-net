using System;
using System.IO;

namespace ToDoApi.Services.Loger
{
    public sealed class FileLogger
    {
        private static readonly Lazy<FileLogger> _instance = new(() => new FileLogger());
        public static FileLogger Instance => _instance.Value;

        private readonly string _logDir;
        private static readonly object _lock = new();

        private FileLogger()
        {
          var logDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
          Directory.CreateDirectory(logDir);
          _logDir = logDir;
        }
        

        private string LogFilePath => Path.Combine(_logDir, $"{DateTime.Now:yyyy-MM-dd}.log");

        public void Info(string message) => Write("INFO", message);
        public void Warn(string message) => Write("WARN", message);
        public void Error(string message, Exception? ex = null) =>
            Write("ERROR", $"{message} {ex?.Message}");

        private void Write(string level, string message)
        {
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}{Environment.NewLine}";
            lock (_lock)
            {
                File.AppendAllText(LogFilePath, line);
            }
        }
    }
}
