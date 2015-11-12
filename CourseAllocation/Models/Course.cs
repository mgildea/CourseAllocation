using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseAllocation.Models
{
    public class Course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID{ get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Units { get; set; }

        [Required]
        public bool IsFoundational { get; set; }

        public virtual ICollection<Course> Prerequisites { get; set; }
        public virtual ICollection<Course> PrerequisiteFor { get; set; }

        public virtual ICollection<Requirement> Requirements { get; set; }
    }
}