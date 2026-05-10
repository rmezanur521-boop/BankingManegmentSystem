using BankingManegmentSystem.Data;
using BankingManegmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BankingManegmentSystem.Helper
{
    public static class Extensions
    {
        public static async Task<Account> GetUserAccountAsync(this string userId, AppDbContext _context)
        {
            var accoun = await _context.Accounts.FirstOrDefaultAsync(x => x.UserId == userId);
            if (accoun != null)
            {
                return accoun;
            }
            else
            {
                return null;
            }
        }
    }
}
