using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{
    public class Requirement
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int RequiredUnits { get; set; }

        public virtual ICollection<Requirement> Requirements { get; set; }
        public virtual ICollection<Requirement> RequirementsFor { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}