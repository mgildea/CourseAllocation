namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class missingstudents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recommendations", "MissingSeats", c => c.Int(nullable: false));
            DropColumn("dbo.Recommendations", "MaxClassSize");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recommendations", "MaxClassSize", c => c.Double(nullable: false));
            DropColumn("dbo.Recommendations", "MissingSeats");
        }
    }
}
