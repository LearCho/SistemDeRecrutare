using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SistemRecrutare.Startup))]
namespace SistemRecrutare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
