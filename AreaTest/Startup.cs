using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AreaTest.Startup))]
namespace AreaTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
