namespace DDrop.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DropPhotoes",
                c => new
                {
                    DropPhotoId = c.Guid(nullable: false),
                    Path = c.String(),
                    Name = c.String(),
                    XDiameterInPixels = c.Int(nullable: false),
                    YDiameterInPixels = c.Int(nullable: false),
                    ZDiameterInPixels = c.Int(nullable: false),
                    Content = c.Binary(),
                    Time = c.Int(nullable: false),
                    Drop_DropId = c.Guid(),
                    Series_SeriesId = c.Guid(),
                })
                .PrimaryKey(t => t.DropPhotoId)
                .ForeignKey("dbo.Drops", t => t.Drop_DropId)
                .ForeignKey("dbo.Series", t => t.Series_SeriesId)
                .Index(t => t.Drop_DropId)
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
                    RadiusInMeters = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.DropId);

            CreateTable(
                "dbo.ReferencePhotoes",
                c => new
                {
                    ReferencePhotoId = c.Guid(nullable: false),
                    Path = c.String(),
                    Content = c.Binary(),
                    Line_X1 = c.Double(nullable: false),
                    Line_Y1 = c.Double(nullable: false),
                    Line_X2 = c.Double(nullable: false),
                    Line_Y2 = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.ReferencePhotoId);

            CreateTable(
                "dbo.Series",
                c => new
                {
                    SeriesId = c.Guid(nullable: false),
                    Title = c.String(),
                    ExactCalculationModel = c.Boolean(nullable: false),
                    IntervalBetweenPhotos = c.Double(nullable: false),
                    CanDrawPlot = c.Boolean(nullable: false),
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
                    Password = c.String(),
                    Email = c.String(),
                    IsLoggedIn = c.Boolean(nullable: false),
                    Avatar = c.Binary(),
                })
                .PrimaryKey(t => t.UserId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Series", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Series", "ReferencePhotoForSeries_ReferencePhotoId", "dbo.ReferencePhotoes");
            DropForeignKey("dbo.DropPhotoes", "Series_SeriesId", "dbo.Series");
            DropForeignKey("dbo.DropPhotoes", "Drop_DropId", "dbo.Drops");
            DropIndex("dbo.Series", new[] { "User_UserId" });
            DropIndex("dbo.Series", new[] { "ReferencePhotoForSeries_ReferencePhotoId" });
            DropIndex("dbo.DropPhotoes", new[] { "Series_SeriesId" });
            DropIndex("dbo.DropPhotoes", new[] { "Drop_DropId" });
            DropTable("dbo.Users");
            DropTable("dbo.Series");
            DropTable("dbo.ReferencePhotoes");
            DropTable("dbo.Drops");
            DropTable("dbo.DropPhotoes");
        }
    }
}
