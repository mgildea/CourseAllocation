using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{
    public enum SemesterType
    {
        Spring,
        Summer,
        Fall
    }

 

    public class Semester : ILog
    {
        [Required, Column(Order = 0), Key]
        public SemesterType Type { get; set; }

        [Required, Column(Order = 1), Key]
        public int Year { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required, ForeignKey("CreatedBy")]
        public string CreatedBy_Id { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
    }
}