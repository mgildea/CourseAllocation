namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removerecomendationname : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Recommendations", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recommendations", "Name", c => c.DateTime(nullable: false));
        }
    }
}
