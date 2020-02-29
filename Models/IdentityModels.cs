using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace SistemRecrutare.Models
{
    //  clasa Utilizator
    public class Utilizator : IdentityUser //nu folosesc IdentityUser momentan
    {
        public string email { get; set; }
        public string parola { get; set; }
        public string nume_utilizator { get; set; }
        public string prenume_utilizator { get; set; }
        public string oras { get; set; }
        public string nr_tel { get; set; }
        public DateTime? data_nasterii { get; set; } 
        public Sex_Utilizator sex { get; set; }
        public string domenii_lucru { get; set; }
        public bool verificare_email { get; set; } //////---
        public Guid cod_activare { get; set; } //////----
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Utilizator> manager)
        {
        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }  // nope
    }

    public class ApplicationDbContext : IdentityDbContext<Utilizator>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}