using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{
    public class RecommendationRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public StudentPreference StudentPreference { get; set; }

        public CourseSemester CourseSemester { get; set; }
    }
}