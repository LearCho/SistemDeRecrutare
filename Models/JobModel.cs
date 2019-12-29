using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace SistemRecrutare.Models
{
    public class JobModel
    {
        public int nrcrt { get; set; }
        [DisplayName("Denumire Job")]
        public string denumire_job {get; set;}
        [DisplayName("Cod")]
        public string cod_job {get; set;}
        [DisplayName("Data expirare")]
        public string data_expirare {get; set;}
    }
}