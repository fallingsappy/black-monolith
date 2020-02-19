using DDrop.BE.Models;
using System.Data.Entity;

namespace DDrop.DAL
{
    class DDropContextcs : DbContext
    {
        public DDropContextcs() : base("DDropDataBase")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<DropPhoto> DropPhotos { get; set; }
        public DbSet<Drop> Drops { get; set; }
        public DbSet<ReferencePhoto> ReferencePhotos { get; set; }
    }
}
