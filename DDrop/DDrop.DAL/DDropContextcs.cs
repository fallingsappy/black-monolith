using DDrop.BE.Models;
using System.Data.Entity;

namespace DDrop.DAL
{
    class DDropContextcs : DbContext
    {
        public DDropContextcs() : base("DDropDataBase")
        {

        }

        public DbSet<UserViewModel> Users { get; set; }
        public DbSet<SeriesViewModel> Series { get; set; }
        public DbSet<DropPhotoViewModel> DropPhotos { get; set; }
        public DbSet<DropViewModel> Drops { get; set; }
        public DbSet<ReferencePhotoViewModel> ReferencePhotos { get; set; }
    }
}
