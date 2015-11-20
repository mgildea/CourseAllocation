namespace CourseAllocation.Migrations
{
    using Models;
    using System;
    using System.Data.Entity.Migrations;

    public partial class ilogrequired : DbMigration
    {
        public override void Up()
        {
            //using (var ctx = new ApplicationDbContext())
            //{
            //    ctx.Users.Add(new ApplicationUser() { UserName = "admin" });
            //    ctx.SaveChanges();
            //}
            var GUID = Guid.NewGuid();


            Sql("Insert Into AspNetUsers (Id, UserName, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount) values ('" + GUID + "', 'admin', 0, 0, 0, 0, 0);");
             Sql("Update Semesters Set CreatedBy_Id = '" + GUID + "' Where CreatedBy_Id is NULL;");
            Sql("Update Courses Set CreatedBy_Id = '" + GUID + "' Where CreatedBy_Id is NULL;");
            Sql("Update CourseSemesters Set CreatedBy_Id = '" + GUID + "' Where CreatedBy_Id is NULL;");
            Sql("Update StudentPreferences Set CreatedBy_Id = '" + GUID + "' Where CreatedBy_Id is NULL;");
            Sql("Update Recommendations Set CreatedBy_Id = '" + GUID + "' Where CreatedBy_Id is NULL;");


            DropForeignKey("dbo.Courses", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.StudentPreferences", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Recommendations", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseSemesters", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Semesters", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Courses", new[] { "CreatedBy_Id" });
            DropIndex("dbo.StudentPreferences", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Recommendations", new[] { "CreatedBy_Id" });
            DropIndex("dbo.CourseSemesters", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Semesters", new[] { "CreatedBy_Id" });
            AlterColumn("dbo.Courses", "CreatedBy_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.StudentPreferences", "CreatedBy_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Recommendations", "CreatedBy_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.CourseSemesters", "CreatedBy_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Semesters", "CreatedBy_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Courses", "CreatedBy_Id");
            CreateIndex("dbo.StudentPreferences", "CreatedBy_Id");
            CreateIndex("dbo.Recommendations", "CreatedBy_Id");
            CreateIndex("dbo.CourseSemesters", "CreatedBy_Id");
            CreateIndex("dbo.Semesters", "CreatedBy_Id");
            AddForeignKey("dbo.Courses", "CreatedBy_Id", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            AddForeignKey("dbo.StudentPreferences", "CreatedBy_Id", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Recommendations", "CreatedBy_Id", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            AddForeignKey("dbo.CourseSemesters", "CreatedBy_Id", "dbo.AspNetUsers", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Semesters", "CreatedBy_Id", "dbo.AspNetUsers", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Semesters", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseSemesters", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Recommendations", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.StudentPreferences", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Courses", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Semesters", new[] { "CreatedBy_Id" });
            DropIndex("dbo.CourseSemesters", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Recommendations", new[] { "CreatedBy_Id" });
            DropIndex("dbo.StudentPreferences", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Courses", new[] { "CreatedBy_Id" });
            AlterColumn("dbo.Semesters", "CreatedBy_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.CourseSemesters", "CreatedBy_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Recommendations", "CreatedBy_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.StudentPreferences", "CreatedBy_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Courses", "CreatedBy_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Semesters", "CreatedBy_Id");
            CreateIndex("dbo.CourseSemesters", "CreatedBy_Id");
            CreateIndex("dbo.Recommendations", "CreatedBy_Id");
            CreateIndex("dbo.StudentPreferences", "CreatedBy_Id");
            CreateIndex("dbo.Courses", "CreatedBy_Id");
            AddForeignKey("dbo.Semesters", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.CourseSemesters", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Recommendations", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.StudentPreferences", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Courses", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
