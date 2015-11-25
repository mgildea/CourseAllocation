using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{
    public class Recommendation : ILog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Name { get; set; }

        //[Required]
        //public DateTime Name { get; set; }
        
       // public Double MaxClassSize { get; set; }

        public int MissingSeats { get; set; }

        [JsonIgnore]
        public virtual ICollection<StudentPreference> StudentPreferences { get; set; }

        [JsonIgnore]
        public virtual ICollection<CourseSemester> CourseSemesters { get; set; }

        [JsonIgnore]
        public virtual ICollection<RecommendationRecord> Records { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required, ForeignKey("CreatedBy")]
        public string CreatedBy_Id { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }
    }
}