using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{
    public class CourseSemester
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public Semester Semester { get; set; }

        [Required]
        public Course Course { get; set; }

        [Required, DefaultValue("true")]
        public bool IsActive { get; set; }
    }
}