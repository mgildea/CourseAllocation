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

        [ForeignKey("Recommendation")]
        public int Recommendation_ID { get; set; }
        public virtual Recommendation Recommendation { get; set; }


        [ForeignKey("StudentPreference")]
        public int StudentPreference_ID { get; set; }
        public virtual StudentPreference StudentPreference { get; set; }



        [ForeignKey("CourseSemester")]
        public int CourseSemester_ID { get; set; }
        public virtual CourseSemester CourseSemester { get; set; }
    }
}