namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecommendationRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecommendationRecords",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CourseSemester_ID = c.Int(),
                        StudentPreference_ID = c.Int(),
                        Recommendation_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CourseSemesters", t => t.CourseSemester_ID)
                .ForeignKey("dbo.StudentPreferences", t => t.StudentPreference_ID)
                .ForeignKey("dbo.Recommendations", t => t.Recommendation_ID)
                .Index(t => t.CourseSemester_ID)
                .Index(t => t.StudentPreference_ID)
                .Index(t => t.Recommendation_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecommendationRecords", "Recommendation_ID", "dbo.Recommendations");
            DropForeignKey("dbo.RecommendationRecords", "StudentPreference_ID", "dbo.StudentPreferences");
            DropForeignKey("dbo.RecommendationRecords", "CourseSemester_ID", "dbo.CourseSemesters");
            DropIndex("dbo.RecommendationRecords", new[] { "Recommendation_ID" });
            DropIndex("dbo.RecommendationRecords", new[] { "StudentPreference_ID" });
            DropIndex("dbo.RecommendationRecords", new[] { "CourseSemester_ID" });
            DropTable("dbo.RecommendationRecords");
        }
    }
}
