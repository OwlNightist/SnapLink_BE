using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;

namespace SnapLink_Service.IService
{
    public interface IWithdrawalRequestService
    {
        // Create and manage withdrawal requests
        Task<WithdrawalRequestResponse> CreateWithdrawalRequestAsync(CreateWithdrawalRequest request, int userId);
        Task<WithdrawalRequestResponse> UpdateWithdrawalRequestAsync(int withdrawalId, UpdateWithdrawalRequest request, int userId);
        Task<bool> CancelWithdrawalRequestAsync(int withdrawalId, int userId);
        
        // Get withdrawal requests
        Task<WithdrawalRequestResponse?> GetWithdrawalRequestByIdAsync(int withdrawalId);
        Task<WithdrawalRequestDetailResponse?> GetWithdrawalRequestDetailAsync(int withdrawalId);
        Task<WithdrawalRequestListResponse> GetUserWithdrawalRequestsAsync(int userId, int page = 1, int pageSize = 10);
        Task<WithdrawalRequestListResponse> GetAllWithdrawalRequestsAsync(int page = 1, int pageSize = 10, string? status = null);
        Task<WithdrawalRequestListResponse> GetWithdrawalRequestsByStatusAsync(string status, int page = 1, int pageSize = 10);
        
        // Admin/Moderator operations
        Task<WithdrawalRequestResponse> ProcessWithdrawalRequestAsync(int withdrawalId, ProcessWithdrawalRequest request, int moderatorId);
        Task<WithdrawalRequestResponse> ApproveWithdrawalRequestAsync(int withdrawalId, int moderatorId);
        Task<WithdrawalRequestResponse> RejectWithdrawalRequestAsync(int withdrawalId, string rejectionReason, int moderatorId);
        Task<WithdrawalRequestResponse> CompleteWithdrawalRequestAsync(int withdrawalId, int moderatorId, string? transactionReference = null);
        
        // Business logic
        Task<bool> CanUserCreateWithdrawalRequestAsync(int userId, decimal amount);
        Task<decimal> GetMinimumWithdrawalAmountAsync();
        Task<decimal> GetMaximumWithdrawalAmountAsync(int userId);
        Task<bool> ValidateWithdrawalAmountAsync(int userId, decimal amount);
    }
}
