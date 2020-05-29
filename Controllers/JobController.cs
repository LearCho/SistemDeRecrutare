using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security;
using System.Windows;
using System.Text;
using System.IO;
using PagedList;
using PagedList.Mvc;
using SistemRecrutare.Models;

namespace SistemRecrutare.Controllers
{
    public class JobController : Controller
    {
        public static string connectionString = @"Data Source = (local)\SQLINSTANCE; 
                                       Initial Catalog = DB_sistem_recrutare;
                                       Integrated Security = true";

        // GET: Job/ListaAdmin
        [HttpGet]
        public ActionResult ListaAdmin(string val_cautare) // afiseaza joburi in dataTable
        {
            DBrecrutare db = new DBrecrutare();            
            //TODO : de creat pagini cu PagedList

            try
            {    // DataTable dataTable_Joburi = new DataTable();
                //using (SqlConnection sqlCon = new SqlConnection(connectionString))
                //{   sqlCon.Open();
                //    SqlDataAdapter sqlDataA = new SqlDataAdapter("SELECT * FROM dbo.job", sqlCon);
                //    sqlDataA.Fill(dataTable_Joburi);}

                return View(db.jobs.Where(j => j.cod_job.Contains(val_cautare) || j.denumire_job.Contains(val_cautare) ||
                j.data_creare_job.ToString().Contains(val_cautare) || j.angajator.Contains(val_cautare) || 
                j.data_expirare_job.ToString().Contains(val_cautare) || j.tara.Contains(val_cautare) || j.oras.Contains(val_cautare)
                || j.descriere_job.Contains(val_cautare) || val_cautare == null));
            }
            catch (SqlException exc)
            {
                throw new InvalidOperationException("Datele nu au putut fi citite.", exc);
            }

          //  return View(dataTable_Joburi);
        }

        // GET: Job/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // varifica daca contine fisier sau nu
        [NonAction]
        public bool ExistaFisier(HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }

        // GET: Job/Creare
        [HttpGet]
        [Authorize(Roles = "Admin, Angajator")]
        public ActionResult Creare()
        {          
            return View(new SistemRecrutare.Models.JobModel());
        }

        // POST: Job/Creare
        [HttpPost]
        [Authorize(Roles = "Admin, Angajator")]
        public ActionResult Creare(Models.JobModel jobModel)
        {
            //try
            //{
                // upload imagine in baza de date
                foreach (string upload in Request.Files)
                {
                    if (!ExistaFisier(Request.Files[upload]))
                        continue;
                    Stream fileStream = Request.Files[upload].InputStream;
                    int lungimeFisier = Request.Files[upload].ContentLength;
                    jobModel.imagine_job = new byte[lungimeFisier];
                    fileStream.Read(jobModel.imagine_job, 0, lungimeFisier);
                }

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();

                    string query = "INSERT INTO dbo.job VALUES(@denumire_job, @cod_job, @data_expirare_job," +
                        " @angajator,  @imagine_job, @descriere_job, @data_creare_job, @tara, @oras)";
                    SqlCommand sql_cmd = new SqlCommand(query, sqlCon);
                   
                    //sql_cmd.Parameters.AddWithValue("@id_job", id_job);
                    sql_cmd.Parameters.AddWithValue("@denumire_job", jobModel.denumire_job);
                    sql_cmd.Parameters.AddWithValue("@cod_job", jobModel.cod_job);
                    sql_cmd.Parameters.AddWithValue("@data_creare_job", jobModel.data_creare_job);
                    sql_cmd.Parameters.AddWithValue("@data_expirare_job", jobModel.data_expirare_job);
                    sql_cmd.Parameters.AddWithValue("@angajator", jobModel.angajator);
                    sql_cmd.Parameters.AddWithValue("@tara", jobModel.tara);
                    sql_cmd.Parameters.AddWithValue("@oras", jobModel.oras);
                    sql_cmd.Parameters.AddWithValue("@imagine_job", jobModel.imagine_job);
                    sql_cmd.Parameters.AddWithValue("@descriere_job", jobModel.descriere_job);

                    sql_cmd.ExecuteNonQuery();
                }
            //}
            //catch
            //{
            //    return Content("<script language='javascript' type='text/javascript'>alert ('A aparut o eroare la introducerea datelor! ');</script>");
            //}  
            
            return RedirectToAction("ListaAdmin");          
        }

