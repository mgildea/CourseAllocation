using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CourseAllocation.Models;
using System.Data.Entity;
using CourseAllocation.ViewModels;

namespace CourseAllocation.Controllers
{
    public class AdminApiController : ApiController
    {

        [HttpPost]
        public CourseSemesterViewModel CourseSemester(CourseSemester courseSemester)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if(courseSemester == null || courseSemester.Course == null || courseSemester.Semester == null ||
                    ctx.CourseSemesters.Where(m => m.Course.ID == courseSemester.Course.ID && m.Semester.Type == courseSemester.Semester.Type && m.Semester.Year == courseSemester.Semester.Year).Any())
                {
                    //record already exists or data incomplete
                    return null;
                }

                courseSemester.Course = ctx.Courses.Find(courseSemester.Course.ID);
                courseSemester.Semester = ctx.Semesters.Find(courseSemester.Semester.Type, courseSemester.Semester.Year);

                ctx.CourseSemesters.Add(courseSemester);
                ctx.SaveChanges();

                return new CourseSemesterViewModel(courseSemester);
            }

          

   
        }

        [HttpGet]
        public IEnumerable<CourseSemesterViewModel> CourseSemesters()
        {
            using (var ctx = new ApplicationDbContext())
            {
                return ctx.CourseSemesters.Include(m => m.Course).Include(m => m.Semester).ToList().Select(m => new CourseSemesterViewModel(m));
            }
        }


        [HttpGet]
        public IEnumerable<CourseViewModel> Courses()
        {

            using (var ctx = new ApplicationDbContext())
            {
                return ctx.Courses.Include(m => m.Prerequisites).ToList().Select(m => new CourseViewModel(m));
            }
        }


        [HttpPost]
        public CourseViewModel Course(Course course)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (course.Number == null || course.Name == null || ctx.Courses.Where(m => m.Number == course.Number).Any())
                {
                    //record already exists or is invalid
                    return null;
                }

                var prereqIds = course.Prerequisites.Select(n => n.ID);
                course.Prerequisites = ctx.Courses.Where(m => prereqIds.Contains(m.ID)).ToList();

                ctx.Courses.Add(course);
                ctx.SaveChanges();

                return new CourseViewModel(course);
            }

            
        }

        [HttpPost]
        public IEnumerable<SemesterViewModel> Year()
        {
            using (var ctx = new ApplicationDbContext())
            {
                int newYear = ctx.Semesters.Max(m => m.Year) + 1;

                List<Semester> newSemesters = new List<Semester>();

                foreach (SemesterType type in Enum.GetValues(typeof(SemesterType)))
                {
                    newSemesters.Add(new Semester() { Type = type, Year = newYear });
                }

                ctx.Semesters.AddRange(newSemesters);

                ctx.SaveChanges();

                return newSemesters.Select(m => new SemesterViewModel(m));
            }
            
        }

        [HttpGet]
        public IEnumerable<SemesterViewModel> Semesters()
        {
            using (var ctx = new ApplicationDbContext())
            {
                return ctx.Semesters.ToList().Select(m => new SemesterViewModel(m));
            }
        }
    }
}
