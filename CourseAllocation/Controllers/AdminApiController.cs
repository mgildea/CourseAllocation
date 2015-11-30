﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CourseAllocation.Models;
using System.Data.Entity;
using CourseAllocation.ViewModels;
using System.Web.Http.ModelBinding;

namespace CourseAllocation.Controllers
{
    [Authorize]
    public class AdminApiController : ApiController
    {


        [HttpGet]
        public IEnumerable<OptimizationViewModel> Optimizations()
        {
            using (var ctx = new ApplicationDbContext())
            {
                return ctx.Recommendations.Include(m => m.CreatedBy).ToList().Select(m => new OptimizationViewModel(m));
            }
        }


        [HttpGet]
        public IEnumerable<OptimizationRecordViewModel> OptimizationStudentRecomendations(int Recomendation_ID, string GaTechId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var recomendations = ctx.RecomendationRecords.Include(m => m.StudentPreference).Where(m => m.Recommendation_ID == Recomendation_ID && m.StudentPreference.GaTechId == GaTechId).ToList();

                var Records = recomendations.Select(m => new OptimizationRecordViewModel(m)).ToList();

                int prefId = recomendations.Select(m => m.StudentPreference_ID).Distinct().Single();


                foreach (var course in ctx.CompletedCourses.Include(m => m.Course).Where(m => m.GaTechId == GaTechId))
                {
                    Records.Add(new OptimizationRecordViewModel()
                    {
                        ID = course.Course_ID,
                        IsCompleted = true,
                        Name = course.Course.Name,
                        Number = course.Course.Number
                    });
                }

                foreach (var course in ctx.StudentPreferences.Single(m => m.ID == prefId).Courses)
                {
                    if (!Records.Select(m => m.Number).Contains(course.Number))
                    {
                        Records.Add(new OptimizationRecordViewModel()
                        {
                            ID = course.ID,
                            IsAssigned = false,
                            Name = course.Name,
                            Number = course.Number

                        });
                    }

                }




                return Records;
            }
        }

