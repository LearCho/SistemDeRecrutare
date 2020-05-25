using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemRecrutare.Controllers
{
    public class UtilizatorLogat : Controller
    {
        // Utilizator logat  -- mutat in _LoginPartial
        // GET: Utilizator
        [Authorize]
        public ActionResult HomeUtilizatorLogat()
        {
            return View();
        }
    }
}