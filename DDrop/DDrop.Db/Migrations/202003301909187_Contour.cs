namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Contour : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DbContours",
                c => new
                    {
                        ContourId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ContourId)
                .ForeignKey("dbo.DropPhotos", t => t.ContourId)
                .Index(t => t.ContourId);
            
            CreateTable(
                "dbo.DbLogs",
                c => new
                    {
                        LogId = c.Guid(nullable: false),
                        DateOfAddition = c.DateTime(nullable: false),
                        Message = c.String(),
                        Details = c.String(),
                    })
                .PrimaryKey(t => t.LogId);
            
            AddColumn("dbo.SimpleLines", "DbContour_ContourId", c => c.Guid());
            CreateIndex("dbo.SimpleLines", "DbContour_ContourId");
            AddForeignKey("dbo.SimpleLines", "DbContour_ContourId", "dbo.DbContours", "ContourId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SimpleLines", "DbContour_ContourId", "dbo.DbContours");
            DropForeignKey("dbo.DbContours", "ContourId", "dbo.DropPhotos");
            DropIndex("dbo.SimpleLines", new[] { "DbContour_ContourId" });
            DropIndex("dbo.DbContours", new[] { "ContourId" });
            DropColumn("dbo.SimpleLines", "DbContour_ContourId");
            DropTable("dbo.DbLogs");
            DropTable("dbo.DbContours");
        }
    }
}
