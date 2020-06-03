using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemRecrutare.Models
{
    public class CVModels
    {  
        [Required(AllowEmptyStrings = false, ErrorMessage = "Selecteaza un fisier")]
        [DataType(DataType.Upload)]
        [Display(Name = "Selecteaza Fisierele")]
        public HttpPostedFileBase fisiere { get; set; }
    }

    public class DetaliiCvModel
    {
        public int id_cv { get; set; }
        [Display(Name = "Fisier Incarcat")]
        public String nume_fisier { get; set; }
        public byte[] continut_fisier { get; set; }


    }
}