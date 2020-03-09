using DDrop.BE.Models.Entities;
using System.Data.Entity;

namespace DDrop.DAL
{
    public class DDropContext : DbContext
    {
        public DDropContext() : base("DDropDataBase")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<DropPhoto> DropPhotos { get; set; }
        public DbSet<Drop> Drops { get; set; }
        public DbSet<ReferencePhoto> ReferencePhotos { get; set; }
        public DbSet<SimpleLine> SimpleLines { get; set; }
    }
}
