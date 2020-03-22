namespace DDrop.Db.Migrations
    {
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UseAddedDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Series", "UseCreationDateTime", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Series", "UseCreationDateTime");
        }
    }
}
