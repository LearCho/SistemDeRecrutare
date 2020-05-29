using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SistemRecrutare.Models
{
    public class JobModel
    {
        public int nrcrt { get; set; }
        public int id_job { get; set; }
        [DisplayName("Denumire Job")]
        public string denumire_job {get; set;}
        [DisplayName("Cod")]
        public string cod_job {get; set;}
        [DisplayName("Data creare")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]

        public Nullable<System.DateTime> data_creare_job { get; set; }
        [DisplayName("Data expirare(limita)")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> data_expirare_job { get; set; }
        [DisplayName("Angajator")]
        public string angajator { get; set; }
        [DisplayName("Tara")]
        public string tara { get; set; }
        [DisplayName("Oras")]
        public string oras { get; set; }
        [DisplayName("Imagine Descriptiva")]
        public byte[] imagine_job { get; set; }
       // public HttpPostedFileBase cale_imagine_job { get; set; }
        [DisplayName("Despre Job")]
        public string descriere_job { get; set; }
    }
}