using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using CourseAllocation.Models;
using System.Collections.Generic;
using Gurobi;
using System.Net.Http;
using System.Net;

namespace CourseAllocation.Controllers
{
    public class OptimizeApiController : ApiController
    {
        private StudentPreference[] students;
        private Course[] courses;
        private Semester[] sems;
        private CourseSemester[] crssems;
        private GRBLinExpr MAX_COURSES_PER_SEMESTER = new GRBLinExpr(2);

        /// <summary>
        /// The Optimize class calculates student to course and semester placement for all students, courses, semesters
        /// across all course / semester offerings.  The objective of the Optimize class is to minimize the total slack 
        /// while maximizing the total numbers of students being put into a course during a semester.  Constraints taken
        /// into consideration are students can't repeat a class in other semesters, a student can't take more than 2
        /// classes per semester, prerequisite courses need to be taken in a semester prior courses requiring the pre-
        /// reqsuite course.
        /// </summary>
        /// <param name="RunName"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Optimize(string RunName)
        {
            using (var dbConn = new ApplicationDbContext())
            {
                //Variables for students, semesters, courses, and course/semester offerings
                students = dbConn.StudentPreferences.Where(m => m.IsActive == true).Include(m => m.Courses).Include(m => m.Student.CompletedCourses).OrderByDescending(m => m.Student.CompletedCourses.Count()).ToArray();
                crssems = dbConn.CourseSemesters.Where(m => m.IsActive == true).Include(m => m.Course).Include(m => m.Semester).ToArray();
                courses = crssems.Select(m => m.Course).Distinct().ToArray();
                sems = crssems.Select(m => m.Semester).Distinct().OrderBy(m => m.Type).OrderBy(m => m.Year).ToArray();

                var completed = dbConn.CompletedCourses.ToList();
                try
                {
                    GRBEnv env = new GRBEnv("mip1.log");
                    GRBModel model = new GRBModel(env);
                    model.Set(GRB.StringAttr.ModelName, "Course Optimizer");
                    GRBVar[,] slacks = new GRBVar[courses.Length, sems.Length];

                    //Assignment of student, course, and semester.  Student must have a desire to take the coure, has not taken the course, and the course is offered to be included
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

                    //MUST TAKE DESIRED COURSE ONLY ONCE
                    //Constrains the students to only take courses they desire once and for when the course is offered and does not allow a repeat of a course in another semester
                    for (int i = 0; i < students.Length; i++)
                    {
                        for (int j = 0; j < courses.Length; j++)
                        {
                            if (students[i].Courses.Contains(courses[j]) && !completed.Any(m => m.GaTechId == students[i].GaTechId && courses[j].ID == m.Course_ID))
                            {
                                GRBLinExpr constStudentDesiredCourses = 0.0;
                                for (int k = 0; k < sems.Length; k++)
                                {
                                    if (crssems.Any(m => m.Course.ID == courses[j].ID && m.Semester.Type == sems[k].Type && m.Semester.Year == sems[k].Year))
                                        constStudentDesiredCourses.AddTerm(1.0, CourseAllocation[i, j, k]);
                                }
                                String sStudentDesiredCourses = "DesiredCourse." + j + 1 + "_Student." + i + 1;
                                model.AddConstr(constStudentDesiredCourses == 1, sStudentDesiredCourses);
                            }
                        }

                        //MAX COURSES PER SEMESTER
                        //Constrains the students to only have a maximum number of 2 courses per semester.
                        for (int k = 0; k < sems.Length; k++)
                        {
                            GRBLinExpr constMaxPerSem = 0.0;
                            for (int j = 0; j < courses.Length; j++)
                            {
                                if (!completed.Any(m => m.GaTechId == students[i].GaTechId && courses[j].ID == m.Course_ID) && (crssems.Any(m => m.Course.ID == courses[j].ID && m.Semester.Type == sems[k].Type && m.Semester.Year == sems[k].Year)))
                                    constMaxPerSem.AddTerm(1, CourseAllocation[i, j, k]);
                            }
                            String sCourseSem = "maxCourseStudent." + i + 1 + "_Semester." + k + 1;
                            model.AddConstr(constMaxPerSem <= MAX_COURSES_PER_SEMESTER, sCourseSem);
                        }

                        //PREREQUISITES
                        //Constrains the students to take prerequisite courses prior to taking a course that needs the prerequisite
                        for (int j = 0; j < courses.Length; j++)
                        {
                            if (courses[j].Prerequisites.Any() && students[i].Courses.Contains(courses[j]) && !completed.Any(m => m.GaTechId == students[i].GaTechId && courses[j].ID == m.Course_ID))
                            {

                                foreach (var prereq in courses[j].Prerequisites)
                                {
                                    int prereqIndex = Array.IndexOf(courses, prereq);
                                    GRBLinExpr coursePrereqConst1 = 0.0;
                                    GRBLinExpr coursePrereqConst2 = 0.0;
                                    if (!completed.Any(m => m.GaTechId == students[i].GaTechId && m.Course.ID == prereq.ID))
                                    {
                                      
                                        for (int k = 0; k < sems.Length; k++)
                                        {
                                           
                                            if (prereqIndex >= 0)
                                            {
                                                coursePrereqConst1.AddTerm(k + 1, CourseAllocation[i, prereqIndex, k]);
                                                coursePrereqConst2.AddTerm(k, CourseAllocation[i, j, k]);
                                            }
                                        }
                                    }
                                    model.AddConstr(coursePrereqConst1, GRB.LESS_EQUAL, coursePrereqConst2, "PREREQ_Student" + i + "_Course+" + j + "_Prereq" + prereqIndex);

                                }

                            }
                        }
                    }

                    //SENIORITY
                    //Students are already ordered from dB query by seniority in descending order and puts a preference to senior students over the next student that desires that
                    //same course with less seniority.
                    for (int j = 0; j < courses.Length; j++)
                    {
                        for (int i = 0; i < students.Length - 1; i++)
                        {
                            if (students[i].Courses.Contains(courses[j]) && !completed.Any(m => m.GaTechId == students[i].GaTechId && courses[j].ID == m.Course_ID))
                            {
                                int SemsRemain = (students[i].Courses.Count - students[i].Student.CompletedCourses.Count) / 2 + (students[i].Courses.Count - students[i].Student.CompletedCourses.Count) % 2;
                                for (int n = i + 1; n < students.Length; n++)
                                {
                                    if (students[n].Courses.Contains(courses[j]) && !completed.Any(m => m.GaTechId == students[n].GaTechId && courses[j].ID == m.Course_ID))
                                    {
                                        GRBLinExpr​ seniority = 0.0;
                                        for (int k = 0; k < sems.Length; k++)
                                        {
                                            if (crssems.Any(m => m.Course.ID == courses[j].ID && m.Semester.Type == sems[k].Type && m.Semester.Year == sems[k].Year))
                                            {
                                                if (k <= SemsRemain)
                                                {
                                                    seniority​.AddTerm(1.0, CourseAllocation[i, j, k]);
                                                    seniority​.AddTerm(-1.0, CourseAllocation​[n, j, k]);
                                                }
                                                else
                                                {
                                                    seniority​.AddTerm(-1.0, CourseAllocation[i, j, k]);
                                                    seniority​.AddTerm(1.0, CourseAllocation​[n, j, k]);
                                                }
                                            }
                                        }
                                        model.AddConstr(seniority, GRB.GREATER_EQUAL, 0, "Seniority for Student." + students[i] + "_Course." + courses[j]);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    //Add the slack variable for all semester & course offerings then constrain the maximum number of students
                    //to take a couse in a semester.
                    for (int k = 0; k < sems.Length; k++)
                    {
                        for (int j = 0; j < courses.Length; j++)
                        {
                            if (crssems.Any(m => m.Course.ID == courses[j].ID && m.Semester.Type == sems[k].Type && m.Semester.Year == sems[k].Year))
                            {
                                slacks[j, k] = model.AddVar(0, GRB.INFINITY, 0, GRB.INTEGER, sems[k].Type.ToString() + "." + sems[k].Year.ToString() + "." + courses[j].Name + ".Slacks");
                                GRBLinExpr constMaxStudCrsSem = 0.0;
                                for (int i = 0; i < students.Length; i++)
                                {
                                    if (!completed.Any(m => m.GaTechId == students[i].GaTechId && courses[j].ID == m.Course_ID))
                                        constMaxStudCrsSem.AddTerm(1.0, CourseAllocation[i, j, k]);
                                }
                                model.Update();
                                constMaxStudCrsSem.AddTerm(-1.0, slacks[j, k]);
                                model.AddConstr(constMaxStudCrsSem <= crssems.Single(m => m.Course.ID == courses[j].ID && m.Semester.Type == sems[k].Type && m.Semester.Year == sems[k].Year).StudentLimit, sems[k].Type.ToString() + "." + sems[k].Year.ToString() + "." + courses[j].Name);
                            }
                        }
                    }

                    //Add total slack to the optimization model for all courses in the semesters they are offered.
                    GRBVar totSlack = model.AddVar(0, GRB.INFINITY, 0, GRB.INTEGER, "totSlack");
                    GRBLinExpr lhs = new GRBLinExpr();
                    lhs.AddTerm(-1.0, totSlack);
                    for (int j = 0; j < courses.Length; j++)
                    {
                        for (int k = 0; k < sems.Length; k++)
                        {
                            if (crssems.Any(m => m.Course.ID == courses[j].ID && m.Semester.Type == sems[k].Type && m.Semester.Year == sems[k].Year))
                                lhs.AddTerm(1.0, slacks[j, k]);
                        }
                    }
                    model.Update();
                    model.AddConstr(lhs, GRB.EQUAL, 0, "totSlack");

                    // Objective: minimize the total slack
                    GRBLinExpr obj = new GRBLinExpr();
                    obj.AddTerm(1.0, totSlack);
                    model.SetObjective(obj);

                    //Optimize the model to minimize the total slack and maximize students to course offerings based on input variables and constraints.
                    model.Optimize();

                    //Write Results optimization results to database
                    writeResults(CourseAllocation, students, courses, sems, crssems, dbConn, Convert.ToInt32(model.Get(GRB.DoubleAttr.ObjVal)), RunName);

                    //Clean-Up
                    model.Dispose();
                    env.Dispose();
                }
                catch (Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An Error occured while running the optimization.");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// The writeResults class writes the Course Optimization run to the database.  The class takes in the optimization model, all students, all courses, 
        /// all semesters, all course/semster offerings, the dB context, the optimization variable (total slack for overbooked students across all course/
        /// semester offerings, and the RunName provided to the Optimization engine.  
        /// </summary>
        /// <param name="GRBModelData"></param>
        /// <param name="students"></param>
        /// <param name="courses"></param>
        /// <param name="sems"></param>
        /// <param name="crssems"></param>
        /// <param name="ctx"></param>
        /// <param name="ObjectiveValue"></param>
        /// <param name="RunName"></param>
        private static void writeResults(GRBVar[,,] GRBModelData, StudentPreference[] students, Course[] courses, Semester[] sems, CourseSemester[] crssems, ApplicationDbContext ctx, int ObjectiveValue, string RunName)
        {
            Recommendation rec = new Recommendation() { Name = RunName };
            rec.Records = new List<RecommendationRecord>();
            rec.StudentPreferences = students;
            rec.CourseSemesters = crssems;

            rec.MissingSeats = ObjectiveValue;
            for (int i = 0; i < students.Length; i++)
            {
                for (int j = 0; j < courses.Length; j++)
                {
                    for (int k = 0; k < sems.Length; k++)
                    {
                        try
                        {
                            if (GRBModelData[i, j, k].Get(GRB.DoubleAttr.X) == 1)
                                rec.Records.Add(new RecommendationRecord() { StudentPreference = students[i], CourseSemester = crssems.Single(m => m.Course == courses[j] && m.Semester == sems[k]) });
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
    }
}