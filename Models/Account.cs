using BankingManegmentSystem.Constants;
using System.ComponentModel.DataAnnotations;

namespace BankingManegmentSystem.Models
{
    public class Account
    {

        public int Id { get; set; }
        [StringLength(50)]
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string UserId { get; set; }
        [StringLength(50)]
        public string AccountName {  get; set; }
        public AccountStatus Status { get; set; }

        public IEnumerable<Transaction> Transaction { get; set; }

        
    }
}
