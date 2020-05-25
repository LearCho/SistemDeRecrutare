using System;
using System.Globalization;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SistemRecrutare.Models;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Web.Security;

namespace SistemRecrutare.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // ------------ Inregistrare Utilizatori ---------- //

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Inregistrare()
        {
            return View();
        }

        [NonAction]   // verificare daca exista deja email-ul
        public bool existaEmail(string email_id, string rol)
        {
            using (DBrecrutare db = new DBrecrutare())
            {
                // caz submit campuri goale: reumplere lista
                //UtilizatorAngajatViewModel user = new UtilizatorAngajatViewModel();
                //user.DomeniiSelectateIds = new List<int>();
                //user.ListaDomenii = new SelectList(db.domeniu_lucru.ToList(),"id_domeniu", "denumire_domeniu");

                switch (rol)
                {
                    case "Angajat":
                        var exista_id_angajat = db.utilizators.Where(u => u.email == email_id).FirstOrDefault();
                        return exista_id_angajat == null ? false : true;

                    case "Angajator":
                        var exista_id_angajator = db.angajators.Where(u => u.email == email_id).FirstOrDefault();
                        return exista_id_angajator == null ? false : true;

                    default:
                        return false;
                }
            }
        }

        [NonAction]  // trimitere link de verificare prin email
        public void trimiteLinkVerificareEmail(string email_id, string cod_activare, string rol)
        {
            var email_deLa = new MailAddress("sis.rec.utcb@gmail.com", "Sistem Recrutare");
            var email_pentru = new MailAddress(email_id);
            var email_deLa_parola = "licenta2020"; // parola actuala
            string email_titlu = "Contul tau a fost creat cu succes!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(email_deLa.Address, email_deLa_parola)
            };

            switch (rol) 
            { 
                case "Angajat":
                    var url_verificare_angajat = "/Account/VerificareCont/" + cod_activare;
                    var link_angajat = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url_verificare_angajat);

                    string body_angajat = "<br/><br/> Bine ai venit pe platforma de joburi! <p> Speram ca serviciile noastre sa " +
                         "te ajute in cautarea celui mai potrivit loc de munca pentru tine! Inca un pas si te poti apuca de" +
                         " treaba. <p> Te rugam sa dai click pe link-ul urmator pentru a finaliza inregistrarea : </p>" +
                         "<br/><a href = '" + link_angajat + "'>" + link_angajat + "</a> ";

                    using (var mesaj_angajat = new MailMessage(email_deLa, email_pentru)
                    {
                        Subject = email_titlu,
                        Body = body_angajat,
                        IsBodyHtml = true
                    })
                        smtp.Send(mesaj_angajat);
                    break;

                case "Angajator":
                    var url_verificare_angajator = "/Account/VerificareContAngajator/" + cod_activare;
                    var link_angajator = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url_verificare_angajator);

                    string body_angajator = "<br/><br/> Bine ai venit pe platforma de joburi! <p> Iti dorim sa ai o " +
                       "experienta placuta si speram ca serviciile noastre sa te ajute sa gasesti candidatii cei mai potriviti " +
                       "pentru rolurile din compania ta! Inca un pas si iti poti completa profilul companiei tale! </p>" +
                       "<p> Te rugam sa dai click pe link-ul urmator pentru a finaliza inregistrarea :</p> <br/><a href = '" + link_angajator + 
                       "'>" + link_angajator + "</a> ";

                    using (var mesaj_angajator = new MailMessage(email_deLa, email_pentru)
                    {
                        Subject = email_titlu,
                        Body = body_angajator,
                        IsBodyHtml = true
                    })
                        smtp.Send(mesaj_angajator);
                    break;

                default: break;
            }
          
            ViewBag.Message = "\tUn mesaj de confirmare ti-a fost trimis pe email, la adresa " + email_pentru;
        }


        // GET: /Account/ContNouAngajat
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ContNouAngajat()
        {
            UtilizatorAngajatViewModel u = new UtilizatorAngajatViewModel();

            using (DBrecrutare db = new DBrecrutare())
            {
                u.DomeniiSelectateIds = new List<int>();
                u.ListaDomenii = new SelectList(db.domeniu_lucru.ToList(),
                    "id_domeniu", "denumire_domeniu");

                //ViewData["DBDomenii"] = u.ListaDomenii;
            }
            return View(u);
        }

        // POST: /Account/ContNouAngajat           
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ContNouAngajat([Bind(Exclude = "verificare_email, cod_activare")] Models.UtilizatorAngajatViewModel cont_angajat)
        {
            bool status = false; // status & message merg in viewBag
            string message = "";

            if (ModelState.IsValid) // validare model
            {
                #region --- verificare email deja existent ---
                if (existaEmail(cont_angajat.Utilizator.email, "Angajat") == true)
                {
                    ModelState.AddModelError("ExistaEmail", "Adresa de e-mail introdusa exista deja");
                    // caz submit campuri goale: reumplere lista
                    using (DBrecrutare db = new DBrecrutare())
                    {
                        cont_angajat.DomeniiSelectateIds = new List<int>();
                        cont_angajat.ListaDomenii = new SelectList(db.domeniu_lucru.ToList(),
                            "id_domeniu", "denumire_domeniu");
                    }
                    return View(cont_angajat);
                }
                #endregion

                #region --- cod de activare ---
                cont_angajat.Utilizator.cod_activare = Guid.NewGuid();
                #endregion

                #region --- hashing parola si hashing confirma parola ---
                cont_angajat.Utilizator.parola = CriptareParola.Hash(cont_angajat.Utilizator.parola);
                cont_angajat.Utilizator.confirma_parola = CriptareParola.Hash(cont_angajat.Utilizator.confirma_parola);
                #endregion
                cont_angajat.Utilizator.verificare_email = false;
                cont_angajat.Utilizator.id_rol = 2;

                #region --- salvare in baza de date ---
                using (DBrecrutare db = new DBrecrutare())
                {
                    db.utilizators.Add(cont_angajat.Utilizator);
                    db.SaveChanges();

                    if (cont_angajat.Utilizator == null)
                    {
                        return View("ContNouAngajat");
                    }

                    if (cont_angajat.Utilizator != null && cont_angajat.DomeniiSelectateIds != null)
                    {
                        foreach (var id_selectat in cont_angajat.DomeniiSelectateIds) //
                        {
                            db.utilizator_domeniu_leg.Add(new utilizator_domeniu_leg
                            {
                                id_utilizator = cont_angajat.Utilizator.id_utilizator,
                                id_domeniu = id_selectat
                            });
                        }
                    }
                    else
                    {
                        db.utilizator_domeniu_leg.Add(new utilizator_domeniu_leg
                        {
                            id_utilizator = cont_angajat.Utilizator.id_utilizator,
                            id_domeniu = 9 //Altele
                        });
                    }

                    db.SaveChanges();

                    // trimitere email catre utilizator
                    trimiteLinkVerificareEmail(cont_angajat.Utilizator.email, cont_angajat.Utilizator.cod_activare.ToString(),
                        "Angajat");
                    message = "Inregistrarea a fost efectuata cu succes. Un link de activare " +
                        "ti-a fost trimis pe email, la adresa " + cont_angajat.Utilizator.email;
                    status = true;
                }
                #endregion
            }

            else
                message = "Cererea nu a putut fi efectuata";

            ViewBag.Mesage = message;
            ViewBag.Status = status;

            // caz submit campuri goale: reumplere lista
            using (DBrecrutare db = new DBrecrutare())
            {
                cont_angajat.DomeniiSelectateIds = new List<int>();
                cont_angajat.ListaDomenii = new SelectList(db.domeniu_lucru.ToList(),
                    "id_domeniu", "denumire_domeniu");

            }
            return View(cont_angajat);
        }

        // GET: /Account/ContNouAngajator
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ContNouAngajator()
        {
            return View();
        }

        // POST: /Account/ContNouAngajator
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ContNouAngajator([Bind(Exclude = "verificare_email, cod_activare")] Models.angajator cont_angajator)
        {
            bool status = false; // status & message merg in viewBag
            string message = "";

            if (ModelState.IsValid) // validare model
            {
                #region --- verificare email deja existent ---
                if (existaEmail(cont_angajator.email, "Angajator") == true)
                {
                    ModelState.AddModelError("ExistaEmail", "Adresa de e-mail introdusa exista deja");
                    return View(cont_angajator);
                }
                #endregion

                #region --- cod de activare ---
                cont_angajator.cod_activare = Guid.NewGuid();
                #endregion

                #region --- hashing parola si hashing confirma parola ---
                cont_angajator.parola = CriptareParola.Hash(cont_angajator.parola);
                cont_angajator.confirma_parola = CriptareParola.Hash(cont_angajator.confirma_parola);
                #endregion
                cont_angajator.verificare_email = false;
                cont_angajator.id_rol = 3;

                #region --- salvare in baza de date ---
                using (DBrecrutare db = new DBrecrutare())
                {
                    db.angajators.Add(cont_angajator);
                    db.SaveChanges();

                    if (cont_angajator == null)
                    {
                        return View("ContNouAngajator");
                    }

                    db.SaveChanges();

                    // trimitere email catre angajator
                    trimiteLinkVerificareEmail(cont_angajator.email, cont_angajator.cod_activare.ToString(), "Angajator");
                    message = "Inregistrarea a fost efectuata cu succes. Un link de activare v-a fost trimis pe email, " +
                        "la adresa " + cont_angajator.email;
                    status = true;
                }
                #endregion
            }

            else
                message = "Cererea nu a putut fi efectuata";

            ViewBag.Mesage = message;
            ViewBag.Status = status;

            return View(cont_angajator);
        }

        // --------- Verificare cod_activare din email pentru autentificare --------
        // GET: /Account/VerificareCont  (Angajat)
        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerificareCont(string id)
        {
            bool Status = false;
            using (DBrecrutare db = new DBrecrutare())
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var utilizator = db.utilizators.Where(u => u.cod_activare == new Guid(id)).FirstOrDefault();
                if(utilizator != null)
                {
                    utilizator.verificare_email = true;
                    db.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = " Cererea nu a putut fi efectuata";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        // GET: /Account/VerificareContAngajator
        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerificareContAngajator(string id)
        {
            bool Status = false;
            using (DBrecrutare db = new DBrecrutare())
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var angajator = db.angajators.Where(u => u.cod_activare == new Guid(id)).FirstOrDefault();
                if (angajator != null)
                {
                    angajator.verificare_email = true;
                    db.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = " Cererea nu a putut fi efectuata";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        // -------------- Logare Utilizatori -------------
        [HttpGet] 
        [AllowAnonymous]
        public ActionResult IntraInCont() 
        {
            return View();
        }

        //GET: /Account/IntraInContAngajat
        [HttpGet]
        [AllowAnonymous]
        public ActionResult IntraInContAngajat(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // POST: /Account/IntraInContAngajat
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult IntraInContAngajat(Logare logare, string ReturnUrl)
        {
            string mesaj = "";
            using (DBrecrutare db = new DBrecrutare())
            {
                var utilizator = db.utilizators.Where(u => u.email == logare.Email).FirstOrDefault();
                if (utilizator != null)
                {
                    if (string.Compare(CriptareParola.Hash(logare.Parola),
                        utilizator.parola) == 0)
                    {
                        int sesiune = logare.TineMinte ? 525600 : 20; //525600 min = 1 an
                        var ticket = new FormsAuthenticationTicket(logare.Email, logare.TineMinte,
                            sesiune);
                        string encriptare = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encriptare);
                        cookie.Expires = DateTime.Now.AddMinutes(sesiune);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ParolaInvalida", "\tParola nu este corecta");
                    }
                }
                else { mesaj = "Datele nu sunt valide!"; }
            }
            ViewBag.Message = mesaj;
            return View();
        }

        //GET: /Account/IntraInContAngajator
        [HttpGet]
        [AllowAnonymous]
        public ActionResult IntraInContAngajator(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // POST: /Account/IntraInContAngajator
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult IntraInContAngajator(Logare logare, string ReturnUrl)
        {
            string mesaj = "";
            using (DBrecrutare db = new DBrecrutare())
            {
                var angajator = db.angajators.Where(u => u.email == logare.Email).FirstOrDefault();
                if (angajator != null)
                {
                    if (string.Compare(CriptareParola.Hash(logare.Parola),
                        angajator.parola) == 0)
                    {
                        int sesiune = logare.TineMinte ? 525600 : 20; //525600 min = 1 an
                        var ticket = new FormsAuthenticationTicket(logare.Email, logare.TineMinte,
                            sesiune);
                        string encriptare = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encriptare);
                        cookie.Expires = DateTime.Now.AddMinutes(sesiune);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ParolaInvalida", "\tParola nu este corecta");
                    }
                }
                else { mesaj = "Datele nu sunt valide!"; }
            }
            ViewBag.Message = mesaj;
            return View();
        }

        //GET: /Account/Login
        //[AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{   ViewBag.ReturnUrl = returnUrl; return View();}

        // POST: /Account/Login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(Logare model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Parola, model.TineMinte, shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.TineMinte });
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Incercare de logare esuata.");
        //            return View(model);
        //    }
        //}

        // ------------ Logare: Parola Uitata ------------
        // GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        // POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //if (ModelState.IsValid)
        //{
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //    {
        //        return View("ForgotPasswordConfirmation");
        //    }
        //}
        //    return View(model);
        //}

        //
        // GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}
        //
        // GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        // POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    return View();
        //}

        //
        // GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}


        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }
        
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }
       
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new Utilizator { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // POST: /Account/LogOut
        [Authorize]
        [HttpPost]          
        //[ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("IntraInCont", "Account");
        }

        //GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}




// inregistrare cont nou fara enity framework & fara selected dropdowns

//[NonAction]   // verificare daca exista deja email-ul
//public bool existaEmail(string email_id)
//{
//    using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
//    {
//        sqlCon.Open();

//        string query = "SELECT * FROM dbo.utilizator WHERE email = @email";
//        SqlCommand sql_cmd = new SqlCommand(query, sqlCon);
//        sql_cmd.Parameters.AddWithValue("@email", email_id);

//        SqlDataReader sqlData = sql_cmd.ExecuteReader();

//        if (sqlData.HasRows)
//        {
//            return true;
//        }
//        return false;
//    }
//}

//// GET: /Account/ContNou
//[HttpGet]
//[AllowAnonymous]
//public ActionResult ContNouAngajat()
//{
//    //ViewBag.SexId = new SelectList(db.sexes, "id_sex", "denumire_sex");
//    ContNouAngajatModel cont_user = new ContNouAngajatModel();
//    DataTable dataTable_Sexe = new DataTable();
//    List<Sex_Utilizator> lista_sexe = new List<Sex_Utilizator>();

//    using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
//    {
//        sqlCon.Open();
//        string query_genders = "SELECT id_sex, denumire_sex FROM dbo.sex";
//        SqlDataAdapter sqlData = new SqlDataAdapter(query_genders, sqlCon);
//        sqlData.Fill(dataTable_Sexe);
//    }

//    if (dataTable_Sexe.Rows.Count == 1)
//    {
//        lista_sexe = (from DataRow row in dataTable_Sexe.Rows
//                      select new Sex_Utilizator
//                      {
//                          Id_Sex = Convert.ToInt32(row["id_sex"]),
//                          Denumire_Sex = row["denumire_sex"].ToString()
//                      }).ToList<Sex_Utilizator>();
//        cont_user.Sexe = lista_sexe;
//    }
//    return View();
//}

//[HttpPost]
//[AllowAnonymous]
//[ValidateAntiForgeryToken]
//public ActionResult ContNouAngajat([Bind(Exclude = "verificare_email, cod_activare")] ContNouAngajatModel cont_user)
//{
//    // DBModels db = new DBModels();
//    bool status = false;
//    string message = "";

//    if (ModelState.IsValid) // validare model
//    {
//        #region --- exista email ---
//        if (existaEmail(cont_user.Email) == true)
//        {
//            ModelState.AddModelError("ExistaEmail", "Adresa de e-mail exista deja");
//            return View(cont_user);
//        }
//        #endregion

//        #region --- cod activare ---
//        cont_user.cod_activare = Guid.NewGuid();
//        #endregion

//        #region --- hashing parola ---
//        cont_user.Parola = CriptareParola.Hash(cont_user.Parola);
//        cont_user.ConfirmaParola = CriptareParola.Hash(cont_user.ConfirmaParola);
//        #endregion
//        cont_user.verificare_email = false;

//        #region --- salvare in baza de date ---
//        using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
//        {
//            sqlCon.Open();

//            //List<Domenii_Lucru_Utilizator> domenii = new List<Domenii_Lucru_Utilizator>();

//            string query = "INSERT INTO dbo.utilizator VALUES(@email, @parola, @nume_utilizator, " +
//                                    " @prenume_utilizator, @oras, @telefon, @data_nasterii, " +
//                                    " @sex, @verificare_email, @cod_activare)";
//            SqlCommand sql_cmd = new SqlCommand(query, sqlCon);
//            sql_cmd.Parameters.AddWithValue("@email", cont_user.Email);
//            sql_cmd.Parameters.AddWithValue("@parola", cont_user.Parola);
//            sql_cmd.Parameters.AddWithValue("@nume_utilizator", cont_user.Nume);
//            sql_cmd.Parameters.AddWithValue("@prenume_utilizator", cont_user.Prenume);
//            sql_cmd.Parameters.AddWithValue("@oras", cont_user.Oras);
//            sql_cmd.Parameters.AddWithValue("@telefon", cont_user.Telefon);
//            sql_cmd.Parameters.AddWithValue("@data_nasterii", cont_user.Data_nasterii);


//            // ViewBag.SexId = new SelectList(db.sexes, "id_sex", "denumire_sex", cont_user.SelectedSexId);
//            //  string see = ViewBag.SexId.ToString();

//            sql_cmd.Parameters.AddWithValue("@sex", cont_user.SelectedSexId);

//            //sql_cmd.Parameters.AddWithValue("@domenii_lucru", cont_user.Domenii_lucru);
//            sql_cmd.Parameters.AddWithValue("@verificare_email", cont_user.verificare_email);
//            sql_cmd.Parameters.AddWithValue("@cod_activare", cont_user.cod_activare);

//            sql_cmd.ExecuteNonQuery();
//        }
//        #endregion
//    }

//    else
//        message = "Cererea nu a putut fi efectuata";

//    return View(cont_user);
//}





