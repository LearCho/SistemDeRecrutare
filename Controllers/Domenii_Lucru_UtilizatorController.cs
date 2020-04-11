using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemRecrutare.Models;

namespace SistemRecrutare.Controllers
{
    //public class Domenii_Lucru_UtilizatorController : Controller
    //{
    //    //Arata domenii de lucru
    //    public ActionResult Arata_Domenii_Utilizator()
    //    {
    //        DBrecrutare db = new DBrecrutare();
    //        Domenii_Lucru_Utilizator domeniu = new Domenii_Lucru_Utilizator();
    //        domeniu.GetListaDomenii = db.domeniu_lucru.Select(x => new Domenii_Lucru_Utilizator
    //        {
    //            id_domeniu = x.id_domeniu,
    //            denumire_domeniu = x.denumire_domeniu
    //        }).ToList();
          
    //            return View(domeniu);            
    //    }

    //    [HttpPost]
    //    public ActionResult Arata_Domenii_Utilizator(Domenii_Lucru_Utilizator domeniu)
    //    {          
    //        return RedirectToAction("Arata_Domenii_Utilizator");
    //    }


    //    //public static List<Domenii_Lucru_Utilizator> Arata_Lista_Domenii_Utilizator()
    //    //{
    //    //    List<Domenii_Lucru_Utilizator> Domenii = new List<Domenii_Lucru_Utilizator>();
    //    //    using (SqlConnection sqlCon = new SqlConnection(JobController.connectionString))
    //    //    {
    //    //        using (SqlCommand query = new SqlCommand("SELECT denumire_domeniu FROM dbo.domeniu_lucru"))
    //    //        {
    //    //            using (SqlDataAdapter sqlDataA = new SqlDataAdapter())
    //    //            {
    //    //                query.Connection = sqlCon;
    //    //                sqlCon.Open();
    //    //                sqlDataA.SelectCommand = query;
    //    //                SqlDataReader sdr = query.ExecuteReader();
    //    //                while (sdr.Read())
    //    //                {
    //    //                    Domenii_Lucru_Utilizator domeniu = new Domenii_Lucru_Utilizator();
    //    //                    domeniu.denumire_domeniu = sdr["denumire_domeniu"].ToString();
    //    //                    Domenii.Add(domeniu);
    //    //                }
    //    //            }
    //    //            return Domenii;
    //    //        }
    //    //    }
    //    //}
    //}
}