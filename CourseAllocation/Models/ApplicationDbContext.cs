using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using CourseAllocation.Models;

namespace CourseAllocation.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("CourseAllocation", throwIfV1Schema: false)
        {
           
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Requirement> Requirements { get; set; }
        public DbSet<Concentration> Concentrations { get; set; }
        

       public DbSet<StudentPreference> StudentPreferences { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Semester> Semesters { get; set; }
        public DbSet<CourseSemester> CourseSemesters { get; set; }

        



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<Course>()
                .HasMany(m => m.Prerequisites)
                .WithMany(m => m.PrerequisiteFor)
                .Map(m =>
                {
                    m.ToTable("Prerequisites");
                    m.MapRightKey("Preqrequisite_ID");
                });



            modelBuilder.Entity<Requirement>()
                .HasMany(m => m.Requirements)
                .WithMany(m => m.RequirementsFor)
                .Map(m =>
               {
                   m.MapRightKey("HasRequirement_ID");
               });

            modelBuilder.Entity<Requirement>()
                .HasMany(m => m.Courses)
                .WithMany(m => m.Requirements)
                .Map(m =>
                {
                    m.ToTable("RequirementCourses");
                });


            //modelBuilder.Entity<Concentration>()
            //    .HasMany(m => m.RequiredCourses)
                //.WithMany(m => m.Concentrations);
        }
    }
}