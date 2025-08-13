using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync(int page = 1, int pageSize = 10, int? year = null, int? month = null);
        Task<int> GetAllTransactionCountAsync(int? year = null, int? month = null);
        Task<IEnumerable<TransactionResponse>> GetUserTransactionHistoryAsync(int userId, int page = 1, int pageSize = 10, int? year = null, int? month = null);
        Task<IEnumerable<TransactionResponse>> GetPhotographerTransactionHistoryAsync(int photographerId, int page = 1, int pageSize = 10, int? year = null, int? month = null);
        Task<IEnumerable<TransactionResponse>> GetLocationOwnerTransactionHistoryAsync(int locationOwnerId, int page = 1, int pageSize = 10, int? year = null, int? month = null);
        Task<TransactionResponse> GetTransactionByIdAsync(int transactionId);
        Task<int> GetUserTransactionCountAsync(int userId, int? year = null, int? month = null);
        Task<int> GetPhotographerTransactionCountAsync(int photographerId, int? year = null, int? month = null);
        Task<int> GetLocationOwnerTransactionCountAsync(int locationOwnerId, int? year = null, int? month = null);
        Task CreatePaymentDistributionTransactionsAsync(int paymentId, decimal platformFee, decimal photographerPayout, decimal locationFee);
        Task<MonthlyIncomeResponse> GetMonthlyIncomeAsync(int userId, int year, int month);
    }
} 