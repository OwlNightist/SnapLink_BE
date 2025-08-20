namespace SnapLink_Model.DTO.Response
{
    public class WithdrawalRequestResponse
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        public string? BankName { get; set; }
        public string? RequestStatus { get; set; }
        public DateTime? RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int? ProcessedByModeratorId { get; set; }
        public string? RejectionReason { get; set; }
        
        // Additional user information
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public decimal? WalletBalance { get; set; }
    }

    public class WithdrawalRequestListResponse
    {
        public List<WithdrawalRequestResponse> WithdrawalRequests { get; set; } = new List<WithdrawalRequestResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class WithdrawalRequestDetailResponse : WithdrawalRequestResponse
    {
        public string? ModeratorName { get; set; }
        public string? ModeratorEmail { get; set; }
    }
}
