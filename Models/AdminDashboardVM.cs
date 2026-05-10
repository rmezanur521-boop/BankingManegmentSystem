using BankingManegmentSystem.Models;

public class AdminDashboardVM
{
    public int TotalAccounts { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalDeposit { get; set; }
    public decimal TotalWithdraw { get; set; }
    public decimal TotalBankBalance { get; set; }

    public List<Account> RecentAccounts { get; set; }
    public List<Transaction> RecentTransactions { get; set; }
}