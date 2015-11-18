namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Recommendations1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Courses", "Recommendation_ID", "dbo.Recommendations");
            DropForeignKey("dbo.Semesters", "Recommendation_ID", "dbo.Recommendations");
            DropIndex("dbo.Courses", new[] { "Recommendation_ID" });
            DropIndex("dbo.Semesters", new[] { "Recommendation_ID" });
            AddColumn("dbo.CourseSemesters", "Recommendation_ID", c => c.Int());
            CreateIndex("dbo.CourseSemesters", "Recommendation_ID");
            AddForeignKey("dbo.CourseSemesters", "Recommendation_ID", "dbo.Recommendations", "ID");
            DropColumn("dbo.Courses", "Recommendation_ID");
            DropColumn("dbo.Semesters", "Recommendation_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Semesters", "Recommendation_ID", c => c.Int());
            AddColumn("dbo.Courses", "Recommendation_ID", c => c.Int());
            DropForeignKey("dbo.CourseSemesters", "Recommendation_ID", "dbo.Recommendations");
            DropIndex("dbo.CourseSemesters", new[] { "Recommendation_ID" });
            DropColumn("dbo.CourseSemesters", "Recommendation_ID");
            CreateIndex("dbo.Semesters", "Recommendation_ID");
            CreateIndex("dbo.Courses", "Recommendation_ID");
            AddForeignKey("dbo.Semesters", "Recommendation_ID", "dbo.Recommendations", "ID");
            AddForeignKey("dbo.Courses", "Recommendation_ID", "dbo.Recommendations", "ID");
        }
    }
}
