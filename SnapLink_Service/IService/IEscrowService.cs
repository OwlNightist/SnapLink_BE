using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IEscrowService
    {
        /// <summary>
        /// Hold funds in escrow when booking is confirmed
        /// </summary>
        Task<bool> HoldFundsInEscrowAsync(int bookingId, int userId, decimal amount);
        
        /// <summary>
        /// Release funds from escrow when booking is completed
        /// </summary>
        Task<bool> ReleaseFundsFromEscrowAsync(int bookingId);
        
        /// <summary>
        /// Refund funds from escrow when booking is cancelled
        /// </summary>
        Task<bool> RefundFundsFromEscrowAsync(int bookingId, int userId);
        
        /// <summary>
        /// Get escrow balance for a booking
        /// </summary>
        Task<decimal> GetEscrowBalanceAsync(int bookingId);
        
        /// <summary>
        /// Get all escrow transactions for a booking
        /// </summary>
        Task<IEnumerable<TransactionResponse>> GetEscrowTransactionsAsync(int bookingId);
    }
} 