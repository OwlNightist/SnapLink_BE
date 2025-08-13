namespace SnapLink_Model.DTO.Response;

public class BookingResponse
{
    public int Error { get; set; }
    public string Message { get; set; } = string.Empty;
    public BookingData? Data { get; set; }
}

public class BookingData
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int PhotographerId { get; set; }
    public string PhotographerName { get; set; } = string.Empty;
    public string PhotographerEmail { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string LocationAddress { get; set; } = string.Empty;
    public DateTime StartDatetime { get; set; }
    public DateTime EndDatetime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Payment information
    public bool HasPayment { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    
    // Escrow information
    public decimal EscrowBalance { get; set; }
    public bool HasEscrowFunds { get; set; }
    
    // Calculated fields
    public int DurationHours { get; set; }
    public decimal PricePerHour { get; set; }
}

public class BookingListResponse
{
    public int Error { get; set; }
    public string Message { get; set; } = string.Empty;
    public BookingListData? Data { get; set; }
}

public class BookingListData
{
    public List<BookingData> Bookings { get; set; } = new List<BookingData>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
} 