using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security;
using System.Windows;
using System.Web.Optimization;


namespace SistemRecrutare
{
    public class SqlController
    {
        public static bool dataBaseConnect()
        {
            string connectionString = @"Data Source = (local)\SQLINSTANCE; 
                                  Initial Catalog = DB_sistem_recrutare;
                                  Integrated Security = true";

            SqlConnection sqlCon = new SqlConnection(connectionString);

            try
            {                
                sqlCon.Open();
            }
            catch (SqlException exc)
            {
                throw new InvalidOperationException("Datele nu au putut fi citite.", exc);
            }

            if (sqlCon.State == System.Data.ConnectionState.Open)
                return true;
            else
                return false;
        }
    }
}

