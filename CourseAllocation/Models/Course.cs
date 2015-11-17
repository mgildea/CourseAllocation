using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public virtual ICollection<Course> PrerequisiteFor { get; set; }

        [JsonIgnore]
        public virtual ICollection<StudentPreference> StudentPreferences { get; set; }


        [JsonIgnore]
        public virtual ICollection<Requirement> Requirements { get; set; }


        public Course()
        {
            Units = 3;
        }
    }
}