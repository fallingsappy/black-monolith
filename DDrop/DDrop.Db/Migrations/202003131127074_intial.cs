namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intial : DbMigration
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
                        Content = c.Binary(),
                        AddedDate = c.String(),
                        CurrentSeriesId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.DropPhotoId)
                .ForeignKey("dbo.Series", t => t.CurrentSeriesId, cascadeDelete: true)
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
                        PixelsInMillimeter = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReferencePhotoId)
                .ForeignKey("dbo.Series", t => t.ReferencePhotoId)
                .Index(t => t.ReferencePhotoId);
            
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
                .PrimaryKey(t => t.SimpleLineId)
                .ForeignKey("dbo.ReferencePhotos", t => t.SimpleLineId)
                .ForeignKey("dbo.DropPhotos", t => t.SimpleLineId)
                .Index(t => t.SimpleLineId);
            
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
            DropForeignKey("dbo.SimpleLines", "SimpleLineId", "dbo.DropPhotos");
            DropForeignKey("dbo.Drops", "DropId", "dbo.DropPhotos");
            DropForeignKey("dbo.ReferencePhotos", "ReferencePhotoId", "dbo.Series");
            DropForeignKey("dbo.SimpleLines", "SimpleLineId", "dbo.ReferencePhotos");
            DropForeignKey("dbo.DropPhotos", "CurrentSeriesId", "dbo.Series");
            DropForeignKey("dbo.Series", "CurrentUserId", "dbo.Users");
            DropIndex("dbo.Drops", new[] { "DropId" });
            DropIndex("dbo.SimpleLines", new[] { "SimpleLineId" });
            DropIndex("dbo.ReferencePhotos", new[] { "ReferencePhotoId" });
            DropIndex("dbo.Series", new[] { "CurrentUserId" });
            DropIndex("dbo.DropPhotos", new[] { "CurrentSeriesId" });
            DropTable("dbo.Drops");
            DropTable("dbo.SimpleLines");
            DropTable("dbo.ReferencePhotos");
            DropTable("dbo.Users");
            DropTable("dbo.Series");
            DropTable("dbo.DropPhotos");
        }
    }
}
