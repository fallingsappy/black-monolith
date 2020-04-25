using System;
using System.ComponentModel.DataAnnotations;

namespace DDrop.Db.DbEntities
{
    public class DbLogEntry
    {
        [Key] public Guid Id { get; set; }

        public string Date { get; set; }
        public string Username { get; set; }
        public string LogLevel { get; set; }
        public string LogCategory { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
    }
}