using System.Data.Entity.Migrations;

namespace DDrop.Db.Migrations
{
    public partial class UseAddedDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Series", "UseCreationDateTime", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Series", "UseCreationDateTime");
        }
    }
}