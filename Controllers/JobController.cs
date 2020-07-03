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
using System.Net.Mail;
using System.Net;
using Dapper;

namespace SistemRecrutare.Controllers
{
    [Authorize]
    public class JobController : Controller
    {
        public static string connectionString = @"Data Source = (local)\SQLINSTANCE; 
                                       Initial Catalog = DB_sistem_recrutare;
                                       Integrated Security = true";

        // GET: Job/ListaAdmin
        [HttpGet]
        public ActionResult ListaAdmin(string val_cautare, string angajator) // afiseaza joburi in dataTable
        {
            DBrecrutare db = new DBrecrutare();
            HttpContext.Application["Cautare"] = val_cautare;
            angajator = @HttpContext.Application["Nume"].ToString();

            //TODO : de creat pagini cu PagedList
            try
            {         
                var joburi = from job in db.jobs
                             where job.angajator == angajator
                             select job;

                return View(joburi.Where(j => /*(j.angajator.ToUpper() == angajator) &&*/
                (j.cod_job.Contains(val_cautare) || j.denumire_job.Contains(val_cautare) ||
                j.angajator.Contains(val_cautare) || j.tara.Contains(val_cautare) ||
                j.oras.Contains(val_cautare) || j.descriere_job.Contains(val_cautare) ||
                val_cautare == null)).ToList());
            }
            catch
            {
                return View("Error");
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
        //[Authorize(Roles = "Admin, Angajator")]
        public ActionResult Creare()
        {
            return View(new SistemRecrutare.Models.JobModel());
        }

        // POST: Job/Creare
        [HttpPost]
        //[Authorize(Roles = "Admin, Angajator")]
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

                string query = "INSERT INTO dbo.job VALUES(@denumire_job, @cod_job, @data_expirare_job," +
                    " @angajator,  @imagine_job, @descriere_job, @data_creare_job, @tara, @oras, @norma_job)";
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
                sql_cmd.Parameters.AddWithValue("@norma_job", jobModel.norma_job);
                sql_cmd.ExecuteNonQuery();
            }
            }
            catch
            {
                ViewBag.MesajJob = "Completati cu atentie campurile!";
                return View("Error");
            }

            return RedirectToAction("ListaAdmin", "Job", new { angajator = jobModel.angajator });
        }

