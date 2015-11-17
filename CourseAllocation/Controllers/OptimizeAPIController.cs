using System;
using System.Linq;
using System.Web.Http;
using CourseAllocation.Models;
using Gurobi;

namespace CourseAllocation.Controllers
{
    public class OptimizeApiController : ApiController
    {
        private GRBLinExpr MAX_COURSES_PER_SEMESTER = new GRBLinExpr(2);
        public struct Student
        {
            public string GATechID;
            public int prefID;
            public Course[] PreferedCourses;
        }

        [HttpGet]
        public object Optimize()
        {
            Student[] students;
            Course[] courses = GetCourses();
            Semester[] sems = GetSemesters();
            CourseSemester[] crssems = GetCourseSemesters();

            using (var dbConn = new ApplicationDbContext())
            {
                var Holder = dbConn.StudentPreferences.Select(m => m.GaTechId).Distinct();
                students = new Student[Holder.Count()];
                int i = 0;
                foreach (string GTID in Holder)
                {
                    students[i].GATechID = GTID;
                    i++;
                }
                for (i = 0; i < students.Count(); i++)
                {
                    string sname = students[i].GATechID;
                    students[i].prefID = dbConn.StudentPreferences.Where(m => m.GaTechId == sname).Max(m => m.ID);
                    int prefID = students[i].prefID;

                    var PrefCourses = dbConn.StudentPreferences.Where(m => m.ID == prefID).Select(m => m.Courses).ToList();

                    students[i].PreferedCourses = new Course[PrefCourses[0].Count];
                    int c = 0;
                    foreach (Models.Course crs in PrefCourses[0])
                    {
                        students[i].PreferedCourses[c] = crs;
                        c++;
                    }
                }
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
                            if (students[i].PreferedCourses.Select(m => m.ID).Contains(courses[j].ID) && (crssems.Select(m => m.Course.ID).Contains(courses[j].ID) && crssems.Select(m => m.Semester.Type).Contains(sems[k].Type)))
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
                GRBLinExpr constpreReq;
                GRBLinExpr constpostReq;
                GRBLinExpr temp = new GRBLinExpr(2);

                //MUST TAKE DESIRED COURSE ONLY ONCE
                for (int i = 0; i < students.Length; i++)
                {
                    for (int j = 0; j < courses.Length; j++)
                    {
                        if (students[i].PreferedCourses.Select(m => m.ID).Contains(courses[j].ID))
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
                //writeResults(StdntCrsSem);
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
        private static Course[] GetCourses()
        {
            Course[] courses;
            using (var dbConn = new ApplicationDbContext())
            {
                var temp = dbConn.Courses.ToList();
                courses = new Course[temp.Count];
                for (int i = 0; i < temp.Count; i++)
                {
                    courses[i] = temp[i];
                }
            }
            return courses;
        }

        private static Semester[] GetSemesters()
        {
            Semester[] semesters;
            using (var dbConn = new ApplicationDbContext())
            {
                var temp = dbConn.Semesters.OrderBy(m => m.Year).Take(12).ToList();
                semesters = new Semester[temp.Count];
                for (int i = 0; i < temp.Count; i++)
                {
                    semesters[i] = temp[i];
                }
            }
            return semesters;
        }

        private static CourseSemester[] GetCourseSemesters()
        {
            CourseSemester[] crssems;
            using (var dbConn = new ApplicationDbContext())
            {
                var temp = dbConn.CourseSemesters.ToList();
                crssems = new CourseSemester[temp.Count];
                for (int i = 0; i < temp.Count; i++)
                {
                    crssems[i] = temp[i];
                }
            }
            return crssems;
        }
    }
}