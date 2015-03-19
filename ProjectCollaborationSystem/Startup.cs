using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjectCollaborationSystem.Startup))]
namespace ProjectCollaborationSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
