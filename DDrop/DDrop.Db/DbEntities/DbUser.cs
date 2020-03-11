using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDrop.Db.DbEntities
{
    [Table("Users")]
    public class DbUser
    {
        [Key]
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] UserPhoto { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<DbSeries> UserSeries { get; set; }
    }
}
