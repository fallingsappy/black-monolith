using System;
using System.ComponentModel.DataAnnotations;

namespace DDrop.Db.DbEntities
{
    public class DbLog
    {
        [Key]
        public Guid LogId { get; set; }
        public DateTime DateOfAddition { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}