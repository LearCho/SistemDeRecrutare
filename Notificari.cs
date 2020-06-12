using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SistemRecrutare.Controllers;
using SistemRecrutare.Models;

namespace SistemRecrutare
{
    public class Notificari
    {
        public void InregistrareNotificari(DateTime data_curenta/*, string angajator*/)
        {
            string conStr = ConfigurationManager.ConnectionStrings["sqlConStringDependencies"].ConnectionString;
            string query = "SELECT cod_job, nume_angajat FROM dbo.aplicare_job WHERE " +
                "data_aplicare > @data_aplicare" /*AND angajator = @angajator;"*/;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@data_aplicare", data_curenta);
                //cmd.Parameters.AddWithValue("@angajator", angajator);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChange;  
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    /////--
                }
            }
        }

        // trimitere notificare de la server la client
         void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //string angajator;

           //  angajator = HttpContext.Current.Application["Nume"].ToString();

            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;
                //notificare client:
                var hubNotificari = GlobalHost.ConnectionManager.GetHubContext<HubNotificari>();
                hubNotificari.Clients.All.notify("aplicat");
                //re-inregistrare notificare
                InregistrareNotificari(DateTime.Now/*, angajator*/);
            }
        }

        public List<aplicare_job> ListareAplicari(DateTime data_dupa/*, string angajator*/)
        {
           // angajator = HttpContext.Current.Application["Nume"].ToString();

            using (DBrecrutare db = new DBrecrutare())
            {
                return db.aplicare_job.Where(ap => (ap.data_aplicare > data_dupa)/* && 
                (ap.angajator == angajator)*/).OrderByDescending(ap => ap.data_aplicare)
                .ToList();
            }

        }


    }
}