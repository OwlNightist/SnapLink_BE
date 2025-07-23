using System;
using System.Collections.Generic;

namespace SnapLink_Model.DTO.Response;

public class PhotographerEventResponse
{
    public int EventId { get; set; }
    public int PhotographerId { get; set; }
    public string? PhotographerName { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxBookings { get; set; }
    public int? CurrentBookings { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<LocationResponse> Locations { get; set; } = new List<LocationResponse>();
}

public class PhotographerEventListResponse
{
    public int EventId { get; set; }
    public int PhotographerId { get; set; }
    public string? PhotographerName { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxBookings { get; set; }
    public int? CurrentBookings { get; set; }
    public string? Status { get; set; }
    public int LocationCount { get; set; }
    public bool IsAvailable => Status == "Active" && 
                              StartDate <= DateTime.Now && 
                              EndDate >= DateTime.Now && 
                              (MaxBookings == null || CurrentBookings < MaxBookings);
}

public class LocationResponse
{
    public int LocationId { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public decimal? HourlyRate { get; set; }
    public int? Capacity { get; set; }
    public bool? Indoor { get; set; }
    public bool? Outdoor { get; set; }
    public string? AvailabilityStatus { get; set; }
} 