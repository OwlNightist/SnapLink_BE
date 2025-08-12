using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService;

public interface IWalletService
{
    Task<decimal> GetWalletBalanceAsync(int userId);
    Task<bool> AddFundsToWalletAsync(int userId, decimal amount);
    Task<bool> DeductFundsFromWalletAsync(int userId, decimal amount);
    Task<bool> TransferFundsAsync(int fromUserId, int toUserId, decimal amount);
    Task<bool> CreateWalletIfNotExistsAsync(int userId);

} 