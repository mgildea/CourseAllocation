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
            //Models.Course courses = new Models.Course();
            //int[] students = new int[600];

            //int[] sems = new int[600];
            //try
            //{
            //    GRBEnv env = new GRBEnv("mip1.log");
            //    GRBModel model = new GRBModel(env);
            //    model.Set(GRB.StringAttr.ModelName, "Course Optimizer");

            //    GRBVar[,,] CourseAllocation = new GRBVar[students.Length, courses.Length, sems.Length];
            //    GRBVar X = model.AddVar(0, GRB.INFINITY, 0, GRB.INTEGER, "X");

            //    for (int i = 0; i < students.Length; i++)
            //    {
            //        for (int j = 0; j < courses.Length; j++)
            //        {
            //            for (int k = 0; k < sems.Length; k++)
            //            {
            //                //if (students.desiresCourse(i, j) && sems.courseOffered(j, k))
            //                CourseAllocation[i, j, k] = model.AddVar(0, 1, 1, GRB.BINARY, "students." + (i + 1).ToString() + "_Course." + (j + 1).ToString() + "_Semester." + (k + 1).ToString());
            //                //else
            //                //    StdntCrsSem[i][j][k] = model.addVar(0, 0, 1, GRB.BINARY, "students." + String.valueOf(i + 1) + "_Course." + String.valueOf(j + 1) + "_Semester." + String.valueOf(k + 1));
            //            }
            //        }
            //    }

            //    model.Update();

            //    GRBLinExpr constStudentDesiredCourses; // = new GRBLinExpr();
            //    GRBLinExpr constMaxPerSem; // = new GRBLinExpr();
            //    GRBLinExpr constMinStudent; // = new GRBLinExpr();
            //    GRBLinExpr constpreReq;
            //    GRBLinExpr constpostReq;

            //    //MUST TAKE DESIRED COURSE ONLY ONCE
            //    for (int i = 0; i < students.Length; i++)
            //    {
            //        for (int j = 0; j < courses.Length; j++)
            //        {
            //            if (students.desiresCourse(i, j))
            //            {
            //                constStudentDesiredCourses = new GRBLinExpr();
            //                for (int k = 0; k < sems.Length; k++)
            //                    constStudentDesiredCourses.AddTerm(1, CourseAllocation[i, j, k]);

            //                String sStudentDesiredCourses = "DesiredCourse." + j + 1 + "_Student." + i + 1;
            //                model.AddConstr(constStudentDesiredCourses, GRB.EQUAL, 1, sStudentDesiredCourses);
            //            }
            //        }

            //        //MAX COURSES PER SEMESTER 
            //        for (int k = 0; k < sems.Length; k++)
            //        {
            //            constMaxPerSem = new GRBLinExpr();
            //            for (int j = 0; j < courses.Length; j++)
            //                constMaxPerSem.AddTerm(1, CourseAllocation[i, j, k]);

            //            String sCourseSem = "maxCourseStudent." + i + 1 + "_Semester." + k + 1;
            //            model.AddConstr(constMaxPerSem, GRB.LESS_EQUAL, MAX_COURSES_PER_SEMESTER, sCourseSem);
            //        }

            //        //PREREQUISITES
            //        for (int j = 0; j < courses.Length; j++)
            //        {
            //            if (students.desiresCourse(i, j))
            //            {
            //                int prereq = courses.prerequisite(j + 1) - 1;
            //                if (prereq > 0)
            //                {
            //                    constpostReq = new GRBLinExpr();
            //                    constpreReq = new GRBLinExpr();
            //                    for (int kreq = 0; kreq < sems.Length; kreq++)
            //                    {
            //                        if (sems.courseOffered(prereq, kreq++))
            //                            constpostReq.AddTerm(kreq + 1, CourseAllocation[i, prereq, kreq]);
            //                    }
            //                    for (int k = 0; k < sems.Length; k++)
            //                    {
            //                        if (sems.courseOffered(j, k))
            //                            constpreReq.AddTerm(1, CourseAllocation[i, j, k]);
            //                    }
            //                    model.AddConstr(constpreReq, GRB.LESS_EQUAL, constpostReq, "PreRequisiteCourse." + j + 1 + "_Student." + i + 1);
            //                }
            //            }
            //        }
            //    }
            //}
            //catch
            //{ }

            return null;
        }
    }
}
