using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DataEntryWebForm.Startup))]
namespace DataEntryWebForm
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
