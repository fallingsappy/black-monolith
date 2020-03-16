namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DropPhotos",
                c => new
                    {
                        DropPhotoId = c.Guid(nullable: false),
                        Name = c.String(),
                        XDiameterInPixels = c.Int(nullable: false),
                        YDiameterInPixels = c.Int(nullable: false),
                        ZDiameterInPixels = c.Int(nullable: false),
                        SimpleHorizontalLineId = c.Guid(),
                        SimpleVerticalLineId = c.Guid(),
                        Content = c.Binary(),
                        AddedDate = c.String(),
                        CurrentSeriesId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.DropPhotoId)
                .ForeignKey("dbo.Series", t => t.CurrentSeriesId, cascadeDelete: true)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleHorizontalLineId)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleVerticalLineId)
                .Index(t => t.SimpleHorizontalLineId)
                .Index(t => t.SimpleVerticalLineId)
                .Index(t => t.CurrentSeriesId);
            
            CreateTable(
                "dbo.Series",
                c => new
                    {
                        SeriesId = c.Guid(nullable: false),
                        Title = c.String(),
                        IntervalBetweenPhotos = c.Double(nullable: false),
                        AddedDate = c.String(),
                        CurrentUserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SeriesId)
                .ForeignKey("dbo.Users", t => t.CurrentUserId, cascadeDelete: true)
                .Index(t => t.CurrentUserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserPhoto = c.Binary(),
                        Password = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.ReferencePhotos",
                c => new
                    {
                        ReferencePhotoId = c.Guid(nullable: false),
                        Name = c.String(),
                        Content = c.Binary(),
                        SimpleReferencePhotoLineId = c.Guid(),
                        PixelsInMillimeter = c.Int(nullable: false),
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
                        SimpleLineId = c.Guid(nullable: false),
                        X1 = c.Double(nullable: false),
                        Y1 = c.Double(nullable: false),
                        X2 = c.Double(nullable: false),
                        Y2 = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.SimpleLineId);
            
            CreateTable(
                "dbo.Drops",
                c => new
                    {
                        DropId = c.Guid(nullable: false),
                        XDiameterInMeters = c.Double(nullable: false),
                        YDiameterInMeters = c.Double(nullable: false),
                        ZDiameterInMeters = c.Double(nullable: false),
                        VolumeInCubicalMeters = c.Double(nullable: false),
                        RadiusInMeters = c.Double(),
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
            DropIndex("dbo.Drops", new[] { "DropId" });
            DropIndex("dbo.ReferencePhotos", new[] { "SimpleReferencePhotoLineId" });
            DropIndex("dbo.ReferencePhotos", new[] { "ReferencePhotoId" });
            DropIndex("dbo.Series", new[] { "CurrentUserId" });
            DropIndex("dbo.DropPhotos", new[] { "CurrentSeriesId" });
            DropIndex("dbo.DropPhotos", new[] { "SimpleVerticalLineId" });
            DropIndex("dbo.DropPhotos", new[] { "SimpleHorizontalLineId" });
            DropTable("dbo.Drops");
            DropTable("dbo.SimpleLines");
            DropTable("dbo.ReferencePhotos");
            DropTable("dbo.Users");
            DropTable("dbo.Series");
            DropTable("dbo.DropPhotos");
        }
    }
}
