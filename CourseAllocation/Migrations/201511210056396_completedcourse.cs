namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class completedcourse : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompletedCourses",
                c => new
                    {
                        GaTechId = c.String(nullable: false, maxLength: 128),
                        Course_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GaTechId, t.Course_ID })
                .ForeignKey("dbo.Courses", t => t.Course_ID, cascadeDelete: true)
                .Index(t => t.Course_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedCourses", "Course_ID", "dbo.Courses");
            DropIndex("dbo.CompletedCourses", new[] { "Course_ID" });
            DropTable("dbo.CompletedCourses");
        }
    }
}
