using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CourseAllocation.Models;
using Gurobi;

namespace CourseAllocation.Controllers
{
    public class OptimizeApiController : ApiController
    {
        [HttpGet]
        public object Optimize()
        {
            using (var dbConn = new ApplicationDbContext())
            {
                var set = dbConn.Semesters.Where(m => m.Year == 2016 ).ToList();
                foreach (var rec in dbConn.CourseSemesters.ToList())
                {
                  
                }

                //dbConn.Courses.First().Prerequis
            }

           
                return null;
        }
    }
}
