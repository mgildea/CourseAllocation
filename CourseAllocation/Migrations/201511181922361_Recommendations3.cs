namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Recommendations3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Recommendations", "Name", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Recommendations", "Name", c => c.String(nullable: false));
        }
    }
}
