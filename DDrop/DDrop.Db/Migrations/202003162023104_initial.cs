using System.Data.Entity.Migrations;

namespace DDrop.Db.Migrations
{
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.DropPhotos",
                    c => new
                    {
                        DropPhotoId = c.Guid(false),
                        Name = c.String(),
                        XDiameterInPixels = c.Int(false),
                        YDiameterInPixels = c.Int(false),
                        ZDiameterInPixels = c.Int(false),
                        SimpleHorizontalLineId = c.Guid(),
                        SimpleVerticalLineId = c.Guid(),
                        Content = c.Binary(),
                        AddedDate = c.String(),
                        CurrentSeriesId = c.Guid(false)
                    })
                .PrimaryKey(t => t.DropPhotoId)
                .ForeignKey("dbo.Series", t => t.CurrentSeriesId, true)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleHorizontalLineId)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleVerticalLineId)
                .Index(t => t.SimpleHorizontalLineId)
                .Index(t => t.SimpleVerticalLineId)
                .Index(t => t.CurrentSeriesId);

            CreateTable(
                    "dbo.Series",
                    c => new
                    {
                        SeriesId = c.Guid(false),
                        Title = c.String(),
                        IntervalBetweenPhotos = c.Double(false),
                        AddedDate = c.String(),
                        CurrentUserId = c.Guid(false)
                    })
                .PrimaryKey(t => t.SeriesId)
                .ForeignKey("dbo.Users", t => t.CurrentUserId, true)
                .Index(t => t.CurrentUserId);

            CreateTable(
                    "dbo.Users",
                    c => new
                    {
                        UserId = c.Guid(false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserPhoto = c.Binary(),
                        Password = c.String(),
                        Email = c.String()
                    })
                .PrimaryKey(t => t.UserId);

            CreateTable(
                    "dbo.ReferencePhotos",
                    c => new
                    {
                        ReferencePhotoId = c.Guid(false),
                        Name = c.String(),
                        Content = c.Binary(),
                        SimpleReferencePhotoLineId = c.Guid(),
                        PixelsInMillimeter = c.Int(false)
                    })
                .PrimaryKey(t => t.ReferencePhotoId)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleReferencePhotoLineId)
                .ForeignKey("dbo.Series", t => t.ReferencePhotoId)
                .Index(t => t.ReferencePhotoId)
                .Index(t => t.SimpleReferencePhotoLineId);

            CreateTable(
                    "dbo.SimpleLines",
                    c => new
                    {
                        SimpleLineId = c.Guid(false),
                        X1 = c.Double(false),
                        Y1 = c.Double(false),
                        X2 = c.Double(false),
                        Y2 = c.Double(false)
                    })
                .PrimaryKey(t => t.SimpleLineId);

            CreateTable(
                    "dbo.Drops",
                    c => new
                    {
                        DropId = c.Guid(false),
                        XDiameterInMeters = c.Double(false),
                        YDiameterInMeters = c.Double(false),
                        ZDiameterInMeters = c.Double(false),
                        VolumeInCubicalMeters = c.Double(false),
                        RadiusInMeters = c.Double()
                    })
                .PrimaryKey(t => t.DropId)
                .ForeignKey("dbo.DropPhotos", t => t.DropId)
                .Index(t => t.DropId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.DropPhotos", "SimpleVerticalLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.DropPhotos", "SimpleHorizontalLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.Drops", "DropId", "dbo.DropPhotos");
            DropForeignKey("dbo.ReferencePhotos", "ReferencePhotoId", "dbo.Series");
            DropForeignKey("dbo.ReferencePhotos", "SimpleReferencePhotoLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.DropPhotos", "CurrentSeriesId", "dbo.Series");
            DropForeignKey("dbo.Series", "CurrentUserId", "dbo.Users");
            DropIndex("dbo.Drops", new[] {"DropId"});
            DropIndex("dbo.ReferencePhotos", new[] {"SimpleReferencePhotoLineId"});
            DropIndex("dbo.ReferencePhotos", new[] {"ReferencePhotoId"});
            DropIndex("dbo.Series", new[] {"CurrentUserId"});
            DropIndex("dbo.DropPhotos", new[] {"CurrentSeriesId"});
            DropIndex("dbo.DropPhotos", new[] {"SimpleVerticalLineId"});
            DropIndex("dbo.DropPhotos", new[] {"SimpleHorizontalLineId"});
            DropTable("dbo.Drops");
            DropTable("dbo.SimpleLines");
            DropTable("dbo.ReferencePhotos");
            DropTable("dbo.Users");
            DropTable("dbo.Series");
            DropTable("dbo.DropPhotos");
        }
    }
}