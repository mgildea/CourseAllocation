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
          name: "QueryCourseSemesters",
          routeTemplate: "Api/CourseSemesters",
          defaults: new { controller = "AdminApi", action = "CourseSemesters" }

      );


            config.Routes.MapHttpRoute(
        name: "QueryStudents",
        routeTemplate: "Api/Students",
        defaults: new { controller = "AdminApi", action = "Students" }

    );

            config.Routes.MapHttpRoute(
name: "QueryStudentPreferences",
routeTemplate: "Api/StudentPreferences",
defaults: new { controller = "AdminApi", action = "StudentPreferences" }

);


            config.Routes.MapHttpRoute(
      name: "CourseSemester",
      routeTemplate: "Api/CourseSemester",
      defaults: new { controller = "AdminApi", action = "CourseSemester" }

  );

            config.Routes.MapHttpRoute(
name: "RemoveCourseSemester",
routeTemplate: "Api/RemoveCourseSemester/{ID}",
defaults: new { controller = "AdminApi", action = "RemoveCourseSemester", ID = RouteParameter.Optional}

);


            config.Routes.MapHttpRoute(
name: "StudentPreference",
routeTemplate: "Api/StudentPreference",
defaults: new { controller = "AdminApi", action = "StudentPreference" }

);


            config.Routes.MapHttpRoute(
           name: "QueryCourses",
           routeTemplate: "Api/Courses",
           defaults: new { controller = "AdminApi", action = "Courses" }

       );

            config.Routes.MapHttpRoute(
         name: "Course",
         routeTemplate: "Api/Course/{ID}",
         defaults: new { controller = "AdminApi", action = "Course", ID = RouteParameter.Optional}

     );


            config.Routes.MapHttpRoute(
      name: "QuerySemesters",
      routeTemplate: "Api/Semesters",
      defaults: new { controller = "AdminApi", action = "Semesters" }

  );


            config.Routes.MapHttpRoute(
name: "AddYear",
routeTemplate: "Api/Year",
defaults: new { controller = "AdminApi", action = "Year" }

);



            config.Routes.MapHttpRoute(
      name: "Optimizer",
      routeTemplate: "Api/Optimizer/{RunName}",
      defaults: new { controller = "OptimizeApi", action = "Optimize", RunName = RouteParameter.Optional }

  );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
