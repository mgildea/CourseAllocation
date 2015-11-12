using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseAllocation.Models
{

    public enum StatusMessageType
    {
        Success,
        Warning,
        Danger,
        Info
    }

    public class StatusMessage
    {
        public string Message { get; set; }
        public StatusMessageType Type { get; set; }

        public string CssClass
        {
            get
            {
                switch(Type)
                {
                    case StatusMessageType.Success :
                        return "alert-success";
                    case StatusMessageType.Warning :
                        return "alert-warning";
                    case StatusMessageType.Danger :
                        return "alert-danger";
                    case StatusMessageType.Info :
                        return "alert-info";
                    default :
                        return String.Empty;
                }
            }
        }
    }
}