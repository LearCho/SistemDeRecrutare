using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using System.Windows;

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
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDataA = new SqlDataAdapter("SELECT * FROM dbo.job", sqlCon);
                sqlDataA.Fill(dataTable_Job);
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
                    string query = "INSERT INTO dbo.job VALUES(@denumire_job, @cod_job, @data_expirare_job)";
                    SqlCommand sql_cmd = new SqlCommand(query, sqlCon);
                    //DataSet dataSet = new DataSet();
                    //DataTable dataTable;
                    //dataTable = dataSet.Tables["Joburi"];
                    //sql_cmd.Parameters.AddWithValue("@id_job", id_job);
                    sql_cmd.Parameters.AddWithValue("@denumire_job", jobModel.denumire_job);
                    sql_cmd.Parameters.AddWithValue("@cod_job", jobModel.cod_job);
                    sql_cmd.Parameters.AddWithValue("@data_expirare_job", jobModel.data_expirare);
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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Job/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
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
