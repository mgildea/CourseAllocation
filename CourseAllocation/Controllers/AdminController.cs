using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourseAllocation.Models;

namespace CourseAllocation.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public ActionResult Semesters()
        //{
        //    return View();   
        //}

        //public ActionResult Courses()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public ActionResult CourseSemesters()
        //{
        //    return View();
        //}


    }
}