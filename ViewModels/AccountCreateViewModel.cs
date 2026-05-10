using BankingManegmentSystem.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankingManegmentSystem.ViewModels
{
    public class AccountCreateViewModel
    {
        [Required]
        [EmailAddress]
        [DisplayName("Email address")]
        public string Email { get; set; }
        [Required]
        [DisplayName("Password")]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password do not match" )]

        public string ConfirmationPassword { get; set; }

        //Account

        [Required]
        [DisplayName("Enter account number")]
        public string AccountNumber { get; set; }
        [Required]
        [DisplayName("Opening Balanc")]
        public decimal OpeningBalance { get; set; }
        [Required]
        [DisplayName("Enter account Name")]
        public string AccountName { get; set; }

        [Required]
        [DisplayName("Status")]
        public AccountStatus Status { get; set; }
    }
}
