namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recordsrequiredforeignkey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RecommendationRecords", "Recommendation_ID", "dbo.Recommendations");
            DropForeignKey("dbo.RecommendationRecords", "CourseSemester_ID", "dbo.CourseSemesters");
            DropForeignKey("dbo.RecommendationRecords", "StudentPreference_ID", "dbo.StudentPreferences");
            DropIndex("dbo.RecommendationRecords", new[] { "CourseSemester_ID" });
            DropIndex("dbo.RecommendationRecords", new[] { "StudentPreference_ID" });
            DropIndex("dbo.RecommendationRecords", new[] { "Recommendation_ID" });
            AlterColumn("dbo.RecommendationRecords", "CourseSemester_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.RecommendationRecords", "StudentPreference_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.RecommendationRecords", "Recommendation_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.RecommendationRecords", "Recommendation_ID");
            CreateIndex("dbo.RecommendationRecords", "StudentPreference_ID");
            CreateIndex("dbo.RecommendationRecords", "CourseSemester_ID");
            AddForeignKey("dbo.RecommendationRecords", "Recommendation_ID", "dbo.Recommendations", "ID", cascadeDelete: true);
            AddForeignKey("dbo.RecommendationRecords", "CourseSemester_ID", "dbo.CourseSemesters", "ID", cascadeDelete: true);
            AddForeignKey("dbo.RecommendationRecords", "StudentPreference_ID", "dbo.StudentPreferences", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecommendationRecords", "StudentPreference_ID", "dbo.StudentPreferences");
            DropForeignKey("dbo.RecommendationRecords", "CourseSemester_ID", "dbo.CourseSemesters");
            DropForeignKey("dbo.RecommendationRecords", "Recommendation_ID", "dbo.Recommendations");
            DropIndex("dbo.RecommendationRecords", new[] { "CourseSemester_ID" });
            DropIndex("dbo.RecommendationRecords", new[] { "StudentPreference_ID" });
            DropIndex("dbo.RecommendationRecords", new[] { "Recommendation_ID" });
            AlterColumn("dbo.RecommendationRecords", "Recommendation_ID", c => c.Int());
            AlterColumn("dbo.RecommendationRecords", "StudentPreference_ID", c => c.Int());
            AlterColumn("dbo.RecommendationRecords", "CourseSemester_ID", c => c.Int());
            CreateIndex("dbo.RecommendationRecords", "Recommendation_ID");
            CreateIndex("dbo.RecommendationRecords", "StudentPreference_ID");
            CreateIndex("dbo.RecommendationRecords", "CourseSemester_ID");
            AddForeignKey("dbo.RecommendationRecords", "StudentPreference_ID", "dbo.StudentPreferences", "ID");
            AddForeignKey("dbo.RecommendationRecords", "CourseSemester_ID", "dbo.CourseSemesters", "ID");
            AddForeignKey("dbo.RecommendationRecords", "Recommendation_ID", "dbo.Recommendations", "ID");
        }
    }
}
