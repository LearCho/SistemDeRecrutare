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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Denumire Job este obligatoriu.")]
        public string denumire_job {get; set;}
        [DisplayName("Cod")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Cod este obligatoriu.")]
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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Angajator este obligatoriu.")]
        public string angajator { get; set; }
        [DisplayName("Tara")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Tara este obligatoriu.")]
        public string tara { get; set; }
        [DisplayName("Oras")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Oras este obligatoriu.")]
        public string oras { get; set; }
        [DisplayName("Norma")]
        public Norma_Job norma_job { get; set; }
        [DisplayName("Logo")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Imagine Descriptiva este obligatoriu.")]
        public byte[] imagine_job { get; set; }
       // public HttpPostedFileBase cale_imagine_job { get; set; }
        [DisplayName("Despre Job")]
        public string descriere_job { get; set; }
    }

    public enum Norma_Job
    {  // valori lista Norma Job
        Fulltime,
        Partime,
        Remote,
        Internship,
        Practica
    }
}