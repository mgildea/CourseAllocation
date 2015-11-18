namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CourseSemesterSoftDelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseSemesters", "IsActive", c => c.Boolean(nullable: false));

            Sql("Update CourseSemesters Set IsActive = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseSemesters", "IsActive");
        }
    }
}
