using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using CourseAllocation.Models;
using System.Collections.Generic;
using Gurobi;

namespace CourseAllocation.Controllers
{
    public class OptimizeApiController : ApiController
    {
        private StudentPreference[] students;
        private Course[] courses;
        private Semester[] sems;
        private CourseSemester[] crssems;
        private GRBLinExpr MAX_COURSES_PER_SEMESTER = new GRBLinExpr(2);

        [HttpGet]
        public object Optimize()
        {
            using (var dbConn = new ApplicationDbContext())
            {
                students = dbConn.StudentPreferences.Include(m => m.Courses).ToArray();
                crssems = dbConn.CourseSemesters.Where(m => m.IsActive == true).Include(m => m.Course).Include(m => m.Semester).ToArray();
                courses = crssems.Select(m => m.Course).Distinct().ToArray();
                sems = crssems.Select(m => m.Semester).Distinct().OrderBy(m => m.Type).OrderBy(m => m.Year).ToArray();
            }
            try
            {
                GRBEnv env = new GRBEnv("mip1.log");
                GRBModel model = new GRBModel(env);
                model.Set(GRB.StringAttr.ModelName, "Course Optimizer");

                GRBVar[,,] CourseAllocation = new GRBVar[students.Length, courses.Length, sems.Length];
                GRBVar X = model.AddVar(0, GRB.INFINITY, 0, GRB.INTEGER, "X");
                for (int i = 0; i < students.Length; i++)
                {
                    for (int j = 0; j < courses.Length; j++)
                    {
                        for (int k = 0; k < sems.Length; k++)
                        {
                            if (students[i].Courses.Contains(courses[j]) && (crssems.Select(m => m.Course).Contains(courses[j]) && crssems.Select(m => m.Semester).Contains(sems[k])))
                                CourseAllocation[i, j, k] = model.AddVar(0, 1, 1, GRB.BINARY, "students." + (i + 1).ToString() + "_Course." + (j + 1).ToString() + "_Semester." + (k + 1).ToString());
                            else
                                CourseAllocation[i, j, k] = model.AddVar(0, 0, 1, GRB.BINARY, "students." + (i + 1).ToString() + "_Course." + (j + 1).ToString() + "_Semester." + (k + 1).ToString());
                        }
                    }
                }

                model.Update();

                GRBLinExpr constStudentDesiredCourses; // = new GRBLinExpr();
                GRBLinExpr constMaxPerSem;
                GRBLinExpr constMinStudent = new GRBLinExpr();
                //GRBLinExpr constpreReq;
                //GRBLinExpr constpostReq;
                GRBLinExpr temp = new GRBLinExpr(2);

                //MUST TAKE DESIRED COURSE ONLY ONCE
                for (int i = 0; i < students.Length; i++)
                {
                    for (int j = 0; j < courses.Length; j++)
                    {
                        if (students[i].Courses.Contains(courses[j]))
                        {
                            constStudentDesiredCourses = new GRBLinExpr();
                            for (int k = 0; k < sems.Length; k++)
                                constStudentDesiredCourses.AddTerm(1, CourseAllocation[i, j, k]);

                            String sStudentDesiredCourses = "DesiredCourse." + j + 1 + "_Student." + i + 1;
                            model.AddConstr(constStudentDesiredCourses, GRB.EQUAL, 1, sStudentDesiredCourses);
                        }
                    }
                    //MAX COURSES PER SEMESTER 
                    for (int k = 0; k < sems.Length; k++)
                    {
                        constMaxPerSem = new GRBLinExpr();
                        for (int j = 0; j < courses.Length; j++)
                            constMaxPerSem.AddTerm(1, CourseAllocation[i, j, k]);

                        String sCourseSem = "maxCourseStudent." + i + 1 + "_Semester." + k + 1;
                        model.AddConstr(constMaxPerSem, GRB.LESS_EQUAL, MAX_COURSES_PER_SEMESTER, sCourseSem);
                    }

                    //PREREQUISITES
                    //for (int j = 0; j < courses.Length; j++)
                    //{
                    //    if (students[i].PreferedCourses.Select(m => m.ID).Contains(courses[j].ID))
                    //    {
                    //        int prereq = courses.prerequisite(j + 1) - 1;
                    //        if (prereq > 0)
                    //        {
                    //            constpostReq = new GRBLinExpr();
                    //            constpreReq = new GRBLinExpr();
                    //            for (int kreq = 0; kreq < sems.Length; kreq++)
                    //            {
                    //                if (sems.courseOffered(prereq, kreq++))
                    //                    constpostReq.AddTerm(kreq + 1, CourseAllocation[i, prereq, kreq]);
                    //            }
                    //            for (int k = 0; k < sems.Length; k++)
                    //            {
                    //                if (sems.courseOffered(j, k))
                    //                    constpreReq.AddTerm(1, CourseAllocation[i, j, k]);
                    //            }
                    //            model.AddConstr(constpreReq, GRB.LESS_EQUAL, constpostReq, "PreRequisiteCourse." + j + 1 + "_Student." + i + 1);
                    //        }
                    //    }
                    //}
                }

                //MINIMIZE STUDENTS
                for (int j = 0; j < courses.Length; j++)
                {
                    for (int k = 0; k < sems.Length; k++)
                    {
                        constMinStudent = new GRBLinExpr();
                        for (int i = 0; i < students.Length; i++)
                        {
                            constMinStudent.AddTerm(1, CourseAllocation[i, j, k]);
                        }
                        String sMinStudents = "MinStudentinCourse_" + j + 1 + ".Semester_" + k + 1;
                        model.AddConstr(constMinStudent, GRB.LESS_EQUAL, X, sMinStudents);
                    }
                }

                GRBLinExpr minX = new GRBLinExpr();
                minX.AddTerm(1, X);
                model.SetObjective(minX, GRB.MINIMIZE);

                model.Optimize();

                writeResults(CourseAllocation,students, courses, sems);
                
                double objectiveValue = model.Get(GRB.DoubleAttr.ObjVal);

                model.Dispose();
                env.Dispose();

            }
            catch (Exception e)
            {
                string temp = e.ToString();
            }
            return null;
        }

        private static void writeResults(GRBVar[,,] GRBModelData, StudentPreference[] students, Course[] courses, Semester[] sems)
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter("c:\\output.txt");
            using (var ctx = new ApplicationDbContext())
            {
                var rec = ctx.Recommendations.Create();
                rec.Name = "Test 1";
                for (int i = 0; i < students.Length; i++)
                {
                    for (int j = 0; j < courses.Length; j++)
                    {
                        for (int k = 0; k < sems.Length; k++)
                        {
                            try
                            {
                                if (GRBModelData[i, j, k].Get(GRB.DoubleAttr.X) == 1)
                                {
                                    rec.StudentPreferences.Add(students[i]);
                                    //CourseSemester
                                    
                                    //rec.Courses.Add(courses[j]);
                                    //rec.Semesters.Add(sems[k]);
                                    writer.WriteLine(students[i].GaTechId + " taking Course: " + courses[j].Number + ": " + courses[j].Name + " in Semester: " + sems[k].Type.ToString() + " " + sems[k].Year.ToString());
                                }
                            }
                            catch (GRBException e)
                            {
                            }
                        }
                    }
                }
                ctx.Recommendations.Add(rec);
                ctx.SaveChanges();
            }
            writer.Close();
        }
    }
}