﻿using CourseAllocation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseAllocation.ViewModels
{
    public class CourseSemesterViewModel
    {
        public int ID { get; set; }

        public SemesterType TypeValue { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }

        public CourseSemesterViewModel(CourseSemester m)
        {
            ID = m.ID;
            TypeValue = m.Semester.Type;
            Type = m.Semester.Type.ToString();
            Year = m.Semester.Year;
            Number = m.Course.Number;
            Name = m.Course.Name;
        }
        
    }

    public class SemesterViewModel
    {
        public SemesterType TypeValue { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }


        public SemesterViewModel(Semester m)
        {
            TypeValue = m.Type;
            Type = m.Type.ToString();
            Year = m.Year;
        }

    }

    public class CourseViewModel
    {
        public int ID { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public bool IsFoundational { get; set; }

        public List<CourseViewModel> Prerequisites { get; set; }


        public CourseViewModel(Course m)
        {
            ID = m.ID;
            Number = m.Number;
            Name = m.Name;
            IsFoundational = m.IsFoundational;

            Prerequisites = (m.Prerequisites == null) ? new List<CourseViewModel>() :  m.Prerequisites.Select(n => new CourseViewModel(n)).ToList();
        }
    }
}