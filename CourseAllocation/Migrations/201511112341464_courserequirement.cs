namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class courserequirement : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Requirements",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        RequiredUnits = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Number = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Units = c.Int(nullable: false),
                        IsFoundational = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Prerequisites",
                c => new
                    {
                        Course_ID = c.Int(nullable: false),
                        Preqrequisite_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Course_ID, t.Preqrequisite_ID })
                .ForeignKey("dbo.Courses", t => t.Course_ID)
                .ForeignKey("dbo.Courses", t => t.Preqrequisite_ID)
                .Index(t => t.Course_ID)
                .Index(t => t.Preqrequisite_ID);
            
            CreateTable(
                "dbo.RequirementCourses",
                c => new
                    {
                        Requirement_ID = c.Int(nullable: false),
                        Course_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Requirement_ID, t.Course_ID })
                .ForeignKey("dbo.Requirements", t => t.Requirement_ID, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.Course_ID, cascadeDelete: true)
                .Index(t => t.Requirement_ID)
                .Index(t => t.Course_ID);
            
            CreateTable(
                "dbo.RequirementRequirements",
                c => new
                    {
                        Requirement_ID = c.Int(nullable: false),
                        HasRequirement_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Requirement_ID, t.HasRequirement_ID })
                .ForeignKey("dbo.Requirements", t => t.Requirement_ID)
                .ForeignKey("dbo.Requirements", t => t.HasRequirement_ID)
                .Index(t => t.Requirement_ID)
                .Index(t => t.HasRequirement_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequirementRequirements", "HasRequirement_ID", "dbo.Requirements");
            DropForeignKey("dbo.RequirementRequirements", "Requirement_ID", "dbo.Requirements");
            DropForeignKey("dbo.RequirementCourses", "Course_ID", "dbo.Courses");
            DropForeignKey("dbo.RequirementCourses", "Requirement_ID", "dbo.Requirements");
            DropForeignKey("dbo.Prerequisites", "Preqrequisite_ID", "dbo.Courses");
            DropForeignKey("dbo.Prerequisites", "Course_ID", "dbo.Courses");
            DropIndex("dbo.RequirementRequirements", new[] { "HasRequirement_ID" });
            DropIndex("dbo.RequirementRequirements", new[] { "Requirement_ID" });
            DropIndex("dbo.RequirementCourses", new[] { "Course_ID" });
            DropIndex("dbo.RequirementCourses", new[] { "Requirement_ID" });
            DropIndex("dbo.Prerequisites", new[] { "Preqrequisite_ID" });
            DropIndex("dbo.Prerequisites", new[] { "Course_ID" });
            DropTable("dbo.RequirementRequirements");
            DropTable("dbo.RequirementCourses");
            DropTable("dbo.Prerequisites");
            DropTable("dbo.Courses");
            DropTable("dbo.Requirements");
        }
    }
}
