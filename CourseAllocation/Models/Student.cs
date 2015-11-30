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


        public virtual ICollection<StudentPreference> StudentPreferences { get; set; }

        public virtual ICollection<CompletedCourse> CompletedCourses { get; set; }
    }
}