using Microsoft.EntityFrameworkCore;
using SnapLink_Repository.DBContext;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class WalletService : IWalletService
    {
        private readonly SnaplinkDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(SnaplinkDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> GetWalletBalanceAsync(int userId)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId);
            
            return wallet?.Balance ?? 0;
        }

        public async Task<bool> AddFundsToWalletAsync(int userId, decimal amount)
        {
            try
            {
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);
                
                if (wallet == null)
                {
                    // Create wallet if it doesn't exist
                    wallet = new Wallet
                    {
                        UserId = userId,
                        Balance = amount,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _context.Wallets.AddAsync(wallet);
                }
                else
                {
                    wallet.Balance = (wallet.Balance ?? 0) + amount;
                    wallet.UpdatedAt = DateTime.UtcNow;
                }

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding funds to wallet: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeductFundsFromWalletAsync(int userId, decimal amount)
        {
            try
            {
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);
                
                if (wallet == null || (wallet.Balance ?? 0) < amount)
                {
                    return false; // Insufficient funds
                }

                wallet.Balance = wallet.Balance.Value - amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deducting funds from wallet: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> TransferFundsAsync(int fromUserId, int toUserId, decimal amount)
        {
            try
            {
                // Deduct from source wallet
                var success = await DeductFundsFromWalletAsync(fromUserId, amount);
                if (!success)
                {
                    return false;
                }

                // Add to destination wallet
                success = await AddFundsToWalletAsync(toUserId, amount);
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error transferring funds: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateWalletIfNotExistsAsync(int userId)
        {
            try
            {
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.UserId == userId);
                
                if (wallet == null)
                {
                    wallet = new Wallet
                    {
                        UserId = userId,
                        Balance = 0,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _context.Wallets.AddAsync(wallet);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating wallet: {ex.Message}");
                return false;
            }
        }

    }
} 