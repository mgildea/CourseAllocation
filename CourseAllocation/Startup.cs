using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CourseAllocation.Startup))]
namespace CourseAllocation
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
