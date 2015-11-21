using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{
    public class CompletedCourse
    {
        [Required, Column(Order = 0), Key]
        public string GaTechId { get; set; }

        [Required, Column(Order = 1), Key, ForeignKey("Course")]
        public int Course_ID { get; set; }
        public virtual Course Course { get; set; }
    }
}