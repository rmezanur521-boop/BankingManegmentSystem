using BankingManegmentSystem.Models;

public class CustomerDashboardVM
{
    public decimal MyTotalBalance { get; set; }
    public int TotalTransactions { get; set; }
    public int TotalDepositCount { get; set; }
    public int TotalWithdrawCount { get; set; }

    public List<Transaction> RecentTransactions { get; set; }
}