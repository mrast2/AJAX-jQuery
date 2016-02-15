using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IS8012_FinalProject.Startup))]
namespace IS8012_FinalProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
