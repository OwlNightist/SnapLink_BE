using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.Entity;

namespace SnapLink_Model.DTO
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.FavoriteStyles, opt => opt.MapFrom(src => src.UserStyles.Select(us => us.Style.Name)));

            CreateMap<User, UserDetailResponse>()
                .ForMember(dest => dest.FavoriteStyles, opt => opt.MapFrom(src => src.UserStyles.Select(us => us.Style.Name)))
                .ForMember(dest => dest.FavoriteStyleDetails, opt => opt.MapFrom(src => src.UserStyles.Select(us => new UserStyleInfo
                {
                    StyleId = us.StyleId,
                    StyleName = us.Style.Name ?? "",
                    StyleDescription = us.Style.Description,
                    AddedAt = us.CreatedAt
                })))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.Bookings.Count))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Bookings.SelectMany(b => b.Reviews).Count()));

            // Photographer mappings
            CreateMap<Photographer, PhotographerResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.User.Bio))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.User.CreateAt))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => src.User.UpdateAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User.Status))
                .ForMember(dest => dest.Styles, opt => opt.MapFrom(src => src.PhotographerStyles.Select(ps => ps.Style.Name)));

            CreateMap<Photographer, PhotographerListResponse>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage))
                .ForMember(dest => dest.Styles, opt => opt.MapFrom(src => src.PhotographerStyles.Select(ps => ps.Style.Name)));

            CreateMap<Photographer, PhotographerDetailResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.User.ProfileImage))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.User.Bio))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.User.CreateAt))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => src.User.UpdateAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.User.Status))
                .ForMember(dest => dest.TotalBookings, opt => opt.MapFrom(src => src.Bookings.Count))
                .ForMember(dest => dest.WalletBalance, opt => opt.MapFrom(src => src.PhotographerWallets.FirstOrDefault().Balance))
                .ForMember(dest => dest.Styles, opt => opt.MapFrom(src => src.PhotographerStyles.Select(ps => ps.Style.Name)));

            CreateMap<CreatePhotographerRequest, Photographer>();
            CreateMap<UpdatePhotographerRequest, Photographer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Style mappings
            CreateMap<Style, StyleResponse>()
                .ForMember(dest => dest.PhotographerCount, opt => opt.MapFrom(src => src.PhotographerStyles.Count));

            CreateMap<Style, StyleDetailResponse>()
                .ForMember(dest => dest.PhotographerCount, opt => opt.MapFrom(src => src.PhotographerStyles.Count))
                .ForMember(dest => dest.Photographers, opt => opt.MapFrom(src => src.PhotographerStyles.Select(ps => new StylePhotographerInfo
                {
                    PhotographerId = ps.Photographer.PhotographerId,
                    FullName = ps.Photographer.User.FullName ?? "",
                    Specialty = ps.Photographer.Specialty,
                    HourlyRate = ps.Photographer.HourlyRate,
                    Rating = ps.Photographer.Rating,
                    AvailabilityStatus = ps.Photographer.AvailabilityStatus,
                    ProfileImage = ps.Photographer.User.ProfileImage
                })));

            CreateMap<CreateStyleRequest, Style>();
            CreateMap<UpdateStyleRequest, Style>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Review mappings
            CreateMap<Review, ReviewResponse>()
                .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Booking.User.FullName))
                .ForMember(dest => dest.RevieweeName, opt => opt.MapFrom(src => src.Booking.Photographer.User.FullName))
                .ForMember(dest => dest.BookingDescription, opt => opt.MapFrom(src => $"Booking on {src.Booking.StartDatetime:MMM dd, yyyy}"));

            CreateMap<CreateReviewRequest, Review>();
            CreateMap<UpdateReviewRequest, Review>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, UserDto>()
           .ForMember(dest => dest.Roles,
                      opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.RoleName)));
            CreateMap<Location, LocationDto>();
        }
    }
}
