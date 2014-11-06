using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AuthWebApi.Startup))]

namespace AuthWebApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}