        // GET: Job/Editare/5
        //[Authorize(Roles = "Admin, Angajator")]
        public ActionResult Editare(string cod_job)
        {
            //using (DBrecrutare db = new DBrecrutare())
            //{
            //    return View(db.jobs.Where(j => j.cod_job == cod_job).FirstOrDefault());
            //}
            try
            {
                Models.JobModel jobModel = new Models.JobModel();

                DataTable dataTable_Job = new DataTable();
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    string query1 = "SELECT id_job, denumire_job AS 'DENUMIRE JOB', cod_job AS 'COD', data_expirare_job AS" +
                        "'DATA EXPIRARE', angajator AS 'ANGAJATOR', imagine_job AS ' ', descriere_job AS 'DESPRE', tara AS" +
                        " 'TARA', oras AS 'ORAS', data_creare_job, norma_job FROM dbo.job WHERE cod_job = @cod_job;";
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
                        jobModel.imagine_job = (byte[])(dataTable_Job.Rows[0][5]); // nu se mai editeaza
                    }
                    jobModel.descriere_job = dataTable_Job.Rows[0][6].ToString();
                    jobModel.tara = dataTable_Job.Rows[0][7].ToString();
                    jobModel.oras = dataTable_Job.Rows[0][8].ToString();
                    if (dataTable_Job.Rows[0][9].ToString() != "")
                    {
                        jobModel.data_creare_job = Convert.ToDateTime(dataTable_Job.Rows[0][9]);
                    }
                    if (dataTable_Job.Rows[0][10] != null)
                    {
                        jobModel.norma_job = (Norma_Job)Enum.Parse(typeof(Norma_Job), dataTable_Job.Rows[0][10].ToString());
                    }
                    return View(jobModel);
                }
                else
                {
                    ViewBag.MesajJob = "Prea multe inregistrari";

                    return View("Error");
                }
            }
            catch
            {
                return View("Error");
            }
        }

        // POST: Job/Editare/5
        [HttpPost]
        //[Authorize(Roles = "Admin, Angajator")]
        public ActionResult Editare(string cod_job, JobModel jobModel/*, FormCollection collection*/)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();

                    string query = "UPDATE dbo.job SET denumire_job = @denumire_job, cod_job = @cod_job," +
                        " data_expirare_job = @data_expirare_job, angajator = @angajator, descriere_job = @descriere_job," +
                        " data_creare_job = @data_creare_job, tara = @tara, oras = @oras, norma_job = @norma_job " +
                        "WHERE cod_job = @cod_job;";
                    SqlCommand sql_cmd = new SqlCommand(query, sqlCon);

                    //sql_cmd.Parameters.AddWithValue("@id_job", id_job);
                    sql_cmd.Parameters.AddWithValue("@denumire_job", jobModel.denumire_job);
                    sql_cmd.Parameters.AddWithValue("@cod_job", jobModel.cod_job);
                    sql_cmd.Parameters.AddWithValue("@data_creare_job", jobModel.data_creare_job);
                    sql_cmd.Parameters.AddWithValue("@data_expirare_job", jobModel.data_expirare_job);
                    sql_cmd.Parameters.AddWithValue("@angajator", jobModel.angajator);
                    sql_cmd.Parameters.AddWithValue("@tara", jobModel.tara);
                    sql_cmd.Parameters.AddWithValue("@oras", jobModel.oras);
                    //sql_cmd.Parameters.AddWithValue("@imagine_job", jobModel.imagine_job);
                    sql_cmd.Parameters.AddWithValue("@descriere_job", jobModel.descriere_job);
                    sql_cmd.Parameters.AddWithValue("@norma_job", jobModel.norma_job);

                    sql_cmd.ExecuteNonQuery();
                }
                return RedirectToAction("ListaAdmin", "Job", new { angajator = jobModel.angajator });
            }
            catch
            {
                ViewBag.MesajJob = "Completati cu atentie campurile!";
                return View("Error");
            }
        }

        // GET: Job/Delete/5 
        //[Authorize(Roles = "Admin, Angajator")]
        public ActionResult Stergere(string cod_job)
        {
            try
            {
                JobModel jobModel = new JobModel();
                DataTable dataTable_Angajator = new DataTable();

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();

                    string query = "DELETE FROM dbo.job WHERE cod_job = @cod_job;";
                    string query_angajator = "SELECT angajator FROM dbo.job WHERE cod_job = @cod_job;";

                    SqlCommand sql_cmd = new SqlCommand(query, sqlCon);
                    sql_cmd.Parameters.AddWithValue("@cod_job", cod_job);

                    SqlDataAdapter sqlData = new SqlDataAdapter(query_angajator, sqlCon);
                    sqlData.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData.Fill(dataTable_Angajator);

                    if (dataTable_Angajator.Rows.Count == 1)
                    {
                        jobModel.angajator = dataTable_Angajator.Rows[0][0].ToString();
                    }
                    sql_cmd.ExecuteNonQuery();
                }

                return RedirectToAction("ListaAdmin", "Job", new { angajator = jobModel.angajator });
            }
            catch
            {
                return View("Error");
            }
        }

        //Job/Cautare 
        public JsonResult Cautare(string CautaDupa, string ValCautare)
        {
            List<job> ListaJoburi = new List<job>();
            DBrecrutare db = new DBrecrutare();

            ListaJoburi = db.jobs.Where(j => j.cod_job.Contains(ValCautare) ||
            j.denumire_job.Contains(ValCautare) || j.descriere_job.Contains(ValCautare) ||
             j.angajator.Contains(ValCautare) || j.tara.Contains(ValCautare) ||
             j.oras.Contains(ValCautare) || ValCautare == null).ToList();
            return Json(ListaJoburi, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
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

                    for (int i = 0; i < dataTable_Job.Rows.Count; i++)
                    {
                        if (dataTable_Job.Rows[i][10].ToString() == "0")
                        {
                            dataTable_Job.Rows[i][10] = "Fulltime";
                        }
                        if (dataTable_Job.Rows[i][10].ToString() == "1")
                        {
                            dataTable_Job.Rows[i][10] = "Partime";
                        }
                        if (dataTable_Job.Rows[i][10].ToString() == "2")
                        {
                            dataTable_Job.Rows[i][10] = "Remote";
                        }
                        if (dataTable_Job.Rows[i][10].ToString() == "3")
                        {
                            dataTable_Job.Rows[i][10] = "Internship";
                        }
                        if (dataTable_Job.Rows[i][10].ToString() == "4")
                        {
                            dataTable_Job.Rows[i][10] = "Practica";
                        }
                    }
                }
            }
            catch (SqlException exc)
            {
                throw new InvalidOperationException("Datele nu au putut fi citite.", exc);
            }

            return View(dataTable_Job);
        }

        [AllowAnonymous]
        // GET: Job/ListaUtilizatorCauta
        [HttpGet]
        public ActionResult ListaUtilizatorCauta(string val_cautare) //
        {
            //DataTable dataTable_Job = new DataTable();
            //try
            //{
            //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
            //    {
            //        sqlCon.Open();
            //        SqlDataAdapter sqlDataA;
            //        if (val_cautare == "")
            //        {
            //            sqlDataA = new SqlDataAdapter("SELECT * FROM dbo.job", sqlCon);
            //        }
            //        else
            //        {
            //            sqlDataA = new SqlDataAdapter("SELECT * FROM dbo.job WHERE job.cod_job = '" + val_cautare +
            //            "' OR job.denumire_job = '" + val_cautare + "' OR job.descriere_job = '" + val_cautare + "'", sqlCon);
            //        }
            //        sqlDataA.Fill(dataTable_Job);
            //    }
            //}
            //catch (SqlException exc)
            //{
            //    throw new InvalidOperationException("Datele nu au putut fi citite.", exc);
            //}

            //return View(dataTable_Job);

            DBrecrutare db = new DBrecrutare();
            HttpContext.Application["Cauta"] = val_cautare;

            try
            {
                return View(db.jobs.Where(j => (j.cod_job.Contains(val_cautare) ||
                j.denumire_job.Contains(val_cautare) || j.angajator.Contains(val_cautare) ||
                j.tara.Contains(val_cautare) || j.oras.Contains(val_cautare) ||
                j.descriere_job.Contains(val_cautare) || val_cautare == null)).ToList());
            }
            catch
            {
                return View("Error");
            }
        }

        [AllowAnonymous]
        public ActionResult AplicaLaJob(string cod_job)
        {
            //cod_job = HttpContext.Application["Cod"].ToString();

            try
            {
                JobModel jobModel = new JobModel();
                DataTable dataTable_Job = new DataTable();

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();

                    string query = "SELECT * FROM dbo.job WHERE cod_job = @cod_job;";

                    SqlDataAdapter sqlData = new SqlDataAdapter(query, sqlCon);
                    sqlData.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData.Fill(dataTable_Job);

                    if (dataTable_Job.Rows.Count == 1)
                    {
                        jobModel.cod_job = dataTable_Job.Rows[0][2].ToString();

                        if (dataTable_Job.Rows[0][10].ToString() == "0")
                        {
                            dataTable_Job.Rows[0][10] = "Fulltime";
                        }
                        if (dataTable_Job.Rows[0][10].ToString() == "1")
                        {
                            dataTable_Job.Rows[0][10] = "Partime";
                        }
                        if (dataTable_Job.Rows[0][10].ToString() == "2")
                        {
                            dataTable_Job.Rows[0][10] = "Remote";
                        }
                        if (dataTable_Job.Rows[0][10].ToString() == "3")
                        {
                            dataTable_Job.Rows[0][10] = "Internship";
                        }
                        if (dataTable_Job.Rows[0][10].ToString() == "4")
                        {
                            dataTable_Job.Rows[0][10] = "Practica";
                        }
                    }
                }
                return View(dataTable_Job);
            }
            catch
            {
                return View("Error");
            }
        }

        // trimitere aplicatie prin email catre angajator
        public ActionResult TrimiteNotificareAplicareEmail(string cod_job)
        {
            angajator angajatorModel = new angajator();
            aplicare_job aplicareModel = new aplicare_job();

            DataTable dataTable_verificare = new DataTable();
            DataTable dataTable_angajat = new DataTable();
            DataTable dataTable_angajator = new DataTable();
            DataTable dataTable_mail = new DataTable();
            string email = HttpContext.Application["Email"].ToString();
            HttpContext.Application["Cod"] = cod_job;

            try
            {
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    //verificare daca a aplicat deja la jobul curent
                    string query_verificare = "SELECT aplicat FROM dbo.aplicare_job WHERE email_angajat = @email_angajat " +
                        "AND cod_job = @cod_job;";
                    SqlDataAdapter sqlData00 = new SqlDataAdapter(query_verificare, sqlCon);
                    sqlData00.SelectCommand.Parameters.AddWithValue("@email_angajat", email);
                    sqlData00.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData00.Fill(dataTable_verificare);

                    if (dataTable_verificare.Rows.Count > 0)
                    {
                        if (Convert.ToInt16(dataTable_verificare.Rows[0][0]) == 1)
                        {
                            ViewBag.MesajAplicare = "Ai aplicat deja la acest job!!";
                            return View("Error");
                        }
                    }

                    // angajat
                    string query_Angajat = "SELECT prenume_utilizator, nume_utilizator FROM dbo.utilizator WHERE " +
                        "email = @email;";
                    SqlDataAdapter sqlData0 = new SqlDataAdapter(query_Angajat, sqlCon);
                    sqlData0.SelectCommand.Parameters.AddWithValue("@email", email);
                    sqlData0.Fill(dataTable_angajat);

                    if (dataTable_angajat.Rows.Count == 1)
                    {
                        aplicareModel.nume_angajat = dataTable_angajat.Rows[0][0].ToString() + " " +
                            dataTable_angajat.Rows[0][1].ToString();
                    }

                    // angajator
                    string query_numeAngajator = "SELECT angajator, cod_job FROM dbo.job WHERE cod_job = @cod_job;";
                    SqlDataAdapter sqlData1 = new SqlDataAdapter(query_numeAngajator, sqlCon);
                    sqlData1.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData1.Fill(dataTable_angajator);

                    if (dataTable_angajator.Rows.Count == 1)
                    {
                        //angajatorModel.nume_angajator = dataTable_angajator.Rows[0][0].ToString();
                        aplicareModel.angajator = dataTable_angajator.Rows[0][0].ToString();
                        aplicareModel.cod_job = dataTable_angajator.Rows[0][1].ToString();
                    }

                    string query_email = "SELECT email FROM dbo.angajator WHERE nume_angajator = @nume_angajator;";

                    SqlDataAdapter sqlData2 = new SqlDataAdapter(query_email, sqlCon);
                    sqlData2.SelectCommand.Parameters.AddWithValue("@nume_angajator", aplicareModel.angajator);
                    sqlData2.Fill(dataTable_mail);

                    if (dataTable_mail.Rows.Count == 1)
                    {
                        angajatorModel.email = dataTable_mail.Rows[0][0].ToString();
                    }
                }

                var email_deLa = new MailAddress("sis.rec.utcb@gmail.com", "Sistem Recrutare");
                var email_pentru = new MailAddress(angajatorModel.email);
                var email_deLa_parola = "licenta2020"; // parola actuala
                string email_titlu = "Cineva a aplicat la locul de muncă postat de tine!";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(email_deLa.Address, email_deLa_parola)
                };

                var url_detalii_aplicatie = "/Job/Detalii/?cod_job=" + cod_job + "&email=" + email;
                var link_job = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url_detalii_aplicatie);

                string email_body = "<br/><br/>Cineva a aplicat recent la un anunt postat de tine.<br/>Pentru a vedea " +
                    "cv-ul și detaliile apasă <a href = '" + link_job + "'>aici</a> ";

                using (var mesaj_aplicare = new MailMessage(email_deLa, email_pentru)
                {
                    Subject = email_titlu,
                    Body = email_body,
                    IsBodyHtml = true
                })
                    smtp.Send(mesaj_aplicare);

                ViewBag.Message = "\tAi aplicat cu succes la acest loc de muncă!\n Angajatorul va fi " +
                    "notificat în scurt timp.";

                aplicareModel.data_aplicare = DateTime.Now;
                aplicareModel.email_angajat = email;
                aplicareModel.aplicat = 1;

                // salvare campuri in aplicare_job din bd 
                using (DBrecrutare db = new DBrecrutare())
                {
                    db.aplicare_job.Add(aplicareModel);
                    db.SaveChanges();
                }

                return View(dataTable_mail);
            }
            catch
            {
                return View("Error");
            }
        }

        // detalii aplicatie - vedere angajator dupa primire notificare
        public ActionResult Detalii(string cod_job, string email)
        {
            HttpContext.Application["Email"] = email;

            try
            {
                JobModel jobModel = new JobModel();
                DataTable dataTable_Job = new DataTable();

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();

                    string query = "SELECT * FROM dbo.job inner join dbo.aplicare_job ON " +
                        "job.cod_job = aplicare_job.cod_job WHERE job.cod_job = @cod_job AND " +
                        "aplicare_job.email_angajat = @email_angajat;";

                    SqlDataAdapter sqlData = new SqlDataAdapter(query, sqlCon);
                    sqlData.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData.SelectCommand.Parameters.AddWithValue("@email_angajat", email);
                    sqlData.Fill(dataTable_Job);

                    if (dataTable_Job.Rows.Count == 1)
                    {
                        jobModel.cod_job = dataTable_Job.Rows[0][2].ToString();

                        if (dataTable_Job.Rows[0][10].ToString() == "0")
                        {
                            dataTable_Job.Rows[0][10] = "Fullime";
                        }
                        if (dataTable_Job.Rows[0][10].ToString() == "1")
                        {
                            dataTable_Job.Rows[0][10] = "Partime";
                        }
                        if (dataTable_Job.Rows[0][10].ToString() == "2")
                        {
                            dataTable_Job.Rows[0][10] = "Remote";
                        }
                        if (dataTable_Job.Rows[0][10].ToString() == "3")
                        {
                            dataTable_Job.Rows[0][10] = "Internship";
                        }
                        if (dataTable_Job.Rows[0][10].ToString() == "4")
                        {
                            dataTable_Job.Rows[0][10] = "Practica";
                        }
                    }
                }
                return View(dataTable_Job);
            }
            catch
            {
                return View("Error");
            }
        }

        public ActionResult AprobaAplicatie(/*string cod_job,*/ string email)
        {
            try
            {
                //  email = HttpContext.Application["Email"].ToString();
                email = HttpContext.Application["Email"].ToString();
                aplicare_job aplicareModel = new aplicare_job();
                DataTable dataTable_Aplicare = new DataTable();

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();

                    string query = "SELECT distinct email_angajat, nume_angajat, angajator FROM dbo.aplicare_job WHERE " +
                        "aplicare_job.email_angajat = @email_angajat;";

                    SqlDataAdapter sqlData = new SqlDataAdapter(query, sqlCon);
                    //sqlData.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData.SelectCommand.Parameters.AddWithValue("@email_angajat", email);
                    sqlData.Fill(dataTable_Aplicare);

                    if (dataTable_Aplicare.Rows.Count == 1)
                    {
                        aplicareModel.email_angajat = dataTable_Aplicare.Rows[0][0].ToString();
                        aplicareModel.nume_angajat = dataTable_Aplicare.Rows[0][1].ToString();
                        aplicareModel.angajator = dataTable_Aplicare.Rows[0][2].ToString();
                    }

                    var email_deLa = new MailAddress("sis.rec.utcb@gmail.com", "Sistem Recrutare");
                    var email_pentru = new MailAddress(email);
                    var email_deLa_parola = "licenta2020"; // parola actuala
                    string email_titlu = "Aplicatia ta a primit un raspuns!";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(email_deLa.Address, email_deLa_parola)
                    };

                    string email_body = "<br/><br/><p>Felicitari!</p> <p>Angajatorul <b> " + aplicareModel.angajator + "</b> ti-a evaluat " +
                        "aplicatia si urmeaza sa fi contactat pentru urmatorul pas.</p><p> Stai cu ochii pe telefon si pe email!</p><br/> " +
                        "<br/><br/><b>Platforma de Joburi</b><br/>Acesta este un email trimis automat. Angajatorul nu poate fi contactat pe această adresă.";                   
                    using (var mesaj_aplicare = new MailMessage(email_deLa, email_pentru)
                    {
                        Subject = email_titlu,
                        Body = email_body,
                        IsBodyHtml = true
                    })
                        smtp.Send(mesaj_aplicare);

                    ViewBag.Message = "\tRăspunsul a fost trimis. Puteți programa un interviu pentru candidatul selectat.";
                }
                return View(dataTable_Aplicare);
        }
            catch
            {
                return View("Error");
            }
        }

        public ActionResult RespingeAplicatie(/*string cod_job,*/ string email)
        {
            try
            {
                //  email = HttpContext.Application["Email"].ToString();
                email = HttpContext.Application["Email"].ToString();
                aplicare_job aplicareModel = new aplicare_job();
                DataTable dataTable_Aplicare = new DataTable();

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();

                    string query = "SELECT distinct email_angajat, nume_angajat, angajator FROM dbo.aplicare_job WHERE " +
                        "aplicare_job.email_angajat = @email_angajat;";

                    SqlDataAdapter sqlData = new SqlDataAdapter(query, sqlCon);
                    //sqlData.SelectCommand.Parameters.AddWithValue("@cod_job", cod_job);
                    sqlData.SelectCommand.Parameters.AddWithValue("@email_angajat", email);
                    sqlData.Fill(dataTable_Aplicare);

                    if (dataTable_Aplicare.Rows.Count == 1)
                    {
                        aplicareModel.email_angajat = dataTable_Aplicare.Rows[0][0].ToString();
                        aplicareModel.nume_angajat = dataTable_Aplicare.Rows[0][1].ToString();
                        aplicareModel.angajator = dataTable_Aplicare.Rows[0][2].ToString();
                    }

                    var email_deLa = new MailAddress("sis.rec.utcb@gmail.com", "Sistem Recrutare");
                    var email_pentru = new MailAddress(email);
                    var email_deLa_parola = "licenta2020"; // parola actuala
                    string email_titlu = "Aplicatia ta a primit un raspuns!";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(email_deLa.Address, email_deLa_parola)
                    };

                    string email_body = "<br/><br/>Salut!<p> Din pacate, angajatorul <b> " + aplicareModel.angajator + "</b> ti-a evaluat " +
                        "aplicatia, alegand sa nu mearga mai departe cu ea in procesul de recrutare.</p><p> Nu te descumpani! Continua sa iti " +
                        "imbunatatesti cv-ul, nu uita sa adaugi o scrisoare de intentie pentru a-ti creste sansele de selectie si aplica, " +
                        "aplica, aplica! la cele mai noi locuri de munca de pe patforma noastra.</p><p>Iti uram succes!</p><br/> " +
                        "<br/><br/><b>Platforma de Joburi</b><br/>Acesta este un email trimis automat. Angajatorul nu poate fi contactat pe această adresă.";
                    using (var mesaj_aplicare = new MailMessage(email_deLa, email_pentru)
                    {
                        Subject = email_titlu,
                        Body = email_body,
                        IsBodyHtml = true
                    })
                        smtp.Send(mesaj_aplicare);

                    ViewBag.Message = "\tAi respins aplicația candidatului. Răspunsul a fost trimis cu succes!";
                }
                return View(dataTable_Aplicare);
            }
            catch
            {
                return View("Error");
            }
        }
    }
}
