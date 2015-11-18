namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Recommendations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recommendations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Courses", "Recommendation_ID", c => c.Int());
            AddColumn("dbo.StudentPreferences", "Recommendation_ID", c => c.Int());
            AddColumn("dbo.Semesters", "Recommendation_ID", c => c.Int());
            CreateIndex("dbo.Courses", "Recommendation_ID");
            CreateIndex("dbo.StudentPreferences", "Recommendation_ID");
            CreateIndex("dbo.Semesters", "Recommendation_ID");
            AddForeignKey("dbo.Courses", "Recommendation_ID", "dbo.Recommendations", "ID");
            AddForeignKey("dbo.Semesters", "Recommendation_ID", "dbo.Recommendations", "ID");
            AddForeignKey("dbo.StudentPreferences", "Recommendation_ID", "dbo.Recommendations", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentPreferences", "Recommendation_ID", "dbo.Recommendations");
            DropForeignKey("dbo.Semesters", "Recommendation_ID", "dbo.Recommendations");
            DropForeignKey("dbo.Courses", "Recommendation_ID", "dbo.Recommendations");
            DropIndex("dbo.Semesters", new[] { "Recommendation_ID" });
            DropIndex("dbo.StudentPreferences", new[] { "Recommendation_ID" });
            DropIndex("dbo.Courses", new[] { "Recommendation_ID" });
            DropColumn("dbo.Semesters", "Recommendation_ID");
            DropColumn("dbo.StudentPreferences", "Recommendation_ID");
            DropColumn("dbo.Courses", "Recommendation_ID");
            DropTable("dbo.Recommendations");
        }
    }
}
