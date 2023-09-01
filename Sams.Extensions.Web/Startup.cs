using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sams.Extensions.Web.Startup))]
namespace Sams.Extensions.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
