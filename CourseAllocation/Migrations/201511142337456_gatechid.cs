namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gatechid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "GaTechId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "GaTechId");
        }
    }
}
