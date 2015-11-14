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
        SPRING,
        SUMMER,
        FALL
    }

    //public sealed class SemesterType
    //{
    //    private readonly int Value;
    //    private readonly string Name;

    //    public static readonly SemesterType SPRING = new SemesterType(1, "Spring");
    //    public static readonly SemesterType SUMMER = new SemesterType(1, "Summer");
    //    public static readonly SemesterType FALL = new SemesterType(1, "Fall");

    //    private SemesterType(int _value, string _name)
    //    {
    //        Value = _value;
    //        Name = _name;
    //    }

    //    public override string ToString()
    //    {
    //        return Name;
    //    }
    //}

    public class Semester
    {
        [Required, Column(Order = 0), Key]
        public SemesterType Type { get; set; }

        [Required, Column(Order = 1), Key]
        public int Year { get; set; }
    }
}