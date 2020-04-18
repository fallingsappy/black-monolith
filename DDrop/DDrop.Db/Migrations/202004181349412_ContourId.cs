namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContourId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.SimpleLines", name: "DbContour_ContourId", newName: "ContourId");
            RenameIndex(table: "dbo.SimpleLines", name: "IX_DbContour_ContourId", newName: "IX_ContourId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.SimpleLines", name: "IX_ContourId", newName: "IX_DbContour_ContourId");
            RenameColumn(table: "dbo.SimpleLines", name: "ContourId", newName: "DbContour_ContourId");
        }
    }
}
