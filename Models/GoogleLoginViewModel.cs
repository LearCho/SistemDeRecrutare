using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace SistemRecrutare.Models
{
    public class GoogleLoginViewModel // nu
    {
        public string email { get; set; }
        public string nume { get; set; }
        public string prenume { get; set; }
        public string identificator_nume { get; set; }

        internal static GoogleLoginViewModel GetLoginInfo(ClaimsIdentity identity)
        {
            if (identity.Claims.Count() == 0 || identity.Claims.FirstOrDefault
            (x => x.Type == ClaimTypes.Email) == null)
            {
                return null;
            }
            return new GoogleLoginViewModel
            {
                email = identity.Claims.FirstOrDefault
              (x => x.Type == ClaimTypes.Email).Value,
                nume = identity.Claims.FirstOrDefault
              (x => x.Type == ClaimTypes.Surname).Value,
                prenume = identity.Claims.FirstOrDefault
              (x => x.Type == ClaimTypes.Name).Value,
                identificator_nume = identity.Claims.FirstOrDefault
              (x => x.Type == ClaimTypes.NameIdentifier).Value,
            };
        }
    }
}