using System.Web;
using System.Web.Optimization;

namespace CourseAllocation
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-3.3.0.js"
                ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-select.min.js",
                      "~/Scripts/selectPicker.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/spin.js", 
                      "~/Scripts/site.js"));

            //bundling angular does not currently work due to the $scope syntax
            //bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
            //        "~/Scripts/angular.min.js",
            //        "~/Scripts/angular-resource.min.js"
            //    ));

            //bundles.Add(new ScriptBundle("~/bundles/adminAppJs").Include(
            //        "~/Scripts/Apps/Admin/admin.js",
            //        "~/Scripts/Apps/Admin/Controllers/adminController.js",
            //        "~/Scripts/Apps/Admin/Services/adminServices.js"
            //    ));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/bootstrap-social.css",
                      "~/Content/bootstrap-select.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/Landingcss").Include(
                   "~/Content/Landing.css"));
            

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
