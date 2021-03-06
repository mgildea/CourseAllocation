﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace CourseAllocation.Annotations
{
    public class HttpParamActionAttribute : ActionNameSelectorAttribute
    {
        [Pure]
        [ContractVerification(false)]
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            if (actionName.Equals(methodInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                return true;

            var request = controllerContext.RequestContext.HttpContext.Request;
            return request[methodInfo.Name] != null;




            //if (actionName.Equals(methodInfo.Name, StringComparison.InvariantCultureIgnoreCase))
            //    return true;

            //if (!actionName.Equals("Action", StringComparison.InvariantCultureIgnoreCase))
            //    return false;

            //var request = controllerContext.RequestContext.HttpContext.Request;
            //return request[methodInfo.Name] != null;
        }
    }
}