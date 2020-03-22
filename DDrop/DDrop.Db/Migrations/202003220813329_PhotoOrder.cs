namespace DDrop.Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhotoOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DropPhotos", "PhotoOrderInSeries", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DropPhotos", "PhotoOrderInSeries");
        }
    }
}
