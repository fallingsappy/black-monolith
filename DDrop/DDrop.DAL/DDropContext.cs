using DDrop.BE.Models.Entities;
using DDrop.DAL.DbEntities;
using System.Data.Entity;

namespace DDrop.DAL
{
    public class DDropContext : DbContext
    {
        public DDropContext() : base("DDropDataBase")
        {

        }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbSeries> Series { get; set; }
        public DbSet<DbDropPhoto> DropPhotos { get; set; }
        public DbSet<DbDrop> Drops { get; set; }
        public DbSet<DbReferencePhoto> ReferencePhotos { get; set; }
        public DbSet<DbSimpleLine> SimpleLines { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUser>()
                .HasMany(s => s.UserSeries)
                .WithRequired(g => g.CurrentUser)
                .HasForeignKey(s => s.CurrentUserId);

            modelBuilder.Entity<DbSeries>()
                .HasMany(s => s.DropPhotosSeries)
                .WithRequired(g => g.CurrentSeries)
                .HasForeignKey(s => s.CurrentSeriesId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<DbSeries>()
                .HasRequired(s => s.ReferencePhotoForSeries)
                .WithRequiredPrincipal(ad => ad.Series);

            modelBuilder.Entity<DbDropPhoto>()
                .HasRequired(s => s.Drop)
                .WithRequiredPrincipal(ad => ad.DropPhoto);

            modelBuilder.Entity<DbDropPhoto>()
                .HasRequired(s => s.SimpleHorizontalLine)
                .WithRequiredPrincipal(ad => ad.DropPhotoHorizontalLine);

            modelBuilder.Entity<DbDropPhoto>()
                .HasRequired(s => s.SimpleVerticalLine)
                .WithRequiredPrincipal(ad => ad.DropPhotoVerticalLine);

            modelBuilder.Entity<DbReferencePhoto>()
                .HasRequired(s => s.SimpleLine)
                .WithRequiredPrincipal(ad => ad.ReferencePhoto);
        }
    }
}
