using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TRADENET.Startup))]
namespace TRADENET
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
