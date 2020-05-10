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
        public ActionResult Creare()
        {          
            return View(new SistemRecrutare.Models.JobModel());
        }

        // POST: Job/Creare
        [HttpPost]
        public ActionResult Creare(Models.JobModel jobModel)
        {
            try
            {
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

                    string query = "INSERT INTO dbo.job VALUES(@denumire_job, @cod_job, @data_creare_job," +
                        " @data_expirare_job, @angajator, @tara, @oras, @imagine_job, @descriere_job)";
                    SqlCommand sql_cmd = new SqlCommand(query, sqlCon);
                   
                    //sql_cmd.Parameters.AddWithValue("@id_job", id_job);
                    sql_cmd.Parameters.AddWithValue("@denumire_job", jobModel.denumire_job);
                    sql_cmd.Parameters.AddWithValue("@cod_job", jobModel.cod_job);
                    sql_cmd.Parameters.AddWithValue("@data_creare_job", jobModel.data_creare);
                    sql_cmd.Parameters.AddWithValue("@data_expirare_job", jobModel.data_expirare);
                    sql_cmd.Parameters.AddWithValue("@angajator", jobModel.angajator);
                    sql_cmd.Parameters.AddWithValue("@tara", jobModel.tara);
                    sql_cmd.Parameters.AddWithValue("@oras", jobModel.oras);
                    sql_cmd.Parameters.AddWithValue("@imagine_job", jobModel.imagine_job);
                    sql_cmd.Parameters.AddWithValue("@descriere_job", jobModel.descriere_job);

                    sql_cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                return Content("<script language='javascript' type='text/javascript'>alert ('A aparut o eroare la introducerea datelor! ');</script>");
            }  
            
            return RedirectToAction("ListaAdmin");          
        }

        // GET: Job/Editare/5
        public ActionResult Editare(string cod_job = "")
        {
            try
            {
                Models.JobModel jobModel = new Models.JobModel();
                
                //foreach(DataRow row in dataTable_Job.Rows)
                //{
                //    cod_job = row[0].ToString();             
                //}

                DataTable dataTable_Job = new DataTable();
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query1 = "SELECT id_job, denumire_job AS 'DENUMIRE JOB', cod_job AS 'COD', data_expirare_job AS" +
                        "'DATA EXPIRARE', angajator AS 'ANGAJATOR', imagine_job AS ' ', descriere_job AS 'DESPRE' FROM" +
                        " dbo.job WHERE cod_job = @cod_job";
                    SqlDataAdapter sqlData = new SqlDataAdapter(query1, sqlCon);
                    sqlData.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData.Fill(dataTable_Job);
                }

                if (dataTable_Job.Rows.Count == 1)
                {
                    jobModel.id_job = Convert.ToInt32(dataTable_Job.Rows[0][0].ToString());
                    jobModel.denumire_job = dataTable_Job.Rows[0][1].ToString();
                    jobModel.cod_job = dataTable_Job.Rows[0][2].ToString();
                    jobModel.data_creare = Convert.ToDateTime(dataTable_Job.Rows[0][3]);
                    jobModel.data_expirare = Convert.ToDateTime(dataTable_Job.Rows[0][3]);
                    jobModel.angajator = dataTable_Job.Rows[0][4].ToString();
                    jobModel.imagine_job = (byte[])(dataTable_Job.Rows[0][5]);
                    jobModel.descriere_job = dataTable_Job.Rows[0][6].ToString();

                    return View(jobModel);
                }
                else
                    return RedirectToAction("ListaAdmin");
            }
            catch
            {
                return Content("<script language='javascript' type='text/javascript'>alert ('Se pare ca ceva nu a functionat corect');</script>");
            }
        }

        // POST: Job/Editare/5
        [HttpPost]
        public ActionResult Editare(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction("ListaAdmin");
            }
            catch
            {
                return View();
            }
        }

        // GET: Job/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Job/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
    }
}
