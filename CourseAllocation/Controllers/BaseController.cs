using CourseAllocation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CourseAllocation.Controllers
{
    public class BaseController : Controller
    {
        private List<StatusMessage> StatusMessages;

        protected void StatusMessage(string message, StatusMessageType type)
        {
            StatusMessages.Add(new StatusMessage() { Message = message, Type = type });
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            StatusMessages = new List<StatusMessage>();

            if (TempData["StatusMessages"] != null)
                StatusMessages = (List<StatusMessage>)TempData["StatusMessages"];

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            TempData["StatusMessages"] = StatusMessages;

            base.OnActionExecuted(filterContext);
        }
    }
}