using System;
using System.Collections.Generic;

namespace SnapLink_Model.DTO.Response
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
        public string? Bio { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Status { get; set; }
        
        // User's favorite styles
        public List<string> FavoriteStyles { get; set; } = new List<string>();
    }

    public class UserDetailResponse : UserResponse
    {
        public List<UserStyleInfo> FavoriteStyleDetails { get; set; } = new List<UserStyleInfo>();
        public int TotalBookings { get; set; }
        public int TotalReviews { get; set; }
    }
} 