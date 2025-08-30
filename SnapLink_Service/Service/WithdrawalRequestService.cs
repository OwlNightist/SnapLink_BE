using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.Constants;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class WithdrawalRequestService : IWithdrawalRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletService _walletService;

        public WithdrawalRequestService(IUnitOfWork unitOfWork, IWalletService walletService)
        {
            _unitOfWork = unitOfWork;
            _walletService = walletService;
        }

        public async Task<WithdrawalRequestResponse> CreateWithdrawalRequestAsync(CreateWithdrawalRequest request, int userId)
        {
            // Validate withdrawal amount
            if (!await ValidateWithdrawalAmountAsync(userId, request.Amount))
            {
                throw new InvalidOperationException("Invalid withdrawal amount or insufficient funds");
            }

            // Get user's wallet balance to double-check
            var currentBalance = await _walletService.GetWalletBalanceAsync(userId);
            if (currentBalance < request.Amount)
            {
                throw new InvalidOperationException("Insufficient funds in wallet");
            }

            // Get user's wallet using the generic repository
            var wallets = await _unitOfWork.WalletRepository.GetAsync(
                filter: w => w.UserId == userId,
                includeProperties: "User"
            );
            var wallet = wallets.FirstOrDefault();
            
            if (wallet == null)
            {
                throw new InvalidOperationException("Wallet not found for user");
            }

            // Create withdrawal request
            var withdrawalRequest = new WithdrawalRequest
            {
                WalletId = wallet.WalletId,
                Amount = request.Amount,
                BankAccountNumber = request.BankAccountNumber,
                BankAccountName = request.BankAccountName,
                BankName = request.BankName,
                RequestStatus = WithdrawalStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _unitOfWork.WithdrawalRequestRepository.AddAsync(withdrawalRequest);
            await _unitOfWork.SaveChangesAsync();

            // Map to response
            return await MapToResponseAsync(withdrawalRequest);
        }

        public async Task<WithdrawalRequestResponse> UpdateWithdrawalRequestAsync(int withdrawalId, UpdateWithdrawalRequest request, int userId)
        {
            var withdrawalRequest = await _unitOfWork.WithdrawalRequestRepository.GetByIdAsync(withdrawalId);
            if (withdrawalRequest == null)
            {
                throw new InvalidOperationException("Withdrawal request not found");
            }

            // Check if user owns this withdrawal request
            var wallets = await _unitOfWork.WalletRepository.GetAsync(
                filter: w => w.UserId == userId
            );
            var wallet = wallets.FirstOrDefault();
            
            if (wallet?.WalletId != withdrawalRequest.WalletId)
            {
                throw new UnauthorizedAccessException("You can only update your own withdrawal requests");
            }

            // Only allow updates if status is Pending
            if (withdrawalRequest.RequestStatus != WithdrawalStatus.Pending)
            {
                throw new InvalidOperationException("Cannot update withdrawal request that is not pending");
            }

            // Update fields
            if (request.BankAccountNumber != null)
                withdrawalRequest.BankAccountNumber = request.BankAccountNumber;
            if (request.BankAccountName != null)
                withdrawalRequest.BankAccountName = request.BankAccountName;
            if (request.BankName != null)
                withdrawalRequest.BankName = request.BankName;

            _unitOfWork.WithdrawalRequestRepository.Update(withdrawalRequest);
            await _unitOfWork.SaveChangesAsync();

            return await MapToResponseAsync(withdrawalRequest);
        }

        public async Task<bool> CancelWithdrawalRequestAsync(int withdrawalId, int userId)
        {
            var withdrawalRequest = await _unitOfWork.WithdrawalRequestRepository.GetByIdAsync(withdrawalId);
            if (withdrawalRequest == null)
            {
                throw new InvalidOperationException("Withdrawal request not found");
            }

            // Check if user owns this withdrawal request
            var wallets = await _unitOfWork.WalletRepository.GetAsync(
                filter: w => w.UserId == userId
            );
            var wallet = wallets.FirstOrDefault();
            
            if (wallet?.WalletId != withdrawalRequest.WalletId)
            {
                throw new UnauthorizedAccessException("You can only cancel your own withdrawal requests");
            }

            // Only allow cancellation if status is Pending
            if (withdrawalRequest.RequestStatus != WithdrawalStatus.Pending)
            {
                throw new InvalidOperationException("Cannot cancel withdrawal request that is not pending");
            }

            withdrawalRequest.RequestStatus = WithdrawalStatus.Cancelled;
            _unitOfWork.WithdrawalRequestRepository.Update(withdrawalRequest);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<WithdrawalRequestResponse?> GetWithdrawalRequestByIdAsync(int withdrawalId)
        {
            var withdrawalRequest = await _unitOfWork.WithdrawalRequestRepository.GetByIdAsync(withdrawalId);
            if (withdrawalRequest == null) return null;

            return await MapToResponseAsync(withdrawalRequest);
        }

        public async Task<WithdrawalRequestDetailResponse?> GetWithdrawalRequestDetailAsync(int withdrawalId)
        {
            var withdrawalRequest = await _unitOfWork.WithdrawalRequestRepository.GetByIdAsync(withdrawalId);
            if (withdrawalRequest == null) return null;

            var response = await MapToResponseAsync(withdrawalRequest);
            var detailResponse = new WithdrawalRequestDetailResponse
            {
                Id = response.Id,
                WalletId = response.WalletId,
                Amount = response.Amount,
                BankAccountNumber = response.BankAccountNumber,
                BankAccountName = response.BankAccountName,
                BankName = response.BankName,
                RequestStatus = response.RequestStatus,
                RequestedAt = response.RequestedAt,
                ProcessedAt = response.ProcessedAt,
                ProcessedByModeratorId = response.ProcessedByModeratorId,
                RejectionReason = response.RejectionReason,
                UserId = response.UserId,
                UserName = response.UserName,
                UserEmail = response.UserEmail,
                WalletBalance = response.WalletBalance
            };

            // Add moderator information if processed
            if (withdrawalRequest.ProcessedByModeratorId.HasValue)
            {
                var moderator = await _unitOfWork.UserRepository.GetByIdAsync(withdrawalRequest.ProcessedByModeratorId.Value);
                if (moderator != null)
                {
                    detailResponse.ModeratorName = moderator.FullName;
                    detailResponse.ModeratorEmail = moderator.Email;
                }
            }

            return detailResponse;
        }

        public async Task<WithdrawalRequestListResponse> GetUserWithdrawalRequestsAsync(int userId, int page = 1, int pageSize = 10)
        {
            var wallets = await _unitOfWork.WalletRepository.GetAsync(
                filter: w => w.UserId == userId
            );
            var wallet = wallets.FirstOrDefault();
            
            if (wallet == null)
            {
                return new WithdrawalRequestListResponse();
            }

            var withdrawalRequests = await _unitOfWork.WithdrawalRequestRepository.GetAsync(
                filter: wr => wr.WalletId == wallet.WalletId,
                orderBy: q => q.OrderByDescending(wr => wr.RequestedAt),
                includeProperties: "Wallet,Wallet.User"
            );

            var totalCount = withdrawalRequests.Count();
            var pagedRequests = withdrawalRequests
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var responses = new List<WithdrawalRequestResponse>();
            foreach (var request in pagedRequests)
            {
                responses.Add(await MapToResponseAsync(request));
            }

            return new WithdrawalRequestListResponse
            {
                WithdrawalRequests = responses,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<WithdrawalRequestListResponse> GetAllWithdrawalRequestsAsync(int page = 1, int pageSize = 10, string? status = null)
        {
            var filter = status != null 
                ? (Expression<Func<WithdrawalRequest, bool>>)(wr => wr.RequestStatus == status)
                : null;

            var withdrawalRequests = await _unitOfWork.WithdrawalRequestRepository.GetAsync(
                filter: filter,
                orderBy: q => q.OrderByDescending(wr => wr.RequestedAt),
                includeProperties: "Wallet,Wallet.User"
            );

            var totalCount = withdrawalRequests.Count();
            var pagedRequests = withdrawalRequests
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var responses = new List<WithdrawalRequestResponse>();
            foreach (var request in pagedRequests)
            {
                responses.Add(await MapToResponseAsync(request));
            }

            return new WithdrawalRequestListResponse
            {
                WithdrawalRequests = responses,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<WithdrawalRequestListResponse> GetWithdrawalRequestsByStatusAsync(string status, int page = 1, int pageSize = 10)
        {
            return await GetAllWithdrawalRequestsAsync(page, pageSize, status);
        }

        public async Task<WithdrawalRequestResponse> ProcessWithdrawalRequestAsync(int withdrawalId, ProcessWithdrawalRequest request, int moderatorId)
        {
            var withdrawalRequest = await _unitOfWork.WithdrawalRequestRepository.GetByIdAsync(withdrawalId);
            if (withdrawalRequest == null)
            {
                throw new InvalidOperationException("Withdrawal request not found");
            }

            // Only allow processing if status is Pending
            if (withdrawalRequest.RequestStatus != WithdrawalStatus.Pending)
            {
                throw new InvalidOperationException("Cannot process withdrawal request that is not pending");
            }

            // Get wallet and user information
            var wallets = await _unitOfWork.WalletRepository.GetAsync(
                filter: w => w.WalletId == withdrawalRequest.WalletId,
                includeProperties: "User"
            );
            var wallet = wallets.FirstOrDefault();
            
            if (wallet == null)
            {
                throw new InvalidOperationException("Wallet not found for withdrawal request");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(wallet.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found for withdrawal request");
            }

            // Update withdrawal request status
            withdrawalRequest.RequestStatus = request.Status;
            withdrawalRequest.ProcessedAt = DateTime.UtcNow;
            withdrawalRequest.ProcessedByModeratorId = moderatorId;

            if (request.Status == WithdrawalStatus.Rejected)
            {
                withdrawalRequest.RejectionReason = request.RejectionReason;
            }

            

   

            // Update withdrawal request
            _unitOfWork.WithdrawalRequestRepository.Update(withdrawalRequest);
            await _unitOfWork.SaveChangesAsync();

            return await MapToResponseAsync(withdrawalRequest);
        }

        public async Task<WithdrawalRequestResponse> ApproveWithdrawalRequestAsync(int withdrawalId, int moderatorId, string billImageLink)
        {
            return await ProcessWithdrawalRequestAsync(withdrawalId, new ProcessWithdrawalRequest 
            { 
                Status = WithdrawalStatus.Approved,
                RejectionReason = billImageLink // Using RejectionReason field to store bill image link for approved requests
            }, moderatorId);
        }

        public async Task<WithdrawalRequestResponse> RejectWithdrawalRequestAsync(int withdrawalId, string rejectionReason, int moderatorId)
        {
            return await ProcessWithdrawalRequestAsync(withdrawalId, new ProcessWithdrawalRequest 
            { 
                Status = WithdrawalStatus.Rejected,
                RejectionReason = rejectionReason
            }, moderatorId);
        }

        public async Task<WithdrawalRequestResponse> CompleteWithdrawalRequestAsync(int withdrawalId, int moderatorId, string? transactionReference = null)
        {
            var withdrawalRequest = await _unitOfWork.WithdrawalRequestRepository.GetByIdAsync(withdrawalId);
            if (withdrawalRequest == null)
            {
                throw new InvalidOperationException("Withdrawal request not found");
            }

            // Only allow completion if status is Approved
            if (withdrawalRequest.RequestStatus != WithdrawalStatus.Approved)
            {
                throw new InvalidOperationException("Cannot complete withdrawal request that is not approved");
            }

            // Get wallet and user information
            var wallets = await _unitOfWork.WalletRepository.GetAsync(
                filter: w => w.WalletId == withdrawalRequest.WalletId,
                includeProperties: "User"
            );
            var wallet = wallets.FirstOrDefault();
            
            if (wallet == null)
            {
                throw new InvalidOperationException("Wallet not found for withdrawal request");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(wallet.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found for withdrawal request");
            }
                var deductionSuccess = await _walletService.DeductFundsFromWalletAsync(wallet.UserId, withdrawalRequest.Amount);
                if (!deductionSuccess)
                {
                    throw new InvalidOperationException("Failed to deduct funds from wallet. Insufficient balance or wallet error.");
                }
            // Update withdrawal request status to Completed
            withdrawalRequest.RequestStatus = WithdrawalStatus.Completed;
            withdrawalRequest.ProcessedAt = DateTime.UtcNow;
            withdrawalRequest.ProcessedByModeratorId = moderatorId;

            // Create completion transaction (money actually transferred to bank)
            var completionTransaction = new Transaction
            {
                FromUserId = null, // System processes the completion
                ToUserId = wallet.UserId, // User receives the completed withdrawal
                Amount = withdrawalRequest.Amount,
                Currency = "VND",
                Type = TransactionType.Withdrawal,
                Status = TransactionStatus.Success,
                Note = $"Withdrawal request {withdrawalId} completed - Bank: {withdrawalRequest.BankName}, Account: {withdrawalRequest.BankAccountNumber}" + 
                       (transactionReference != null ? $", Reference: {transactionReference}" : ""),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TransactionRepository.AddAsync(completionTransaction);

            // Update withdrawal request
            _unitOfWork.WithdrawalRequestRepository.Update(withdrawalRequest);
            await _unitOfWork.SaveChangesAsync();

            return await MapToResponseAsync(withdrawalRequest);
        }

        public async Task<bool> CanUserCreateWithdrawalRequestAsync(int userId, decimal amount)
        {
            return await ValidateWithdrawalAmountAsync(userId, amount);
        }

        public async Task<decimal> GetMinimumWithdrawalAmountAsync()
        {
            // You can configure this from appsettings or database
            return 10000; // 10000 VND minimum
        }

        public async Task<decimal> GetMaximumWithdrawalAmountAsync(int userId)
        {
            return await _walletService.GetWalletBalanceAsync(userId);
        }

        public async Task<bool> ValidateWithdrawalAmountAsync(int userId, decimal amount)
        {
            var minimumAmount = await GetMinimumWithdrawalAmountAsync();
            var maximumAmount = await GetMaximumWithdrawalAmountAsync(userId);

            return amount >= minimumAmount && amount <= maximumAmount;
        }

        private async Task<WithdrawalRequestResponse> MapToResponseAsync(WithdrawalRequest withdrawalRequest)
        {
            var wallets = await _unitOfWork.WalletRepository.GetAsync(
                filter: w => w.WalletId == withdrawalRequest.WalletId,
                includeProperties: "User"
            );
            var wallet = wallets.FirstOrDefault();
            var user = wallet?.User;

            return new WithdrawalRequestResponse
            {
                Id = withdrawalRequest.Id,
                WalletId = withdrawalRequest.WalletId,
                Amount = withdrawalRequest.Amount,
                BankAccountNumber = withdrawalRequest.BankAccountNumber,
                BankAccountName = withdrawalRequest.BankAccountName,
                BankName = withdrawalRequest.BankName,
                RequestStatus = withdrawalRequest.RequestStatus,
                RequestedAt = withdrawalRequest.RequestedAt,
                ProcessedAt = withdrawalRequest.ProcessedAt,
                ProcessedByModeratorId = withdrawalRequest.ProcessedByModeratorId,
                RejectionReason = withdrawalRequest.RejectionReason,
                UserId = user?.UserId ?? 0,
                UserName = user?.UserName,
                UserEmail = user?.Email,
                WalletBalance = wallet?.Balance
            };
        }
    }
}
