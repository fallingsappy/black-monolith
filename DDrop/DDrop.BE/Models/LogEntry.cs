using DDrop.BE.Enums.Logger;

namespace DDrop.BE.Models
{
    public class LogEntry
    {
        public string Date { get; set; }
        public string Username { get; set; }
        public LogLevel LogLevel { get; set; }
        public LogCategory LogCategory { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
    }
}