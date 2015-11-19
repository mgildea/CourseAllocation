namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class preferencesIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StudentPreferences", "IsActive", c => c.Boolean(nullable: false));

            Sql("Update StudentPreferences Set IsActive = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.StudentPreferences", "IsActive");
        }
    }
}
