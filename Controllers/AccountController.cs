using BankingManegmentSystem.Data;
using BankingManegmentSystem.Helper;
using BankingManegmentSystem.Models;
using BankingManegmentSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankingManegmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        //[Route("/dashboard")]
        public async Task<IActionResult> AdminDashboard()
        {
            var totalAccounts = await _context.Accounts.CountAsync();
            var totalTransactions = await _context.Transactions.CountAsync();

            var totalDeposit = await _context.Transactions
                .Where(x => x.TransactionType == TransactionType.Deposit)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            var totalWithdraw = await _context.Transactions
                .Where(x => x.TransactionType == TransactionType.Withdraw)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;

            var totalBankBalance = await _context.Accounts
                .SumAsync(x => (decimal?)x.Balance) ?? 0;

            var recentAccounts = await _context.Accounts
                .OrderByDescending(x => x.Id)
                .Take(5)
                .ToListAsync();

            var recentTransactions = await _context.Transactions
                .Include(x => x.Account)
                .OrderByDescending(x => x.TransctionDateTime)
                .Take(5)
                .ToListAsync();

            var vm = new AdminDashboardVM
            {
                TotalAccounts = totalAccounts,
                TotalTransactions = totalTransactions,
                TotalDeposit = totalDeposit,
                TotalWithdraw = totalWithdraw,
                TotalBankBalance = totalBankBalance,
                RecentAccounts = recentAccounts,
                RecentTransactions = recentTransactions
            };

            return View(vm);
        }

        [Authorize(Roles = ("Customer"))]
        //[Route("/dashboardCustomer")]
        public async Task<IActionResult> CustomerDashboard()
        {
            var account = new Account();
            string userId = this.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            account = userId.GetUserAccountAsync(_context).Result;
            

            if (account == null)
                return RedirectToAction("AccountView");

            var transactions = await _context.Transactions
                .Where(x => x.AccountId == account.Id)
                .OrderByDescending(x => x.TransctionDateTime)
                .ToListAsync();

            var vm = new CustomerDashboardVM
            {
                MyTotalBalance = account.Balance,
                TotalTransactions = transactions.Count,
                TotalDepositCount = transactions.Count(x => x.TransactionType == TransactionType.Deposit),
                TotalWithdrawCount = transactions.Count(x => x.TransactionType == TransactionType.Withdraw),
                RecentTransactions = transactions.Take(5).ToList()
            };

            return View(vm);
        }
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Index()
        {
                return View(await _context.Accounts.ToListAsync());
            
        }
        [Route("/Welcome")]
        public IActionResult NewUser()
        {
            return View();
        }

        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.Include(x => x.Transaction)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [Authorize(Roles = "Customer")]
        [Route("/Account/View/{accountNumber}")]
        public async Task<IActionResult> AccountView(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var account = await _context.Accounts
                .Include(a => a.Transaction.OrderByDescending(t => t.TransctionDateTime))
                .FirstOrDefaultAsync(a =>
                    a.AccountNumber == accountNumber &&
                    a.UserId == userId);

            if (account == null)
                return NotFound();

            return View(account);
        }

        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Create(AccountCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = await _context.Accounts.Where(x => x.AccountNumber == model.AccountNumber).FirstOrDefaultAsync();
                if (existingAccount != null)
                {
                    ModelState.AddModelError("", $" Account is already exists!");
                    return View(model);
                }
                //user create
                var user = new IdentityUser() {
                    UserName= model.Email,
                    Email = model.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.IsInRoleAsync(user, "Customer");
                }
                else
                {
                    ModelState.AddModelError("", $"User createton Faild! {string.Join(", ", result.Errors.Select(x=>x.Description))}");
                    return View(model);
                }
                //Account cterate
                var account = new Account
                {
                    AccountName = model.AccountName,
                    AccountNumber = model.AccountNumber,
                    Balance = model.OpeningBalance,
                    UserId = user.Id,
                    Status = model.Status
                };
                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();
                //add Transction
                var transction = new Transaction
                {
                    AccountId = account.Id,
                    Amount = model.OpeningBalance,
                    TransactionType = TransactionType.Deposit,
                    TransctionDateTime = DateTime.UtcNow
                };
                await _context.Transactions.AddAsync(transction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            await BuildUserSelectAsync();
            return View(account);
        }

        // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountNumber,Balance,UserId,AccountName,Status")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
        [Authorize(Roles = ("Admin, Customer"))]
        public async Task<IActionResult> Deposit()
        {
            await BuildAccountSelectAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin, Customer"))]
        public async Task<IActionResult> Deposit(decimal amount, string accountNumber)
        {
            if (amount < 500)
            {
                ModelState.AddModelError("", "Deposit amount must be 500");
                return View();
            }
            var account = new Account();
            if (User.IsInRole("Admin") && !string.IsNullOrEmpty(accountNumber))
            {
                account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
            }
            else
            {
                string userId = this.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                account = userId.GetUserAccountAsync(_context).Result;
            }
            if (account != null)
            {
                account.Balance = account.Balance + amount;
                _context.Update(account);
                await _context.SaveChangesAsync();

                var transaction = new Transaction
                {
                    AccountId = account.Id,
                    Amount = amount,
                    TransactionType = TransactionType.Deposit,
                    TransctionDateTime = DateTime.UtcNow
                };
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(Index));
                }
                return Redirect($"/Account/View/{account.AccountNumber}");
            }
            ModelState.AddModelError(" ", "Please enter the account number");
            await BuildAccountSelectAsync();
            return View();


        }
        [Authorize(Roles = ("Admin, Customer"))]
        public async Task<IActionResult> Withdraw()
        {
            //ViewBag.Accounts = new SelectList(await _context.Accounts.ToListAsync(), "AccountNumber", "AccountNumber");
            await BuildAccountSelectAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin, Customer"))]
        public async Task<IActionResult> Withdraw(decimal amount, string accountNumber)
        {
            var account = new Account();
            if (User.IsInRole("Admin") && !string.IsNullOrEmpty(accountNumber))
            {
                account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
            }
            else
            {
                string userId = this.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                account = userId.GetUserAccountAsync(_context).Result;
            }
            if (account != null)
            {
                if (account.Balance - amount < 0)
                {
                    ModelState.AddModelError("", $"Withdraw amount less than {account.Balance}");
                    return View();
                }
                account.Balance = account.Balance - amount;
                _context.Update(account);
                await _context.SaveChangesAsync();

                var transaction = new Transaction
                {
                    AccountId = account.Id,
                    Amount = amount,
                    TransactionType = TransactionType.Withdraw,
                    TransctionDateTime = DateTime.UtcNow
                };
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(Index));
                }
                return Redirect($"/Account/View/{account.AccountNumber}");

            }
                ModelState.AddModelError(" ", "Please enter the account number");
                return View();
            

        }
        private async Task BuildAccountSelectAsync()
        {
            List<Account> account = await _context.Accounts.ToListAsync();
            ViewBag.Accounts = new SelectList(account, "AccountNumber", "AccountNumber");
        }

        private async Task BuildUserSelectAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "Email");
        }
    }
}
