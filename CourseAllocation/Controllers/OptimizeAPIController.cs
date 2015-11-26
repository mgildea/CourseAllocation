﻿using System;
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

        [HttpPost]
        public object Optimize(string RunName)
        {
            using (var dbConn = new ApplicationDbContext())
            {
                // Recommendation rec = dbConn.Recommendations.First();
                students = dbConn.StudentPreferences.Where(m => m.IsActive == true).Include(m => m.Courses).ToArray();

                //int[] studCompleteCourses = new int[students.Length];
                //for (int i = 0; i < students.Length; i++)
                //{
                //    String Id = students[i].GaTechId;
                //    studCompleteCourses[i] = dbConn.CompletedCourses.Count(m => m.GaTechId == Id);
                //}
                crssems = dbConn.CourseSemesters.Where(m => m.IsActive == true).Include(m => m.Course).Include(m => m.Semester).ToArray();
                courses = crssems.Select(m => m.Course).Distinct().ToArray();
                sems = crssems.Select(m => m.Semester).Distinct().OrderBy(m => m.Type).OrderBy(m => m.Year).ToArray();

                var completed = dbConn.CompletedCourses.ToList();
                //try
                // {
                GRBEnv env = new GRBEnv("mip1.log");
                GRBModel model = new GRBModel(env);
                model.Set(GRB.StringAttr.ModelName, "Course Optimizer");
                GRBVar[,] slacks = new GRBVar[courses.Length, sems.Length];

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

                //MUST TAKE DESIRED COURSE ONLY ONCE
                for (int i = 0; i < students.Length; i++)
                {
                    for (int j = 0; j < courses.Length; j++)
                    {
                        if (students[i].Courses.Contains(courses[j]) && !completed.Any(m => m.GaTechId == students[i].GaTechId && courses[j].ID == m.Course_ID))
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
                        {
                            if (!completed.Any(m => m.GaTechId == students[i].GaTechId && courses[j].ID == m.Course_ID))
                                constMaxPerSem.AddTerm(1, CourseAllocation[i, j, k]);
                        }
                        String sCourseSem = "maxCourseStudent." + i + 1 + "_Semester." + k + 1;
                        model.AddConstr(constMaxPerSem <= MAX_COURSES_PER_SEMESTER, sCourseSem);
                    }

                    //PREREQUISITES
                    //    GRBVar[] PreReq = new GRBVar[students[i].Courses.Count];
                    //    double[] weight = new double[students[i].Courses.Count];
                    //    int count = 0;
                    //    for (int j = 0; j < courses.Length;j++)
                    //    {
                    //        if (students[i].Courses.Contains(courses[j]))
                    //        {
                    //            if (courses[j].PrerequisiteFor.Count > 0)
                    //            {
                    //                PreReq[count] = model.AddVar(0, 2, 1, GRB.INTEGER, "Student." + students[i].GaTechId + "-PrerequisiteCourse." + courses[j].Name);
                    //                weight[count] = 2;
                    //            }
                    //            else
                    //            {
                    //                PreReq[count] = model.AddVar(0, 1, 1, GRB.INTEGER, "Student." + students[i].GaTechId + "-PrerequisiteCourse." + courses[j].Name);
                    //                weight[count] = 1;
                    //            }
                    //                count++;
                    //        }
                    //        //model.AddConstr(PreReq,GRB., constpostReq, "PreRequisiteCourse." + j + 1 + "_Student." + i + 1);
                    //    }
                    //    model.AddSOS(PreReq, weight, GRB.SOS_TYPE1);
                }

                
                //for (int j=0;j<courses.Length;j++)
                //{
                //    for (int i=0;i<students.Length-1;i++)
                //    {
                //        GRBLinExpr Seniority = 0;
                //        for (int k=0; k < sems.Length; k++)
                //        {
                //            Seniority.AddTerm(1.0, courses[i, j, k]);
                //        }
                //    }
                //}

             
                for (int k = 0; k < sems.Length; k++)
                {
                    for (int j = 0; j < courses.Length; j++)
                    {
                        slacks[j, k] = model.AddVar(0, GRB.INFINITY, 0, GRB.INTEGER, sems[k].Type.ToString() + "." + sems[k].Year.ToString() + "." + courses[j].Name + ".Slacks");
                    }
                }
                model.Update();

                //SENIORITY
                //StudentPreference[] sortedStudents = new StudentPreference[students.Length];
                //for (int j = 0; j < courses.Length; j++)
                //{
                //    for (int i = 0; i < sortedStudents.Length-1; i++)
                //    {
                //        GRBLinExpr​ seniority = 0.0;
                //        for (int k = 0; k < sems.Length;k++)
                //        {
                //            seniority​.AddTerm(​1.0​, CourseAllocation[i, j, k]);
                //            seniority​.AddTerm(-1.0, CourseAllocation​[i, j, k]);
                //        }
                //        model.AddConstr(seniority, GRB.GREATER_EQUAL, 0, "Seniority for Student." + students[i] + "_Course." + courses[j]);
                //    }
                //}

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
                                if (!completed.Any(m => m.GaTechId == students[i].GaTechId && courses[j].ID == m.Course_ID))
                                    constMaxStudCrsSem.AddTerm(1.0, CourseAllocation[i, j, k]);
                            }
                            constMaxStudCrsSem.AddTerm(-1.0, slacks[j, k]);
                            model.AddConstr(constMaxStudCrsSem <= crssems.Single(m => m.Course.ID == courses[j].ID && m.Semester.Type == sems[k].Type && m.Semester.Year == sems[k].Year).StudentLimit, sems[k].Type.ToString() + "." + sems[k].Year.ToString() + "." + courses[j].Name);
                        }
                    }
                }

                GRBVar totSlack = model.AddVar(0, GRB.INFINITY, 0, GRB.INTEGER, "totSlack");
                GRBLinExpr lhs = new GRBLinExpr();
                lhs.AddTerm(-1.0, totSlack);
                for (int j = 0; j < courses.Length; j++)
                {
                    for (int k = 0; k < sems.Length; k++)
                    {
                        lhs.AddTerm(1.0, slacks[j, k]);
                    }
                }
                model.Update();
                model.AddConstr(lhs, GRB.EQUAL, 0, "totSlack");

                // Objective: minimize the total slack
                GRBLinExpr obj = new GRBLinExpr();
                obj.AddTerm(1.0, totSlack);
                model.SetObjective(obj);

                model.Optimize();

                int status = model.Get(GRB.IntAttr.Status);
                if (GRB.Status.INFEASIBLE == status)
                {
                    int origVars = model.Get(GRB.IntAttr.NumVars);
                    model.FeasRelax(0, false, false, true);
                    model.Optimize();
                }

                status = model.Get(GRB.IntAttr.Status);
                if (status == GRB.Status.OPTIMAL)
                {
                    int objectiveValue = Convert.ToInt32(model.Get(GRB.DoubleAttr.ObjVal));
                    writeResults(CourseAllocation, students, courses, sems, crssems, dbConn, objectiveValue, RunName);
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

        private static void writeResults(GRBVar[,,] GRBModelData, StudentPreference[] students, Course[] courses, Semester[] sems, CourseSemester[] crssems, ApplicationDbContext ctx, int ObjectiveValue, string RunName)
        {
            System.IO.StreamWriter writer = null;

            //if (System.Diagnostics.Debugger.IsAttached)
            writer = new System.IO.StreamWriter("c:\\output.txt");


            Recommendation rec = new Recommendation() { Name = RunName };
            rec.Records = new List<RecommendationRecord>();
            rec.StudentPreferences = students;
            rec.CourseSemesters = crssems;

            //  rec.Name = DateTime.Now;
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
                            {
                                rec.Records.Add(new RecommendationRecord() { StudentPreference = students[i], CourseSemester = crssems.Single(m => m.Course == courses[j] && m.Semester == sems[k]) });

                                //if (System.Diagnostics.Debugger.IsAttached)
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