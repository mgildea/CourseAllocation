namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class studentlimit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseSemesters", "StudentLimit", c => c.Int(nullable: false));

            Sql("Update CourseSemesters Set StudentLimit = 100;");
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseSemesters", "StudentLimit");
        }
    }
}
