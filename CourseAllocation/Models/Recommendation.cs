﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{
    public class Recommendation
    {
        //public Recommendation()
        //{
        //    StudentPreferences = new List<StudentPreference>();
        //    CourseSemesters = new List<CourseSemester>();
        //}
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public DateTime Name { get; set; }

        public virtual ICollection<StudentPreference> StudentPreferences { get; set; }

        public virtual ICollection<CourseSemester> CourseSemesters { get; set; }

        public virtual ICollection<RecommendationRecord> Records { get; set; }
    }
}