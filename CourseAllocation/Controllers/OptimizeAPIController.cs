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
                // Recommendation rec = dbConn.Recommendations.First();
                students = dbConn.StudentPreferences.Where(m => m.IsActive == true).Include(m => m.Courses).ToArray();
                crssems = dbConn.CourseSemesters.Where(m => m.IsActive == true).Include(m => m.Course).Include(m => m.Semester).ToArray();
                courses = crssems.Select(m => m.Course).Distinct().ToArray();
                sems = crssems.Select(m => m.Semester).Distinct().OrderBy(m => m.Type).OrderBy(m => m.Year).ToArray();

                var completed = dbConn.CompletedCourses.ToList();
                //try
                // {
                GRBEnv env = new GRBEnv("mip1.log");
                GRBModel model = new GRBModel(env);
                model.Set(GRB.StringAttr.ModelName, "Course Optimizer");
                //GRBVar X = model.AddVar(0, GRB.INFINITY, 0, GRB.INTEGER, "X");

                //Assignment of student, course, semester
                GRBVar[,,] CourseAllocation = new GRBVar[students.Length, courses.Length, sems.Length];
                for (int i = 0; i < students.Length; i++)
                {
                    for (int j = 0; j < courses.Length; j++)
                    {
                        for (int k = 0; k < sems.Length; k++)
                        {
                            if (students[i].Courses.Contains(courses[j]) && !completed.Any(m => m.GaTechId == students[i].GaTechId && courses[j].ID == m.Course_ID) && crssems.Contains(crssems.SingleOrDefault(m => m.Course == courses[j] && m.Semester == sems[k])))
                                CourseAllocation[i, j, k] = model.AddVar(0, 1, 1, GRB.BINARY, "students." + (i + 1).ToString() + "_Course." + (j + 1).ToString() + "_Semester." + (k + 1).ToString());
                            else
                                CourseAllocation[i, j, k] = model.AddVar(0, 0, 1, GRB.BINARY, "students." + (i + 1).ToString() + "_Course." + (j + 1).ToString() + "_Semester." + (k + 1).ToString());
                        }
                    }
                }
                model.Set(GRB.IntAttr.ModelSense, 1);
                model.Update();

                GRBLinExpr constMinStudent = new GRBLinExpr();
                //GRBLinExpr constMaxStudCrsSem;

                //MUST TAKE DESIRED COURSE ONLY ONCE
                for (int i = 0; i < students.Length; i++)
                {
                    for (int j = 0; j < courses.Length; j++)
                    {
                        if (students[i].Courses.Contains(courses[j]))
                        {
                            GRBLinExpr constStudentDesiredCourses = 0.0;
                            for (int k = 0; k < sems.Length; k++)
                            {
                                constStudentDesiredCourses.AddTerm(1.0, CourseAllocation[i, j, k]);
                            }
                            String sStudentDesiredCourses = "DesiredCourse." + j + 1 + "_Student." + i + 1;
                            model.AddConstr(constStudentDesiredCourses == 1, sStudentDesiredCourses);
                        }
                    }
                    //MAX COURSES PER SEMESTER
                    for (int k = 0; k < sems.Length; k++)
                    {
                        GRBLinExpr constMaxPerSem = 0.0;
                        for (int j = 0; j < courses.Length; j++)
                            constMaxPerSem.AddTerm(1, CourseAllocation[i, j, k]);

                        String sCourseSem = "maxCourseStudent." + i + 1 + "_Semester." + k + 1;
                        model.AddConstr(constMaxPerSem <= MAX_COURSES_PER_SEMESTER, sCourseSem);
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

                //ASSIGN MAX STUDENTS PER COURSE/SEMESTER 
                for (int k = 0; k < sems.Length; k++)
                {
                    for (int j = 0; j < courses.Length; j++)
                    {
                        if (crssems.Any(m => m.Course.ID == courses[j].ID && m.Semester.Type == sems[k].Type && m.Semester.Year == sems[k].Year))
                        {
                            GRBLinExpr constMaxStudCrsSem = 0.0;
                            for (int i = 0; i < students.Length; i++)
                            {
                                constMaxStudCrsSem.AddTerm(1.0, CourseAllocation[i, j, k]);
                            }
                            //String sMaxStudentsCourseSem = "maxStudentsPerCourse." + j + "_Semester." + k;
                            model.AddConstr(constMaxStudCrsSem <= crssems.Single(m => m.Course.ID == courses[j].ID && m.Semester.Type == sems[k].Type && m.Semester.Year == sems[k].Year).StudentLimit, sems[k].Type.ToString() + "." + sems[k].Year.ToString() + "." + courses[j].Name);
                        }
                    }
                }

                model.Optimize();

                int status = model.Get(GRB.IntAttr.Status);
                if (GRB.Status.INFEASIBLE == status)
                {
                    int origVars = model.Get(GRB.IntAttr.NumVars);
                    model.FeasRelax(0, false, false, true);
                    model.Optimize();
                    status = model.Get(GRB.IntAttr.Status);
                    if (status == GRB.Status.INFEASIBLE)
                    {

                    }
                    else
                    {
                        double objectiveValue = model.Get(GRB.DoubleAttr.ObjVal);
                        writeResults(CourseAllocation, students, courses, sems, crssems, dbConn, objectiveValue);
                    }
                }
               

                model.Dispose();
                env.Dispose();

                //}
                //catch (Exception e)
                //{
                //    string temp = e.ToString();
                //}
            }
            return null;
        }

        private static void writeResults(GRBVar[,,] GRBModelData, StudentPreference[] students, Course[] courses, Semester[] sems, CourseSemester[] crssems, ApplicationDbContext ctx, double MaxClassSize)
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter("c:\\output.txt");
            Recommendation rec = new Recommendation();
            rec.Records = new List<RecommendationRecord>();
            rec.StudentPreferences = students;
            rec.CourseSemesters = crssems;

            //  rec.Name = DateTime.Now;
            rec.MaxClassSize = MaxClassSize;
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
                                rec.Records.Add(new RecommendationRecord() { StudentPreference = students[i], CourseSemester = crssems.Single(m => m.Course == courses[j] && m.Semester == sems[k]) });
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
            writer.Close();
        }
    }
}