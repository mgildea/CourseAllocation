namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class semesters : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseSemesters",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Course_ID = c.Int(nullable: false),
                        Semester_Type = c.Int(nullable: false),
                        Semester_Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.Course_ID, cascadeDelete: true)
                .ForeignKey("dbo.Semesters", t => new { t.Semester_Type, t.Semester_Year }, cascadeDelete: true)
                .Index(t => t.Course_ID)
                .Index(t => new { t.Semester_Type, t.Semester_Year });
            
            CreateTable(
                "dbo.Semesters",
                c => new
                    {
                        Type = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Type, t.Year });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseSemesters", new[] { "Semester_Type", "Semester_Year" }, "dbo.Semesters");
            DropForeignKey("dbo.CourseSemesters", "Course_ID", "dbo.Courses");
            DropIndex("dbo.CourseSemesters", new[] { "Semester_Type", "Semester_Year" });
            DropIndex("dbo.CourseSemesters", new[] { "Course_ID" });
            DropTable("dbo.Semesters");
            DropTable("dbo.CourseSemesters");
        }
    }
}
