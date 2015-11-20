namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ilog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Courses", "CreatedBy_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.StudentPreferences", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.StudentPreferences", "CreatedBy_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Recommendations", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Recommendations", "CreatedBy_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.CourseSemesters", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.CourseSemesters", "CreatedBy_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Semesters", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Semesters", "CreatedBy_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Courses", "CreatedBy_Id");
            CreateIndex("dbo.StudentPreferences", "CreatedBy_Id");
            CreateIndex("dbo.Recommendations", "CreatedBy_Id");
            CreateIndex("dbo.CourseSemesters", "CreatedBy_Id");
            CreateIndex("dbo.Semesters", "CreatedBy_Id");
            AddForeignKey("dbo.Courses", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.StudentPreferences", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.CourseSemesters", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Semesters", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Recommendations", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.AspNetUsers", "GaTechId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "GaTechId", c => c.String(nullable: false));
            DropForeignKey("dbo.Recommendations", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Semesters", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseSemesters", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.StudentPreferences", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Courses", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Semesters", new[] { "CreatedBy_Id" });
            DropIndex("dbo.CourseSemesters", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Recommendations", new[] { "CreatedBy_Id" });
            DropIndex("dbo.StudentPreferences", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Courses", new[] { "CreatedBy_Id" });
            DropColumn("dbo.Semesters", "CreatedBy_Id");
            DropColumn("dbo.Semesters", "CreatedAt");
            DropColumn("dbo.CourseSemesters", "CreatedBy_Id");
            DropColumn("dbo.CourseSemesters", "CreatedAt");
            DropColumn("dbo.Recommendations", "CreatedBy_Id");
            DropColumn("dbo.Recommendations", "CreatedAt");
            DropColumn("dbo.StudentPreferences", "CreatedBy_Id");
            DropColumn("dbo.StudentPreferences", "CreatedAt");
            DropColumn("dbo.Courses", "CreatedBy_Id");
            DropColumn("dbo.Courses", "CreatedAt");
        }
    }
}
