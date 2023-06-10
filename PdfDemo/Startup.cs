using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PdfDemo.Startup))]
namespace PdfDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
