using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{
    public class Student
    {
        [Key]
        public string GaTechId { get; set; }


        public virtual IEnumerable<StudentPreference> StudentPreferences { get; set; }

        public virtual IEnumerable<CompletedCourse> CompletedCourses { get; set; }
    }
}