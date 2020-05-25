using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Data;
using System.Web.Mvc;

namespace SistemRecrutare.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    //public class SendCodeViewModel
    //{
    //    public string SelectedProvider { get; set; }
    //    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    //    public string ReturnUrl { get; set; }
    //    public bool RememberMe { get; set; }
    //}

    //public class VerifyCodeViewModel
    //{
    //    [Required]
    //    public string Provider { get; set; }

    //    [Required]
    //    [Display(Name = "Cod")]
    //    public string Code { get; set; }
    //    public string ReturnUrl { get; set; }

    //    [Display(Name = "Remember this browser?")]
    //    public bool RememberBrowser { get; set; }

    //    public bool RememberMe { get; set; }
    //}

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    // ANGAJAT

    // Login Angajat
    public class UtilizatorLogin
    {
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Introduceti adresa de email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Introduceti parola")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Parola { get; set; }

        [Display(Name = "Tine-ma minte")]
        public bool TineMinte { get; set; }
    }

    public class Logare
    {
        [Display(Name = "Email:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Introduceti adresa de email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Parola:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Introduceti parola")]
        [DataType(DataType.Password)]
        public string Parola { get; set; }

        [Display(Name = "Retine-ma")]
        public bool TineMinte { get; set; }
    }

    [MetadataType(typeof(ContNouAngajatMetaData))]
    public partial class utilizator  // clasa pentru maparea campului confirma_parola, care nu exista in utilizator.cs
    {
        public string confirma_parola { get; set; }
    }

    // Cont nou Angajat/ Register
    public class ContNouAngajatMetaData  // Cont Aplicant Model Validare 
    {
        [EmailAddress]
        [Display(Name = "Email* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Email este obligatoriu.")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [StringLength(100, ErrorMessage = "Parola trebuie sa aiba cel putin {2} caractere.", MinimumLength = 4)]
        [Display(Name = "Parola* :")]
        [DataType(DataType.Password)] //tip parola
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Parola este obligatoriu.")]
        public string parola { get; set; }

        [DataType(DataType.Password)]  
        [Display(Name = "Confirma parola* :")]
        [System.ComponentModel.DataAnnotations.Compare("parola", ErrorMessage = "Confirma parola. Parolele introduse nu corespund.")]
        public string confirma_parola { get; set; }

        [Display(Name = "Nume* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Nume este obligatoriu.")]
        public string nume_utilizator { get; set; }

        [Display(Name = "Prenume* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Prenume este obligatoriu.")]
        public string prenume_utilizator { get; set; }

        [Display(Name = "Oras* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Oras este obligatoriu.")]
        public string oras { get; set; }

        [Display(Name = "Telefon :")]
        public string telefon { get; set; }

        [Display(Name = "Data nasterii* :")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Data nasterii este obligatoriu.")]
        public DateTime data_nasterii { get; set; }
        [Display(Name = "Sex* :")]
        public Sex_Utilizator sex { get; set; } //from enum

        //[Display(Name = "Domenii in care vreau sa lucrez :")]
        //public List<Domenii_Lucru_Utilizator> Domenii_lucru { get; set; }
        public bool verificare_email { get; set; } //////---
        public Guid cod_activare { get; set; } //////---- 
    }

    public enum Sex_Utilizator
    {  // valori lista Sex Utilizator
        Femeie = 0,
        Barbat = 1,
        Altul = 2
    }

    public class UtilizatorAngajatViewModel // include lista domenii de interes a angajatului
    {
        public utilizator Utilizator { get; set; }
        public SelectList ListaDomenii { get; set; }
        public List<int> DomeniiSelectateIds { get; set; }
    }

    // ANGAJATOR

    [MetadataType(typeof(ContNouAngajatorMetaData))]
    public partial class angajator  // clasa pentru maparea campului confirma_parola, care nu exista in angajator.cs
    {
        public string confirma_parola { get; set; }
    }

    // Cont nou Angajator
    public class ContNouAngajatorMetaData  // Cont Angajator Model Validare 
    {
        [EmailAddress]
        [Display(Name = "Email* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Email este obligatoriu.")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [StringLength(100, ErrorMessage = "Parola trebuie sa aiba cel putin {2} caractere.", MinimumLength = 4)]
        [Display(Name = "Parola* :")]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Parola este obligatoriu.")]
        public string parola { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmati parola* :")]
        [System.ComponentModel.DataAnnotations.Compare("parola", ErrorMessage = "Confirma parola. Parolele introduse nu corespund.")]
        public string confirma_parola { get; set; }

        [Display(Name = "Numele companiei* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Numele companiei este obligatoriu.")]
        public string nume_angajator { get; set; }
        [Display(Name = "Numar de ordine* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Numar de ordine este obligatoriu.")]
        public string nr_ordine_registru_comert { get; set; }
        [Display(Name = "Telefon :")]
        public string telefon { get; set; }
        [Display(Name = "Tara* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Tara este obligatoriu.")]
        public string tara_sediu { get; set; }
        [Display(Name = "Oras* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Oras este obligatoriu.")]
        public string oras_sediu { get; set; }
        [Display(Name = "Adresa* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Adresa este obligatoriu.")]
        public string adresa_sediu { get; set; }
        public bool verificare_email { get; set; } 
        public Guid cod_activare { get; set; } 
    }


    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} trebuie sa aiba cel putin {2} caractere.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirma parola")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Parolele introduse nu corespund.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}
