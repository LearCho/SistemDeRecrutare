using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SistemRecrutare.Models;

[assembly: OwinStartupAttribute(typeof(SistemRecrutare.Startup))]
namespace SistemRecrutare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreareRoluriUtilizatori();
        }

        public void CreareRoluriUtilizatori()
        {
            DBrecrutare db = new DBrecrutare();

            //var managerRoluri = new RoleManager<IdentityRole>(new 
            //    RoleStore<IdentityRole>(db));

            //var managerUtilizatori = new UserManager<Utilizator>(new
            //    UserStore<Utilizator>(db));

            //// crare rol Admin     
            //if (!managerRoluri.RoleExists("Admin"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Admin";
            //    managerRoluri.Create(role);
            //}

            //// crare rol Angajat     
            //if (!managerRoluri.RoleExists("Angajat"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Angajat";
            //    managerRoluri.Create(role);
            //}

            //// creare rol Angajator     
            //if (!managerRoluri.RoleExists("Angajator"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Angajator";
            //    managerRoluri.Create(role);
            //}
        }
    }
}
