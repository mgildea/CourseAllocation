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
        public string UserName { get; set; }
       // private string _userName = string.Empty;

        public ApplicationDbContext()
            : base("CourseAllocation", throwIfV1Schema: false)
        {
        
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }



        public override int SaveChanges()
        {
            var username = (HttpContext.Current != null && HttpContext.Current.User != null) ? HttpContext.Current.User.Identity.Name : UserName;
            
            var user =  this.Users.Single(m => m.UserName == username);

            foreach(var entry in this.ChangeTracker.Entries().Where(m => m.Entity is ILog && (m.State == EntityState.Added || m.State == EntityState.Modified)))
            {

                if(entry.State == EntityState.Added || ( UserName != string.Empty && entry.State == EntityState.Modified))
                {
                    ((ILog)entry.Entity).CreatedAt = DateTime.UtcNow;
                    ((ILog)entry.Entity).CreatedBy_Id = user.Id;
                }
            }

            return base.SaveChanges();
        }


        public DbSet<Requirement> Requirements { get; set; }
        public DbSet<Concentration> Concentrations { get; set; }
        

       public DbSet<StudentPreference> StudentPreferences { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Semester> Semesters { get; set; }
        public DbSet<CourseSemester> CourseSemesters { get; set; }

        public DbSet<CompletedCourse> CompletedCourses { get; set; }
        
        public DbSet<Recommendation> Recommendations { get; set; }


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