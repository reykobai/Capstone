using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjectCollaborationSystem.Startup))]
namespace ProjectCollaborationSystem
{
    [assembly: OwinStartup(typeof(ProjectCollaborationSystem.Startup))] 
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
