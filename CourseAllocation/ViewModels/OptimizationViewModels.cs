using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CourseAllocation.Models;

namespace CourseAllocation.ViewModels
{
    public class OptimizationStudentViewModel
    {
        public string GaTechId { get; set; }

        public OptimizationStudentViewModel(RecommendationRecord m)
        {
            GaTechId = m.StudentPreference.GaTechId;
        }

    }


    public class OptimizationRecordViewModel : CourseSemesterViewModel
    { 
        public bool IsAssigned { get; set; }

        public bool IsCompleted { get; set; }


        public OptimizationRecordViewModel()
        {
            
        }

        public OptimizationRecordViewModel(RecommendationRecord m) : base(m.CourseSemester)
        {
            IsAssigned = true;
        }
    }

    //public class OptimizationCourseViewModel
    //{

    //    public List<OptimizationStudentViewModel> Students { get; set; }

    //    public int StudentCount
    //    {
    //        get { return Students.Count(); }
    //    }

    //    public int CourseSemester_ID { get; set; }

    //    public int Recommendation_ID { get; set; }


    //    public OptimizationCourseViewModel()
    //    {
    //        Students = new List<OptimizationStudentViewModel>();
    //    }
    //}


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
            Name = m.CreatedAt.ToString("M/d/yy h:mm tt") +((m.Name != null) ? " \"" + m.Name + "\"" : "");
            TimeStamp = m.CreatedAt;
            GaTechId = m.CreatedBy.UserName;
            MissingSeats = m.MissingSeats;
        }

    }
}