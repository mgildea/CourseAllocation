namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Recommendations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CourseSemesters", "Recommendation_ID", "dbo.Recommendations");
            DropForeignKey("dbo.StudentPreferences", "Recommendation_ID", "dbo.Recommendations");
            DropIndex("dbo.StudentPreferences", new[] { "Recommendation_ID" });
            DropIndex("dbo.CourseSemesters", new[] { "Recommendation_ID" });
            DropColumn("dbo.StudentPreferences", "Recommendation_ID");
            DropColumn("dbo.CourseSemesters", "Recommendation_ID");
            DropTable("dbo.Recommendations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Recommendations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.CourseSemesters", "Recommendation_ID", c => c.Int());
            AddColumn("dbo.StudentPreferences", "Recommendation_ID", c => c.Int());
            CreateIndex("dbo.CourseSemesters", "Recommendation_ID");
            CreateIndex("dbo.StudentPreferences", "Recommendation_ID");
            AddForeignKey("dbo.StudentPreferences", "Recommendation_ID", "dbo.Recommendations", "ID");
            AddForeignKey("dbo.CourseSemesters", "Recommendation_ID", "dbo.Recommendations", "ID");
        }
    }
}
