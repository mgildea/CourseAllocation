namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Student : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        GaTechId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.GaTechId);

            Sql(@"Insert Into Students (GaTechId) SELECT Distinct
      [GaTechId]

  FROM[dbo].[StudentPreferences]

  union

  Select Distinct GaTechId

  From [dbo].CompletedCourses;");
            
            AlterColumn("dbo.StudentPreferences", "GaTechId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.CompletedCourses", "GaTechId");
            CreateIndex("dbo.StudentPreferences", "GaTechId");
            AddForeignKey("dbo.StudentPreferences", "GaTechId", "dbo.Students", "GaTechId", cascadeDelete: true);
            AddForeignKey("dbo.CompletedCourses", "GaTechId", "dbo.Students", "GaTechId", cascadeDelete: true);



        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedCourses", "GaTechId", "dbo.Students");
            DropForeignKey("dbo.StudentPreferences", "GaTechId", "dbo.Students");
            DropIndex("dbo.StudentPreferences", new[] { "GaTechId" });
            DropIndex("dbo.CompletedCourses", new[] { "GaTechId" });
            AlterColumn("dbo.StudentPreferences", "GaTechId", c => c.String(nullable: false));
            DropTable("dbo.Students");
        }
    }
}
