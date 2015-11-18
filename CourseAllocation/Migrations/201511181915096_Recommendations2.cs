namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Recommendations2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CourseSemesters", "Recommendation_ID", "dbo.Recommendations");
            DropForeignKey("dbo.StudentPreferences", "Recommendation_ID", "dbo.Recommendations");
            DropIndex("dbo.StudentPreferences", new[] { "Recommendation_ID" });
            DropIndex("dbo.CourseSemesters", new[] { "Recommendation_ID" });
            CreateTable(
                "dbo.CourseSemesterRecommendations",
                c => new
                    {
                        CourseSemester_ID = c.Int(nullable: false),
                        Recommendation_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseSemester_ID, t.Recommendation_ID })
                .ForeignKey("dbo.CourseSemesters", t => t.CourseSemester_ID, cascadeDelete: true)
                .ForeignKey("dbo.Recommendations", t => t.Recommendation_ID, cascadeDelete: true)
                .Index(t => t.CourseSemester_ID)
                .Index(t => t.Recommendation_ID);
            
            CreateTable(
                "dbo.RecommendationStudentPreferences",
                c => new
                    {
                        Recommendation_ID = c.Int(nullable: false),
                        StudentPreference_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Recommendation_ID, t.StudentPreference_ID })
                .ForeignKey("dbo.Recommendations", t => t.Recommendation_ID, cascadeDelete: true)
                .ForeignKey("dbo.StudentPreferences", t => t.StudentPreference_ID, cascadeDelete: true)
                .Index(t => t.Recommendation_ID)
                .Index(t => t.StudentPreference_ID);
            
            DropColumn("dbo.StudentPreferences", "Recommendation_ID");
            DropColumn("dbo.CourseSemesters", "Recommendation_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CourseSemesters", "Recommendation_ID", c => c.Int());
            AddColumn("dbo.StudentPreferences", "Recommendation_ID", c => c.Int());
            DropForeignKey("dbo.RecommendationStudentPreferences", "StudentPreference_ID", "dbo.StudentPreferences");
            DropForeignKey("dbo.RecommendationStudentPreferences", "Recommendation_ID", "dbo.Recommendations");
            DropForeignKey("dbo.CourseSemesterRecommendations", "Recommendation_ID", "dbo.Recommendations");
            DropForeignKey("dbo.CourseSemesterRecommendations", "CourseSemester_ID", "dbo.CourseSemesters");
            DropIndex("dbo.RecommendationStudentPreferences", new[] { "StudentPreference_ID" });
            DropIndex("dbo.RecommendationStudentPreferences", new[] { "Recommendation_ID" });
            DropIndex("dbo.CourseSemesterRecommendations", new[] { "Recommendation_ID" });
            DropIndex("dbo.CourseSemesterRecommendations", new[] { "CourseSemester_ID" });
            DropTable("dbo.RecommendationStudentPreferences");
            DropTable("dbo.CourseSemesterRecommendations");
            CreateIndex("dbo.CourseSemesters", "Recommendation_ID");
            CreateIndex("dbo.StudentPreferences", "Recommendation_ID");
            AddForeignKey("dbo.StudentPreferences", "Recommendation_ID", "dbo.Recommendations", "ID");
            AddForeignKey("dbo.CourseSemesters", "Recommendation_ID", "dbo.Recommendations", "ID");
        }
    }
}
