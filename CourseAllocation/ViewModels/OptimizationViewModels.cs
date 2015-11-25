using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CourseAllocation.Models;

namespace CourseAllocation.ViewModels
{
    public class OptimizationViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public DateTime TimeStamp { get; set; }

        public string GaTechId { get; set; }

        public int MissingSeats { get; set; }

        //public IEnumerable<CourseSemesterViewModel> CourseSemesters { get; set; }

        public OptimizationViewModel(Recommendation m)
        {
            ID = m.ID;
            Name = m.CreatedAt.ToString("M/d/yy h:mm tt") +((m.Name != null) ? " (" + m.Name + ")" : "");
            TimeStamp = m.CreatedAt;
            GaTechId = m.CreatedBy.UserName;
            MissingSeats = m.MissingSeats;
        }

    }
}