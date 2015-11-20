using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseAllocation.Models
{
    interface ILog
    {
        DateTime CreatedAt { get; set; }

        string CreatedBy_Id { get; set; }
        ApplicationUser CreatedBy { get; set; }
    }
}
