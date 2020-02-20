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

namespace SistemRecrutare.Controllers
{
    public class JobController : Controller
    {
        string connectionString = @"Data Source = (local)\SQLINSTANCE; 
                                  Initial Catalog = DB_sistem_recrutare;
                                  Integrated Security = true";

        // GET: Job/Lista
        [HttpGet]
        public ActionResult Lista()
        {
            DataTable dataTable_Job = new DataTable();
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

        // GET: Job/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query = "INSERT INTO dbo.job VALUES(@denumire_job, @cod_job, @data_expirare_job, @angajator," +
                        " @imagine_job, @descriere_job)";
                    SqlCommand sql_cmd = new SqlCommand(query, sqlCon);
                    //DataSet dataSet = new DataSet();
                    //DataTable dataTable;
                    //dataTable = dataSet.Tables["Joburi"];
                    //sql_cmd.Parameters.AddWithValue("@id_job", id_job);
                    sql_cmd.Parameters.AddWithValue("@denumire_job", jobModel.denumire_job);
                    sql_cmd.Parameters.AddWithValue("@cod_job", jobModel.cod_job);
                    sql_cmd.Parameters.AddWithValue("@data_expirare_job", jobModel.data_expirare);
                    sql_cmd.Parameters.AddWithValue("@angajator", jobModel.angajator);
                    sql_cmd.Parameters.AddWithValue("@imagine_job", jobModel.imagine_job);
                    sql_cmd.Parameters.AddWithValue("@descriere_job", jobModel.descriere_job);

                    sql_cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Lista");
            }
            catch
            {
                return Content("<script language='javascript' type='text/javascript'>alert ('A aparut o eroare la introducerea datelor! ');</script>");
            }
        }

        // GET: Job/Edit/5
        public ActionResult Edit(string cod_job = "")
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
                        " dbo.job WHERE cod_job=@cod_job";
                    SqlDataAdapter sqlData = new SqlDataAdapter(query1, sqlCon);
                    sqlData.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData.Fill(dataTable_Job);
                }

                if (dataTable_Job.Rows.Count == 1)
                {
                    jobModel.id_job = Convert.ToInt32(dataTable_Job.Rows[0][0].ToString());
                    jobModel.denumire_job = dataTable_Job.Rows[0][1].ToString();
                    jobModel.cod_job = dataTable_Job.Rows[0][2].ToString();
                    jobModel.data_expirare = DateTime.Parse(dataTable_Job.Rows[0][3].ToString()).ToShortDateString();
                    jobModel.angajator = dataTable_Job.Rows[0][4].ToString();
                    jobModel.imagine_job = (byte[])(dataTable_Job.Rows[0][5]);
                    jobModel.descriere_job = dataTable_Job.Rows[0][6].ToString();

                    return View(jobModel);
                }
                else
                    return RedirectToAction("Lista");
            }
            catch
            {
                return Content("<script language='javascript' type='text/javascript'>alert ('Ceva nu a functionat corect!');</script>");
            }
        }

        // POST: Job/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction("Lista");
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
    }
}
