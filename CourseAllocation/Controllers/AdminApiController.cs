using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CourseAllocation.Models;
using System.Data.Entity;

namespace CourseAllocation.Controllers
{
    public class AdminApiController : ApiController
    {

        [HttpGet]
        public IEnumerable<Course> Courses()
        {
            IEnumerable<Course> courses;
            using (var ctx = new ApplicationDbContext())
            {
                courses = ctx.Courses.Include(m => m.Prerequisites).ToList();
            }

            return courses;
        }

        [HttpGet]
        public IEnumerable<Semester> Semesters()
        {
            IEnumerable<Semester> semesters;
            using (var ctx = new ApplicationDbContext())
            {
                semesters = ctx.Semesters.ToList();
            }

            return semesters;
        }
    }
}
