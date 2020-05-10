using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;

namespace SistemRecrutare
{
    public static class CriptareParola
    {
        public static string Hash(string parola)
        {
                try
                {
                    return Convert.ToBase64String(
                        System.Security.Cryptography.SHA256.Create()
                        .ComputeHash(Encoding.UTF8.GetBytes(parola)));
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
        }
    }
}