namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Recommendation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recommendations", "MaxClassSize", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recommendations", "MaxClassSize");
        }
    }
}
