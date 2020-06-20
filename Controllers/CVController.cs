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
        public ActionResult IncarcaFisier(HttpPostedFileBase fisiere, string email)
        {
            email = HttpContext.Application["Email"].ToString();

            if (fisiere != null && fisiere.ContentLength > 0)
            {
                try
                {
                    String ExtensieFisier = Path.GetExtension(fisiere.FileName).ToUpper();

                    if (ExtensieFisier == ".PDF" || ExtensieFisier == ".DOC" || ExtensieFisier == ".DOCX") // format specific
                    {
                        Stream str = fisiere.InputStream;
                        BinaryReader Br = new BinaryReader(str);
                        Byte[] ContinutFisier = Br.ReadBytes((Int32)str.Length);

                        DetaliiCvModel cv = new Models.DetaliiCvModel();
                        cv.nume_fisier = fisiere.FileName;
                        cv.continut_fisier = ContinutFisier;
                        SalveazaDetaliiFisier(cv, email);
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
            List<DetaliiCvModel> ListaFisiere = VeziListaFisiere();

            var Fisier = (from fis in ListaFisiere
                            where fis.id_cv.Equals(id_cv)
                            select new { fis.nume_fisier, fis.continut_fisier })
                            .ToList().FirstOrDefault();

            return File(Fisier.continut_fisier, "application/pdf", Fisier.nume_fisier);
        }

        // pentru un utlizator
        public PartialViewResult DetaliiCV(string email, string AplicantSauAngajator = "") // afisare pentru descarcare
        {
            email = HttpContext.Application["Email"].ToString();

            List<DetaliiCvModel> ListaFisiere = VeziListaFisiere(/*email*/);
            HttpContext.Application["AplicantSauAngajator"] = AplicantSauAngajator; // arata sau nu buton stergere

            return PartialView("DetaliiCV", ListaFisiere);
        }

        private List<DetaliiCvModel> VeziListaFisiere(/*string email*/)
        {
            string email = HttpContext.Application["Email"].ToString();

            List<DetaliiCvModel> ListaFisiere = new List<DetaliiCvModel>();
            DynamicParameters p = new DynamicParameters();
            p.Add("@email", email);

            using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
            {
                sqlCon.Open();

                ListaFisiere = SqlMapper.Query<DetaliiCvModel>(sqlCon, "vezi_detalii_cv", p, commandType: CommandType.StoredProcedure).ToList();
                return ListaFisiere;
            }
        }

        // pentru angajator
        public PartialViewResult DetaliiCV_Angajator() // afisare pentru descarcare
        {
            List<DetaliiCvModel> ListaFisiereA = VeziListaFisiereAngajator();

            return PartialView("DetaliiCV_Angajator", ListaFisiereA);
        }

        // pentru angajator
        private List<DetaliiCvModel> VeziListaFisiereAngajator()
        {
            List<DetaliiCvModel> ListaFisiereA = new List<DetaliiCvModel>();

            using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
            {
                sqlCon.Open();

                ListaFisiereA = SqlMapper.Query<DetaliiCvModel>(sqlCon, "vezi_detalii_cvuri_angajator", commandType: CommandType.StoredProcedure).ToList();
                return ListaFisiereA;
            }
        }

        // pentru angajator
        [HttpGet]
        public FileResult DescarcaFisierAngajator(int id_cv)
        {
                List<DetaliiCvModel> ListaFisiereA = VeziListaFisiereAngajator();

                var Fisier = (from fis in ListaFisiereA
                              where fis.id_cv.Equals(id_cv)
                              select new { fis.nume_fisier, fis.continut_fisier })
                                .ToList().FirstOrDefault();

                return File(Fisier.continut_fisier, "application/pdf", Fisier.nume_fisier);
       
        }

        public ActionResult CVuri_Angajator()
        {
            try
            {
                UtilizatorAngajatViewModel utilizatorModel = new UtilizatorAngajatViewModel();
                DataTable dataTable_angajati = new DataTable();

                using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
                {
                    sqlCon.Open();

                    string query = "select distinct nume_utilizator, prenume_utilizator, email, telefon, oras from " +
                        "dbo.cv inner join dbo.utilizator on utilizator.id_utilizator = cv.id_angajat";

                    SqlDataAdapter sqlData = new SqlDataAdapter(query, sqlCon);
                  //  sqlData.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData.Fill(dataTable_angajati);

                    //if (dataTable_angajati.Rows.Count > 0)
                    //{
                    //    utilizatorModel.Utilizator.nume_utilizator = dataTable_angajati.Rows[0][0].ToString();
                    //    utilizatorModel.Utilizator.prenume_utilizator = dataTable_angajati.Rows[0][1].ToString();
                    //    utilizatorModel.Utilizator.email = dataTable_angajati.Rows[0][2].ToString();
                    //    utilizatorModel.Utilizator.telefon = dataTable_angajati.Rows[0][3].ToString();
                    //    utilizatorModel.Utilizator.oras = dataTable_angajati.Rows[0][4].ToString();
                    //}
                }
                return View(dataTable_angajati);
            }
            catch
            {
                return View("Error");
            }
        }

        #endregion

        #region Salvare in baza de date prin proceduri stocate  
        private void SalveazaDetaliiFisier(DetaliiCvModel cv, string email)
        {
            DataTable dataTable_Angajat = new DataTable();
            DynamicParameters p = new DynamicParameters();
            p.Add("@nume_fisier", cv.nume_fisier);
            p.Add("@continut_fisier", cv.continut_fisier);

            using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
            {
                sqlCon.Open();
                // DbConnection();
                // con.Open();
                string id_query = "SELECT id_utilizator from dbo.utilizator WHERE utilizator.email = @email;";

                SqlDataAdapter sqlData = new SqlDataAdapter(id_query, sqlCon);
                sqlData.SelectCommand.Parameters.AddWithValue("@email", email);
                sqlData.Fill(dataTable_Angajat);

                if (dataTable_Angajat.Rows.Count == 1)
                {
                    p.Add("@id_angajat", Convert.ToInt32(dataTable_Angajat.Rows[0][0]));
                }

                sqlCon.Execute("dbo.adauga_detalii_cv", p, commandType: System.Data.CommandType.StoredProcedure); //procedura se afla in App_Data
                // con.Close();            
            }
        }
        #endregion

        #region Stergere Fisier
        public ActionResult StergeFisier(int id_cv)
        {
            //List<DetaliiCvModel> ListaFisiere = VeziListaFisiere();
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
                {
                    sqlCon.Open();

                    string query = "DELETE FROM dbo.cv WHERE id_cv = @id_cv;";

                    SqlCommand sql_cmd = new SqlCommand(query, sqlCon);
                    sql_cmd.Parameters.AddWithValue("@id_cv", id_cv);

                    sql_cmd.ExecuteNonQuery();
                }

                return RedirectToAction("IncarcaFisier", "CV", new { email = HttpContext.Application["Email"] });
            }
            catch
            {
                return View("Error");
            }
        }
        #endregion


        #region Conexiune cu baza de date  
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