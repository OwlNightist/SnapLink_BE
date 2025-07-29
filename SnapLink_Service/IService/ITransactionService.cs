using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionResponse>> GetUserTransactionHistoryAsync(int userId, int page = 1, int pageSize = 10);
        Task<IEnumerable<TransactionResponse>> GetPhotographerTransactionHistoryAsync(int photographerId, int page = 1, int pageSize = 10);
        Task<IEnumerable<TransactionResponse>> GetLocationOwnerTransactionHistoryAsync(int locationOwnerId, int page = 1, int pageSize = 10);
        Task<TransactionResponse> GetTransactionByIdAsync(int transactionId);
        Task<int> GetUserTransactionCountAsync(int userId);
        Task<int> GetPhotographerTransactionCountAsync(int photographerId);
        Task<int> GetLocationOwnerTransactionCountAsync(int locationOwnerId);
        Task CreatePaymentDistributionTransactionsAsync(int paymentId, decimal platformFee, decimal photographerPayout, decimal locationFee);
    }
} 