        [HttpGet]
        public IEnumerable<CourseSemesterViewModel> OptimizationOfferings(int Recomendation_ID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var recomendation = ctx.Recommendations
                    .Include(m => m.CourseSemesters.Select(n => n.Course))
                    .Include(m => m.CourseSemesters.Select(n => n.Semester))
                    .Include(m => m.Records)
                    .Single(m => m.ID == Recomendation_ID);

                var temp1 = recomendation
                    .CourseSemesters.ToList();

                var temp = temp1.Select(m => new CourseSemesterViewModel(m, m.Recommendation.Single(n => n.ID == Recomendation_ID).Records.Where(n => n.CourseSemester_ID == m.ID))).ToList();
                return temp;
            }
        }

        [HttpGet]
        public IEnumerable<OptimizationStudentViewModel> OptimizationAssignedStudents(int Recomendation_ID, int CourseSemester_ID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                return ctx.RecomendationRecords.Include(m => m.StudentPreference).Where(m => m.Recommendation_ID == Recomendation_ID && m.CourseSemester_ID == CourseSemester_ID).ToList().Select(m => new OptimizationStudentViewModel(m));
            }
        }



        [HttpGet]
        public IEnumerable<string> Students()
        {
            using (var ctx = new ApplicationDbContext())
            {
                return ctx.StudentPreferences.Where(m => m.IsActive).Select(m => m.GaTechId).ToList();
            }
        }


        [HttpGet]
        public IEnumerable<CourseViewModel> StudentPreferences(string GaTechId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var preference = ctx.StudentPreferences.Include(m => m.Courses).SingleOrDefault(m => m.IsActive && m.GaTechId == GaTechId);

                if (preference == null)
                    return new List<CourseViewModel>();

                var courseIds = preference.Courses.Select(m => m.ID).ToArray();

                var viewmodels = ctx.Courses.Include(m => m.Prerequisites).ToList().Where(m => courseIds.Contains(m.ID)).Select(m => new CourseViewModel(m)).ToList();

                //  var completed = ctx.CompletedCourses.Include(m => m.Course.Prerequisites).Where(m => m.GaTechId == GaTechId).ToList().Select(m => new CourseViewModel(m.Course, true));


                foreach (var completed in ctx.CompletedCourses.Include(m => m.Course.Prerequisites).Where(m => m.GaTechId == GaTechId).ToList().Select(m => new CourseViewModel(m.Course, true)))
                {
                    var vm = viewmodels.SingleOrDefault(m => m.ID == completed.ID);
                    if (vm != null)
                    {
                        viewmodels.Remove(vm);
                    }

                    viewmodels.Add(completed);
                }


                return viewmodels;
            }
        }

        [HttpPost]
        public HttpResponseMessage StudentPreference(StudentPreference studentPreference)
        {

            using (var ctx = new ApplicationDbContext())
            {
                ctx.StudentPreferences.Where(m => m.GaTechId == studentPreference.GaTechId && m.IsActive).ToList().ForEach(m => m.IsActive = false);

                var ids = studentPreference.Courses.Select(n => n.ID);

                var completed = ctx.CompletedCourses.Where(m => m.GaTechId == studentPreference.GaTechId).Select(m => m.Course_ID);

                studentPreference.Courses = ctx.Courses.Where(m => ids.Contains(m.ID) && !completed.Contains(m.ID)).ToList();


                foreach (var course in studentPreference.Courses)
                {
                    foreach (var prereq in ctx.Courses.Single(m => m.ID == course.ID).Prerequisites)
                    {
                        if (!studentPreference.Courses.Select(m => m.ID).Contains(prereq.ID))
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.Conflict, String.Format("Missing prerequisite course {0} for {1}", prereq.Number, course.Number));
                           // ModelState.AddModelError("", String.Format("Missing prerequisite course {0} for {1}", prereq.Number, course.Number));
                        }
                    }
                }


                //if (!ModelState.IsValid)
                //{

                //    foreach (ModelState modelState in ModelState.Values)
                //    {
                //        foreach (ModelError error in modelState.Errors)
                //        {
                //            return Request.CreateErrorResponse(HttpStatusCode.Conflict, error.ErrorMessage);

                //        }
                //    }


                //}


                if (ctx.Students.SingleOrDefault(m => m.GaTechId == studentPreference.GaTechId) == null)
                    ctx.Students.Add(new Student() { GaTechId = studentPreference.GaTechId });


                ctx.StudentPreferences.Add(studentPreference);

                ctx.SaveChanges();


                return Request.CreateResponse(HttpStatusCode.OK);



            }


        }

        [HttpPost]
        public bool RemoveCourseSemester(int ID)
        {
            using (var ctx = new ApplicationDbContext())
            {

                ctx.CourseSemesters.Include(m => m.Course).Include(m => m.Semester).Single(m => m.ID == ID).IsActive = false;



                // ctx.CourseSemesters.Find(ID).IsActive = false;

                ctx.SaveChanges();
            }

            return true;
        }


        [HttpPost]
        public CourseSemesterViewModel CourseSemester(CourseSemester courseSemester)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (courseSemester == null || courseSemester.Course == null || courseSemester.Semester == null ||
                    ctx.CourseSemesters.Where(m => m.Course.ID == courseSemester.Course.ID && m.Semester.Type == courseSemester.Semester.Type && m.Semester.Year == courseSemester.Semester.Year && m.IsActive).Any())
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
                return ctx.CourseSemesters.Include(m => m.Course).Include(m => m.Semester).Where(m => m.IsActive).ToList().Select(m => new CourseSemesterViewModel(m));
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


        [HttpGet]
        public CourseViewModel Course([FromUri] int ID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                return new CourseViewModel(ctx.Courses.Include(m => m.Prerequisites).Single(m => m.ID == ID));
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

                if (course.Prerequisites != null)
                {
                    var prereqIds = course.Prerequisites.Select(n => n.ID);

                    course.Prerequisites = ctx.Courses.Where(m => prereqIds.Contains(m.ID)).ToList();
                }
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
