using System.Data.Entity.Migrations;

namespace DDrop.Db.Migrations
{
    public partial class ContourId : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.SimpleLines", "DbContour_ContourId", "ContourId");
            RenameIndex("dbo.SimpleLines", "IX_DbContour_ContourId", "IX_ContourId");
        }

        public override void Down()
        {
            RenameIndex("dbo.SimpleLines", "IX_ContourId", "IX_DbContour_ContourId");
            RenameColumn("dbo.SimpleLines", "ContourId", "DbContour_ContourId");
        }
    }
}