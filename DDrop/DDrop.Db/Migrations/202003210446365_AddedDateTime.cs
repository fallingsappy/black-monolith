using System.Data.Entity.Migrations;

namespace DDrop.Db.Migrations
{
    public partial class AddedDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DropPhotos", "CreationDateTime", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.DropPhotos", "CreationDateTime");
        }
    }
}