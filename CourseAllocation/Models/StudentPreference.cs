﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{
    public class StudentPreference : ILog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required, ForeignKey("Student")]
        public string GaTechId { get; set; }

        public Student Student { get; set; }

        [Required, DefaultValue("true")]
        public bool IsActive { get; set; }


        public virtual ICollection<Course> Courses { get; set; }

        public virtual ICollection<Recommendation> Recommendation { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required, ForeignKey("CreatedBy")]
        public string CreatedBy_Id { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

    }
}