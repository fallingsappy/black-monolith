namespace DDrop.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DropPhotoes",
                c => new
                    {
                        DropPhotoId = c.Guid(nullable: false),
                        Name = c.String(),
                        XDiameterInPixels = c.Int(nullable: false),
                        YDiameterInPixels = c.Int(nullable: false),
                        ZDiameterInPixels = c.Int(nullable: false),
                        Content = c.Binary(),
                        AddedDate = c.String(),
                        Drop_DropId = c.Guid(),
                        SimpleHorizontalLine_SimpleLineId = c.Guid(),
                        SimpleVerticalLine_SimpleLineId = c.Guid(),
                        Series_SeriesId = c.Guid(),
                    })
                .PrimaryKey(t => t.DropPhotoId)
                .ForeignKey("dbo.Drops", t => t.Drop_DropId)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleHorizontalLine_SimpleLineId)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleVerticalLine_SimpleLineId)
                .ForeignKey("dbo.Series", t => t.Series_SeriesId)
                .Index(t => t.Drop_DropId)
                .Index(t => t.SimpleHorizontalLine_SimpleLineId)
                .Index(t => t.SimpleVerticalLine_SimpleLineId)
                .Index(t => t.Series_SeriesId);
            
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
                .PrimaryKey(t => t.DropId);
            
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
                "dbo.ReferencePhotoes",
                c => new
                    {
                        ReferencePhotoId = c.Guid(nullable: false),
                        Name = c.String(),
                        Content = c.Binary(),
                        PixelsInMillimeter = c.Int(nullable: false),
                        SimpleLine_SimpleLineId = c.Guid(),
                    })
                .PrimaryKey(t => t.ReferencePhotoId)
                .ForeignKey("dbo.SimpleLines", t => t.SimpleLine_SimpleLineId)
                .Index(t => t.SimpleLine_SimpleLineId);
            
            CreateTable(
                "dbo.Series",
                c => new
                    {
                        SeriesId = c.Guid(nullable: false),
                        Title = c.String(),
                        IntervalBetweenPhotos = c.Double(nullable: false),
                        AddedDate = c.String(),
                        ReferencePhotoForSeries_ReferencePhotoId = c.Guid(),
                        User_UserId = c.Guid(),
                    })
                .PrimaryKey(t => t.SeriesId)
                .ForeignKey("dbo.ReferencePhotoes", t => t.ReferencePhotoForSeries_ReferencePhotoId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.ReferencePhotoForSeries_ReferencePhotoId)
                .Index(t => t.User_UserId);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Series", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Series", "ReferencePhotoForSeries_ReferencePhotoId", "dbo.ReferencePhotoes");
            DropForeignKey("dbo.DropPhotoes", "Series_SeriesId", "dbo.Series");
            DropForeignKey("dbo.ReferencePhotoes", "SimpleLine_SimpleLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.DropPhotoes", "SimpleVerticalLine_SimpleLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.DropPhotoes", "SimpleHorizontalLine_SimpleLineId", "dbo.SimpleLines");
            DropForeignKey("dbo.DropPhotoes", "Drop_DropId", "dbo.Drops");
            DropIndex("dbo.Series", new[] { "User_UserId" });
            DropIndex("dbo.Series", new[] { "ReferencePhotoForSeries_ReferencePhotoId" });
            DropIndex("dbo.ReferencePhotoes", new[] { "SimpleLine_SimpleLineId" });
            DropIndex("dbo.DropPhotoes", new[] { "Series_SeriesId" });
            DropIndex("dbo.DropPhotoes", new[] { "SimpleVerticalLine_SimpleLineId" });
            DropIndex("dbo.DropPhotoes", new[] { "SimpleHorizontalLine_SimpleLineId" });
            DropIndex("dbo.DropPhotoes", new[] { "Drop_DropId" });
            DropTable("dbo.Users");
            DropTable("dbo.Series");
            DropTable("dbo.ReferencePhotoes");
            DropTable("dbo.SimpleLines");
            DropTable("dbo.Drops");
            DropTable("dbo.DropPhotoes");
        }
    }
}
