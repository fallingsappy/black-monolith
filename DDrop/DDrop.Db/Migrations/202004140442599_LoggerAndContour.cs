using System.Data.Entity.Migrations;

namespace DDrop.Db.Migrations
{
    public partial class LoggerAndContour : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.DbLogEntries",
                    c => new
                    {
                        Id = c.Guid(false),
                        Date = c.String(),
                        Username = c.String(),
                        LogLevel = c.String(),
                        LogCategory = c.String(),
                        Message = c.String(),
                        Details = c.String(),
                        Exception = c.String(),
                        InnerException = c.String(),
                        StackTrace = c.String()
                    })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.DbContours", "CalculationParameters", c => c.String());
            AddColumn("dbo.DbContours", "CalculationProvider", c => c.String());
            DropTable("dbo.DbLogs");
        }

        public override void Down()
        {
            CreateTable(
                    "dbo.DbLogs",
                    c => new
                    {
                        LogId = c.Guid(false),
                        DateOfAddition = c.DateTime(false),
                        Message = c.String(),
                        Details = c.String()
                    })
                .PrimaryKey(t => t.LogId);

            DropColumn("dbo.DbContours", "CalculationProvider");
            DropColumn("dbo.DbContours", "CalculationParameters");
            DropTable("dbo.DbLogEntries");
        }
    }
}