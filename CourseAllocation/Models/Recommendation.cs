using System;
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
        public Recommendation()
        {
            StudentPreferences = new HashSet<StudentPreference>();
            CourseSemesters = new HashSet<CourseSemester>();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<StudentPreference> StudentPreferences { get; set; }

        public virtual ICollection<CourseSemester> CourseSemesters { get; set; }
    }
}