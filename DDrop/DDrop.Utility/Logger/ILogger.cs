using DDrop.BE.Models;

namespace DDrop.Utility.Logger
{
    public interface ILogger
    {
        void Log(LogEntry logEntry);
        void LogInfo(LogEntry logEntry);
        void LogWarning(LogEntry logEntry);
        void LogError(LogEntry logEntry);
    }
}