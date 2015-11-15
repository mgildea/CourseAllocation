namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class preferences : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentPreferences",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GaTechId = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.StudentPreferenceCourses",
                c => new
                    {
                        StudentPreference_ID = c.Int(nullable: false),
                        Course_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StudentPreference_ID, t.Course_ID })
                .ForeignKey("dbo.StudentPreferences", t => t.StudentPreference_ID, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.Course_ID, cascadeDelete: true)
                .Index(t => t.StudentPreference_ID)
                .Index(t => t.Course_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentPreferenceCourses", "Course_ID", "dbo.Courses");
            DropForeignKey("dbo.StudentPreferenceCourses", "StudentPreference_ID", "dbo.StudentPreferences");
            DropIndex("dbo.StudentPreferenceCourses", new[] { "Course_ID" });
            DropIndex("dbo.StudentPreferenceCourses", new[] { "StudentPreference_ID" });
            DropTable("dbo.StudentPreferenceCourses");
            DropTable("dbo.StudentPreferences");
        }
    }
}
