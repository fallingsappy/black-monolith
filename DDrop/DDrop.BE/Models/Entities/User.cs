using System;
using System.Collections.Generic;

namespace DDrop.BE.Models.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] UserPhoto { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<Series> UserSeries { get; set; }
    }
}
