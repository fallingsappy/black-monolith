using System;
using DDrop.BE.Enums.Logger;
using DDrop.BE.Models;
using DDrop.DAL;
using DDrop.Db.DbEntities;

namespace DDrop.Utility.Logger
{
    public class Logger : ILogger
    {
        private readonly IDDropRepository _dDropRepository;

        public Logger(IDDropRepository dDropRepository)
        {
            _dDropRepository = dDropRepository;
        }

        public async void Log(LogEntry logEntry)
        {
            await _dDropRepository.SaveLogEntry(new DbLogEntry
            {
                Id = Guid.NewGuid(),
                Exception = logEntry.Exception,
                LogCategory = logEntry.LogCategory.ToString(),
                LogLevel = logEntry.LogLevel.ToString(),
                Date = logEntry.Date,
                Details = logEntry.Details,
                InnerException = logEntry.InnerException,
                Message = logEntry.Message,
                StackTrace = logEntry.StackTrace,
                Username = logEntry.Username
            });
        }

        public async void LogInfo(LogEntry logEntry)
        {
            await _dDropRepository.SaveLogEntry(new DbLogEntry
            {
                Id = Guid.NewGuid(),
                Exception = logEntry.Exception,
                LogCategory = logEntry.LogCategory.ToString(),
                LogLevel = LogLevel.Info.ToString(),
                Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Details = logEntry.Details,
                InnerException = logEntry.InnerException,
                Message = logEntry.Message,
                StackTrace = logEntry.StackTrace,
                Username = logEntry.Username
            });
        }

        public async void LogWarning(LogEntry logEntry)
        {
            await _dDropRepository.SaveLogEntry(new DbLogEntry
            {
                Id = Guid.NewGuid(),
                Exception = logEntry.Exception,
                LogCategory = logEntry.LogCategory.ToString(),
                LogLevel = LogLevel.Warning.ToString(),
                Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Details = logEntry.Details,
                InnerException = logEntry.InnerException,
                Message = logEntry.Message,
                StackTrace = logEntry.StackTrace,
                Username = logEntry.Username
            });
        }

        public async void LogError(LogEntry logEntry)
        {
            await _dDropRepository.SaveLogEntry(new DbLogEntry
            {
                Id = Guid.NewGuid(),
                Exception = logEntry.Exception,
                LogCategory = logEntry.LogCategory.ToString(),
                LogLevel = LogLevel.Error.ToString(),
                Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Details = logEntry.Details,
                InnerException = logEntry.InnerException,
                Message = logEntry.Message,
                StackTrace = logEntry.StackTrace,
                Username = logEntry.Username
            });
        }
    }
}