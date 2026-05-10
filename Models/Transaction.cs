namespace BankingManegmentSystem.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransctionDateTime { get; set; }

        public Account Account { get; set; }

    }
        public enum TransactionType
        {
            Withdraw,
            Deposit
        }
}
