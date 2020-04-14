using System.Data.Entity;
using DDrop.Db.DbEntities;

namespace DDrop.Db
{
    public class DDropContext : DbContext
    {
        public DDropContext() : base("DDropDataBase")
        {

        }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbSeries> Series { get; set; }
        public DbSet<DbDropPhoto> DropPhotos { get; set; }
        public DbSet<DbContour> Contours { get; set; }
        public DbSet<DbDrop> Drops { get; set; }
        public DbSet<DbReferencePhoto> ReferencePhotos { get; set; }
        public DbSet<DbSimpleLine> SimpleLines { get; set; }
        public DbSet<DbLogEntry> LogEntries { get; set; }

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

            modelBuilder.Entity<DbContour>()
                .HasRequired(s => s.CurrentDropPhoto)
                .WithOptional(s => s.Contour);

            modelBuilder.Entity<DbContour>()
                .HasMany(s => s.SimpleLines);

            modelBuilder.Entity<DbDropPhoto>()
                .HasOptional(c => c.SimpleHorizontalLine)
                .WithMany()
                .HasForeignKey(s => s.SimpleHorizontalLineId);

            modelBuilder.Entity<DbDropPhoto>()
                .HasOptional(c => c.SimpleVerticalLine)
                .WithMany()
                .HasForeignKey(s => s.SimpleVerticalLineId);

            modelBuilder.Entity<DbReferencePhoto>()
                .HasOptional(c => c.SimpleReferencePhotoLine)
                .WithMany()
                .HasForeignKey(s => s.SimpleReferencePhotoLineId);
        }
    }
}
