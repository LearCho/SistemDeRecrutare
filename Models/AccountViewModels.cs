using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

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

     // Cont nou / Register
    public class ContNouViewModel   // Inregistrare Utilizator
    {
        [EmailAddress]
        [Display(Name = "Email* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul E-mail este obligatoriu.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "{0} trebuie sa aiba cel putin {2} caractere.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Parola este obligatoriu.")]
        public string Parola { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Rescrie parola trebuie sa contina parola setata anterior.")]
        [DataType(DataType.Password)]
        [Display(Name = "Rescrie parola* :")]
        [Compare("Parola", ErrorMessage = "Parolele introduse nu corespund.")]
        public string ConfirmaParola { get; set; }

        [Display(Name = "Prenume* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Prenume este obligatoriu.")]
        public string Prenume { get; set; }

        [Display(Name = "Nume* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Nume este obligatoriu.")]
        public string Nume { get; set; }

        [Display(Name = "Oras* :")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Oras este obligatoriu.")]
        public string Oras { get; set; }

        [Display(Name = "Telefon :")]
        public string Telefon { get; set; }

        [Display(Name = "Data nasterii* :")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campul Data nasterii este obligatoriu.")]
        public DateTime Data_nasterii { get; set; }

        [Display(Name = "Sex* :")]
        [Required(ErrorMessage = "Campul Sex este obligatoriu.")]
        public Sex_Utilizator Sex { get; set; }

        [Display(Name = "Domenii in care vreau sa lucrez :")]
        public string Domenii_lucru { get; set; }
    }

    public enum Sex_Utilizator {  // valori lista dropDown
        Barbat,
        Femeie,
        Altul
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
        [Compare("Password", ErrorMessage = "Parolele introduse nu corespund.")]
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
