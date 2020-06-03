using SistemRecrutare.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace SistemRecrutare.Controllers
{
    [Authorize]
    public class CVController : Controller
    {
        #region Incarcare fisier cv de descarcat  
        public ActionResult IncarcaFisier()
        {
            return View();
        }

        [HttpPost]  
        public ActionResult IncarcaFisier(HttpPostedFileBase fisiere)
        {
            if (fisiere != null && fisiere.ContentLength > 0)
            {
                try
                {
                    String ExtensieFisier = Path.GetExtension(fisiere.FileName).ToUpper();

                    if (ExtensieFisier == ".PDF" || ExtensieFisier == ".DOC") // format specific
                    {
                        Stream str = fisiere.InputStream;
                        BinaryReader Br = new BinaryReader(str);
                        Byte[] ContinutFisier = Br.ReadBytes((Int32)str.Length);

                        DetaliiCvModel cv = new Models.DetaliiCvModel();
                        cv.nume_fisier = fisiere.FileName;
                        cv.continut_fisier = ContinutFisier;
                        SalveazaDetaliiFisier(cv);
                    }
                    else
                    {
                        ViewBag.StatusFisier = "Formatul fisierului este invalid!";
                        return View();
                    }
                }

                catch (Exception ex)
                {
                    ViewBag.Status = "Eroare: " + ex.Message.ToString();
                }
            }
            return RedirectToAction("IncarcaFisier");
        }
        #endregion

        #region Descarcare fisier cv  
        [HttpGet]
        public FileResult DescarcaFisier(int id_cv)
        {
            List<DetaliiCvModel> ListaFisiere = GetFileList();

            var Fisier = (from fis in ListaFisiere
                            where fis.id_cv.Equals(id_cv)
                            select new { fis.nume_fisier, fis.continut_fisier })
                            .ToList().FirstOrDefault();

            return File(Fisier.continut_fisier, "application/pdf", Fisier.nume_fisier);
        }

        //[HttpGet]
        public PartialViewResult DetaliiCV()
        {
            List<DetaliiCvModel> ListaFisiere = GetFileList();

            return PartialView("DetaliiCV", ListaFisiere);
        }

        private List<DetaliiCvModel> GetFileList()
        {
            List<DetaliiCvModel> ListaFisiere = new List<DetaliiCvModel>();

            // DbConnection();
            // con.Open();
            using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
            {
                sqlCon.Open();
                ListaFisiere = SqlMapper.Query<DetaliiCvModel>(sqlCon, "vezi_detalii_cv", commandType: CommandType.StoredProcedure).ToList();
                //sqlCon.Close();
                return ListaFisiere;
            }
        }
        #endregion

        #region Salvare in baza de date prin proceduri stocate  
        private void SalveazaDetaliiFisier(DetaliiCvModel cv)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@nume_fisier", cv.nume_fisier);
            p.Add("@continut_fisier", cv.continut_fisier);
            using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
            {
                sqlCon.Open();
                // DbConnection();
                // con.Open();
                sqlCon.Execute("dbo.adauga_detalii_cv", p, commandType: System.Data.CommandType.StoredProcedure);
               // con.Close();
            }
        }
        #endregion


        #region Database connection  
        //private SqlConnection con;
        //private string connection_str;
        //private void DbConnection()
        //{
        //    connection_str = ConfigurationManager.ConnectionStrings["dbcon"].ToString();
        //    con = new SqlConnection(connection_str);
        //}
        #endregion

    }
}