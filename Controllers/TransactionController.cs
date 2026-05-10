using BankingManegmentSystem.Data;
using BankingManegmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankingManegmentSystem.Controllers
{
    public class TransactionController : Controller
    {

        private readonly AppDbContext _context;
        private string UserId;
        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TransactionController
        public async Task<ActionResult> Index()
        {
            if (!this.User.IsInRole("Admin"))
            {
            int accountId = await GetAccountIdAsync();
            var transactions = await _context.Transactions.Include("Account").Where(x=>x.AccountId == accountId).ToListAsync();
            return View(transactions);
            }
            return View(await _context.Transactions.Include("Account").ToListAsync());
        }

        // GET: TransactionController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0) return NotFound("Id is requard");

            var existingTransactions = await _context.Transactions.Include(x=>x.Account).FirstOrDefaultAsync(x=>x.Id == id);
            if (existingTransactions != null)
            {
                if(existingTransactions.AccountId != await GetAccountIdAsync())
                {
                    return BadRequest("unauthorzied Request");
                }
                return View(existingTransactions);
            }
             return NotFound("Transaction is not found");

        }
      
        private async Task<Account> GetAccountAsync()
        {
            var userAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == AssignUserId());
            return userAccount;
        }
        private async Task<int> GetAccountIdAsync()
        {
            var userAccount = await _context.Accounts.FirstOrDefaultAsync(x=>x.UserId==AssignUserId());
            return userAccount.Id;
        }
        private string AssignUserId()
        {

            UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return UserId;
        }

    }
}
