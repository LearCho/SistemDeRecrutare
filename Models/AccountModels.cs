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

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Cod")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [Display(Name = "Iti amintesti de mine?")]
        public bool RememberMe { get; set; }
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

    //public class Sex_Utilizator
    //{
    //    public int Id_Sex { get; set; }
    //    [Display(Name = "Sex* :")]
    //   // [Required(ErrorMessage = "Campul Sex este obligatoriu.")]
    //    public string Denumire_Sex { get; set; }
    //}


    public enum Sex_Utilizator
    {  // valori lista dropDown Sex Utilizator
        Femeie = 0,
        Barbat = 1,
        Altul = 2
    }

    public class UtilizatorViewModel // include lista domenii de interes
    {
        public utilizator Utilizator { get; set; }
        public SelectList ListaDomenii { get; set; }
        public List<int> DomeniiSelectateIds { get; set; }
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
