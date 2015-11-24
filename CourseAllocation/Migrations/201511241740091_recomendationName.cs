namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recomendationName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recommendations", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recommendations", "Name");
        }
    }
}
