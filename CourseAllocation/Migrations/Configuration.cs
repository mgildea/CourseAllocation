namespace CourseAllocation.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using CourseAllocation.Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<CourseAllocation.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }


        private void SetCPRConcentration(ApplicationDbContext context)
        {
            context.Concentrations.AddOrUpdate(m => m.Name,

                new Concentration
                {
                    Name = "Computational Perception and Robotics",
                    RequiredUnits = 30,
                    Requirements = new List<Requirement>
                    {
                        new Requirement
                        {
                            Name = "Computational Perception and Robotics - Core Courses",
                            RequiredUnits = 6,
                            Requirements = new List<Requirement>
                            {
                                new Requirement
                                {
                                    Name = "Computational Perception and Robotics - Core Courses - Algorithms",
                                    RequiredUnits = 3,
                                    Courses = context.Courses.Where(m => (new string[] { "CS 6505", "CS 6520", "CS 6550", "CS 7520", "CS 7530", "CSE 6140" }).Contains(m.Number)).ToList(),
                                },
                                new Requirement
                                {
                                    Name = "Computational Perception and Robotics - Core Courses - Other",
                                    RequiredUnits = 3,
                                    Courses = context.Courses.Where(m => (new string[] { "CS 6601", "CS 7641" }).Contains(m.Number)).ToList(),
                                }
                            }
                        },

                        new Requirement
                        {
                            Name = "Computational Perception and Robotics - Electives",
                            RequiredUnits = 9,
                            Requirements = new List<Requirement>
                            {
                                new Requirement
                                {
                                    Name = "Computational Perception and Robotics - Electives - Perception",
                                    RequiredUnits = 3,
                                    Courses = context.Courses.Where(m => (new string[] { "CS 6475", "CS 7495" }).Contains(m.Number)).ToList()
                                },
                                new Requirement
                                {
                                    Name = "Computational Perception and Robotics - Electives - Robotics",
                                    RequiredUnits = 3,
                                    Courses = context.Courses.Where(m => (new string[] { "CS 8803 - 001" }).Contains(m.Number)).ToList(),
                                }
                            }
                        }
                    }

                });
        }


        private void SetCSoncentration(ApplicationDbContext context)
        {
            context.Concentrations.AddOrUpdate(m => m.Name,

                new Concentration
                {
                    Name = "Computing Systems",
                    RequiredUnits = 30,
                    Requirements = new List<Requirement>
                    {
                        new Requirement
                        {
                            Name = "Computing Systems - Core Courses",
                            RequiredUnits = 9,
                            Requirements = new List<Requirement>
                            {
                                new Requirement
                                {
                                    Name = "Computing Systems - Core Courses - Computability",
                                    RequiredUnits = 3,
                                    Courses = context.Courses.Where(m => (new string[] { "CS 6505" }).Contains(m.Number)).ToList(),
                                },
                                new Requirement
                                {
                                    Name = "Computing Systems - Core Courses - Other",
                                    RequiredUnits = 6,
                                    Courses = context.Courses.Where(m => (new string[] { "CS 6210", "CS 6241", "CS 6250", "CS 6290", "CS 6300", "CS 6400" }).Contains(m.Number)).ToList(),
                                }
                            }
                        },

                        new Requirement
                        {
                            Name = "Computing Systems - Electives",
                            RequiredUnits = 9,
                            Courses = context.Courses.Where(m => (new string[] { "CS 6035", "CS 6310", "CS 6340" }).Contains(m.Number)).ToList()

                        }
                    }

                });
        }


        private void SetIIConcentration(ApplicationDbContext context)
        {
            context.Concentrations.AddOrUpdate(m => m.Name,

                new Concentration
                {
                    Name = "Interactive Intelligence",
                    RequiredUnits = 30,
                    Requirements = new List<Requirement>
                    {
                        new Requirement
                        {
                            Name = "Interactive Intelligence - Core Courses",
                            RequiredUnits = 9,
                            Requirements = new List<Requirement>
                            {
                                new Requirement
                                {
                                    Name = "Interactive Intelligence - Core Courses - Algorithms",
                                    RequiredUnits = 3,
                                    Courses = context.Courses.Where(m => (new string[] { "CS 6505", "CS 6300" }).Contains(m.Number)).ToList(),
                                },
                                new Requirement
                                {
                                    Name = "Interactive Intelligence - Core Courses - Other",
                                    RequiredUnits = 6,
                                    Courses = context.Courses.Where(m => (new string[] { "CS 7637", "CS 7641" }).Contains(m.Number)).ToList(),
                                }
                            }
                        },

                        new Requirement
                        {
                            Name = "Interactive Intelligence - Electives",
                            RequiredUnits = 6,
                            Requirements = new List<Requirement>
                            {
                                new Requirement
                                {
                                    Name = "Interactive Intelligence - Electives - Interaction",
                                    RequiredUnits = 3,
                                    Courses = context.Courses.Where(m => (new string[] { "CS 6440", "CS 6460" }).Contains(m.Number)).ToList()
                                },
                                new Requirement
                                {
                                    Name = "Interactive Intelligence - Electives - Cognition",
                                    RequiredUnits = 0,
                                    Courses = context.Courses.Where(m => (new string[] { "" }).Contains(m.Number)).ToList(),
                                }
                            }
                        }
                    }

                });
        }

        protected override void Seed(CourseAllocation.Models.ApplicationDbContext context)
        {

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            var user = context.Users.FirstOrDefault(m => m.UserName == "admin");

            if(user == null)
            {
                user = new ApplicationUser() { UserName = "admin", Email = "" };
                context.Users.Add(user);
            }

            context.UserName = user.UserName;


       

            context.Courses.AddOrUpdate(
                m => m.Number,
                new Course { Number = "CS 6476", Name = "Computer Vision", Units = 3, IsFoundational = true },
                new Course { Number = "CS 6035", Name = "Introduction to Information Security", Units = 3 },
               
                new Course { Number = "CSE 6220", Name = "Intro to High - Performance Computing", Units = 3, IsFoundational = true },
                new Course { Number = "CS 6250", Name = "Computer Networks", Units = 3, IsFoundational = true },
                new Course { Number = "CS 6290", Name = "High Performance Computer Architecture", Units = 3, IsFoundational = true },
                new Course { Number = "CS 6300", Name = "Software Development Process", Units = 3, IsFoundational = true },
                new Course { Number = "CS 6310", Name = "Software Architecture and Design", Units = 3, IsFoundational = true },
                new Course { Number = "CS 6460", Name = "Educational Technology", Units = 3 },
                new Course { Number = "CS 6475", Name = "Computational Photograph", Units = 3 },
                new Course { Number = "CS 6505", Name = "Computability, Complexity and Algorithms", Units = 3, IsFoundational = true },
                new Course { Number = "CS 7637", Name = "Knowledge - Based Artificial Intelligence: Cognitive Systems", Units = 3, IsFoundational = true },
                new Course { Number = "CS 7641", Name = "Machine Learning", Units = 3, IsFoundational = true },
                 new Course { Number = "CS 8803 - 002", Name = "Introduction to Operating Systems", Units = 3, IsFoundational = true },
                new Course { Number = "CS 8803 - 003", Name = "Special Topics: Reinforcement Learning", Units = 3 }
                 
                );
            //context.courses.single(m => m.number == "cs 7641").prerequisitefor = context.courses.where(m => m.number == "cs 7646").tolist();
            //context.courses.single(m => m.number == "cs 8803 - 002").prerequisitefor = context.courses.where(m => m.number == "cs 6210").tolist();
            //context.courses.single(m => m.number == "cs 7637").prerequisitefor = context.courses.where(m => m.number == "cs 8803 - 001").tolist();
            //context.Courses.Single(m => m.Number == "CS 6300").PrerequisiteFor = context.Courses.Where(m => m.Number == "CS 6440").ToList();
            context.SaveChanges();

            context.Courses.AddOrUpdate(
               m => m.Number,
new Course { Number = "CS 6210", Name = "Advanced Operating Systems", Units = 3, IsFoundational = true, Prerequisites = context.Courses.Where(m => (new string[] { "CS 8803 - 002" }).Contains(m.Number)).ToList() },
  new Course { Number = "CS 8803 - 001", Name = "Artificial Intelligence for Robotics", Units = 3, Prerequisites = context.Courses.Where(m => (new string[] { "CS 7637" }).Contains(m.Number)).ToList() },
               new Course { Number = "CS 7646", Name = "Machine Learning for Trading", Units = 3, Prerequisites = context.Courses.Where(m => (new string[] { "CS 7641" }).Contains(m.Number)).ToList() },
               new Course { Number = "CS 6440", Name = "Intro to Health Informatics", Units = 3, Prerequisites = context.Courses.Where(m => (new string[] { "CS 6300" }).Contains(m.Number)).ToList() }
               
                 );

            context.SaveChanges();

            SetCPRConcentration(context);
            SetCSoncentration(context);
            SetIIConcentration(context);

            for (int i = 2016; i < 2021; i++)
            {
                context.Semesters.AddOrUpdate(
                    m => new { m.Year, m.Type },
                    new Semester { Type = SemesterType.Spring, Year = i },
                    new Semester { Type = SemesterType.Summer, Year = i },
                    new Semester { Type = SemesterType.Fall, Year = i }
                    );
            }


          //  var allCourses = context.Courses.Include(m => m.Prerequisites).ToList();

            for (int i = 1; i <= 600; i++)
            {
                //var courses = allCourses.OrderBy(m => Guid.NewGuid()).Take(12).ToList();

             

                var courses  = context.Courses.Include(m => m.Prerequisites).OrderBy(m => Guid.NewGuid()).Take(12).ToList();

                List<Course> preference = new List<Course>();

                foreach (var course in courses)
                {
                    if (!preference.Select(m => m.ID).Contains(course.ID))
                        preference.Add(course);

   
                    foreach (var prereq in course.Prerequisites)
                    {
                        if (!preference.Select(m => m.ID).Contains(prereq.ID))
                            preference.Add(prereq);
                        
                    }
                }

     
                context.Students.AddOrUpdate(
                    m => m.GaTechId,
                    new CourseAllocation.Models.Student { GaTechId = "Student" + i }
                    );

                context.StudentPreferences.AddOrUpdate(
                    m => new { m.GaTechId, m.IsActive },
                    new StudentPreference { GaTechId = "Student" + i, IsActive = true, Courses = preference }
                    );

            }

        }
    }
}
