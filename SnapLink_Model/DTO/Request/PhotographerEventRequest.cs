using System;
using System.Collections.Generic;

namespace SnapLink_Model.DTO.Request;

public class CreatePhotographerEventRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxBookings { get; set; }
    public List<int> LocationIds { get; set; } = new List<int>();
}

public class UpdatePhotographerEventRequest
{
    public int EventId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxBookings { get; set; }
    public string? Status { get; set; }
    public List<int> LocationIds { get; set; } = new List<int>();
}

public class GetPhotographerEventsRequest
{
    public int? PhotographerId { get; set; }
    public int? LocationId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 