        // GET: Job/Editare/5
        //[Authorize(Roles = "Admin, Angajator")]
        public ActionResult Editare(string cod_job)
        {
            //using (DBrecrutare db = new DBrecrutare())
            //{
            //    return View(db.jobs.Where(j => j.cod_job == cod_job).FirstOrDefault());
            //}

            Models.JobModel jobModel = new Models.JobModel();

            DataTable dataTable_Job = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query1 = "SELECT id_job, denumire_job AS 'DENUMIRE JOB', cod_job AS 'COD', data_expirare_job AS" +
                    "'DATA EXPIRARE', angajator AS 'ANGAJATOR', imagine_job AS ' ', descriere_job AS 'DESPRE', tara AS" +
                    " 'TARA', oras AS 'ORAS', data_creare_job FROM dbo.job WHERE cod_job = @cod_job;";
                SqlDataAdapter sqlData = new SqlDataAdapter(query1, sqlCon);
                sqlData.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                sqlData.Fill(dataTable_Job);
            }

            if (dataTable_Job.Rows.Count == 1)
            {
                jobModel.id_job = Convert.ToInt32(dataTable_Job.Rows[0][0].ToString());
                jobModel.denumire_job = dataTable_Job.Rows[0][1].ToString();
                jobModel.cod_job = dataTable_Job.Rows[0][2].ToString();
                if (dataTable_Job.Rows[0][3].ToString() != "")
                {
                    jobModel.data_expirare_job = Convert.ToDateTime(dataTable_Job.Rows[0][3]);
                }
                jobModel.angajator = dataTable_Job.Rows[0][4].ToString();
                if (dataTable_Job.Rows[0][5].ToString() != "")
                {
                    jobModel.imagine_job = (byte[])(dataTable_Job.Rows[0][5]);
                }
                jobModel.descriere_job = dataTable_Job.Rows[0][6].ToString();
                jobModel.tara = dataTable_Job.Rows[0][7].ToString();
                jobModel.oras = dataTable_Job.Rows[0][8].ToString();
                if (dataTable_Job.Rows[0][9].ToString() != "")
                {
                    jobModel.data_creare_job = Convert.ToDateTime(dataTable_Job.Rows[0][9]);
                }

                return View(jobModel);
            }
            else
                return View("Error");
        }

        // POST: Job/Editare/5
        [HttpPost]
        //[Authorize(Roles = "Admin, Angajator")]
        public ActionResult Editare(string cod_job, JobModel jobModel/*, FormCollection collection*/)
        {
            //try
            //{
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();

                string query = "UPDATE dbo.job SET denumire_job = @denumire_job, cod_job = @cod_job," +
                    " data_expirare_job = @data_expirare_job, angajator = @angajator, imagine_job = " +
                    " @imagine_job, descriere_job = @descriere_job, data_creare_job = @data_creare_job, " +
                    "tara = @tara, oras = @oras WHERE cod_job = @cod_job;";
                SqlCommand sql_cmd = new SqlCommand(query, sqlCon);

                //sql_cmd.Parameters.AddWithValue("@id_job", id_job);
                sql_cmd.Parameters.AddWithValue("@denumire_job", jobModel.denumire_job);
                sql_cmd.Parameters.AddWithValue("@cod_job", jobModel.cod_job);
                sql_cmd.Parameters.AddWithValue("@data_creare_job", jobModel.data_creare_job);
                sql_cmd.Parameters.AddWithValue("@data_expirare_job", jobModel.data_expirare_job);
                sql_cmd.Parameters.AddWithValue("@angajator", jobModel.angajator);
                sql_cmd.Parameters.AddWithValue("@tara", jobModel.tara);
                sql_cmd.Parameters.AddWithValue("@oras", jobModel.oras);
                sql_cmd.Parameters.AddWithValue("@imagine_job", jobModel.imagine_job);
                sql_cmd.Parameters.AddWithValue("@descriere_job", jobModel.descriere_job);

                sql_cmd.ExecuteNonQuery();
            }
            return View("ListaAdmin");
            //}
            //catch
            //{
            //    return View("Error");
            //}
}

        // GET: Job/Delete/5
        [Authorize(Roles = "Admin, Angajator")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Job/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin, Angajator")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: Job/ListaUtilizator
        [HttpGet]
        public ActionResult ListaUtilizator() // afiseaza joburi pentru vedere utilizator
        {
            DataTable dataTable_Job = new DataTable();
            //TODO : de creat pagini
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    SqlDataAdapter sqlDataA = new SqlDataAdapter("SELECT * FROM dbo.job", sqlCon);
                    sqlDataA.Fill(dataTable_Job);
                }
            }
            catch (SqlException exc)
            {
                throw new InvalidOperationException("Datele nu au putut fi citite.", exc);
            }

            return View(dataTable_Job);
        }
        [Authorize(Roles = "Admin, Angajator, Angajat")]
        public void AplicaLaJob()
        {

        }
    }
}
