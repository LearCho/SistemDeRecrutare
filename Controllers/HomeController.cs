using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SistemRecrutare.Models;

namespace SistemRecrutare.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Home/CautaJobCuvantCheie
        [AllowAnonymous]
        public ActionResult CautaJobCuvantCheie()
        {
            return View();
        }

        //
        // POST: /Home/CautaJobCuvantCheie           
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CautaJobCuvantCheie(CautaJobCuvantCheieModel model)
        {
            if (ModelState.IsValid)
            {                              
                
            }

            return View(model);
        }

        public ActionResult Joburi()
        {
            ViewBag.Message = "Gaseste cele mai noi joburi";

            return View();
        }

        public ActionResult Cariera()
        {
            ViewBag.Message = "Pagina de cariere";

            return View();
        }
    }
}