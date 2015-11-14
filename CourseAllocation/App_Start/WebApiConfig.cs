using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace CourseAllocation
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            json.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            


            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
           name: "QueryCourses",
           routeTemplate: "Api/Courses",
           defaults: new { controller = "AdminApi", action = "Courses" }

       );


            config.Routes.MapHttpRoute(
      name: "QuerySemesters",
      routeTemplate: "Api/Semesters",
      defaults: new { controller = "AdminApi", action = "Semesters" }

  );


            config.Routes.MapHttpRoute(
      name: "QueryCourseSemesters",
      routeTemplate: "Api/CourseSemesters",
      defaults: new { controller = "AdminApi", action = "Courses" }

  );


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
