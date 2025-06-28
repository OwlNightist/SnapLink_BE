using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SnapLink_Model.DTO.Request;
using SnapLink_Model.DTO.Response;
using SnapLink_Repository.Entity;
using SnapLink_Repository.Repository;
using SnapLink_Service.IService;

namespace SnapLink_Service.Service
{
    public class UserStyleService : IUserStyleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserStyleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserFavoriteStylesResponse> GetUserFavoriteStylesAsync(int userId)
        {
            // Check if user exists
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            var userStyles = await _unitOfWork.UserStyleRepository.GetAsync(
                filter: us => us.UserId == userId,
                includeProperties: "Style"
            );

            var response = new UserFavoriteStylesResponse
            {
                UserId = userId,
                UserName = user.UserName ?? "",
                FavoriteStyles = userStyles.Select(us => new UserStyleInfo
                {
                    StyleId = us.StyleId,
                    StyleName = us.Style.Name ?? "",
                    StyleDescription = us.Style.Description,
                    AddedAt = us.CreatedAt
                }).ToList()
            };

            return response;
        }

        public async Task<UserStyleResponse> AddUserStyleAsync(AddUserStyleRequest request)
        {
            // Check if user exists
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new ArgumentException($"User with ID {request.UserId} not found");

            // Check if style exists
            var style = await _unitOfWork.StyleRepository.GetByIdAsync(request.StyleId);
            if (style == null)
                throw new ArgumentException($"Style with ID {request.StyleId} not found");

            // Check if relationship already exists
            var existingUserStyle = await _unitOfWork.UserStyleRepository.GetAsync(
                filter: us => us.UserId == request.UserId && us.StyleId == request.StyleId
            );
            if (existingUserStyle.Any())
                throw new InvalidOperationException($"User already has style '{style.Name}' as favorite");

            var userStyle = new UserStyle
            {
                UserId = request.UserId,
                StyleId = request.StyleId,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.UserStyleRepository.AddAsync(userStyle);
            await _unitOfWork.SaveChangesAsync();

            return new UserStyleResponse
            {
                UserStyleId = userStyle.UserStyleId,
                UserId = userStyle.UserId,
                StyleId = userStyle.StyleId,
                StyleName = style.Name ?? "",
                StyleDescription = style.Description,
                CreatedAt = userStyle.CreatedAt
            };
        }

        public async Task<UserFavoriteStylesResponse> UpdateUserStylesAsync(UpdateUserStylesRequest request)
        {
            // Check if user exists
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new ArgumentException($"User with ID {request.UserId} not found");

            // Remove existing styles
            var existingUserStyles = await _unitOfWork.UserStyleRepository.GetAsync(
                filter: us => us.UserId == request.UserId
            );
            foreach (var userStyle in existingUserStyles)
            {
                _unitOfWork.UserStyleRepository.Remove(userStyle);
            }

            // Add new styles
            foreach (var styleId in request.StyleIds)
            {
                var style = await _unitOfWork.StyleRepository.GetByIdAsync(styleId);
                if (style != null)
                {
                    var userStyle = new UserStyle
                    {
                        UserId = request.UserId,
                        StyleId = styleId,
                        CreatedAt = DateTime.Now
                    };
                    await _unitOfWork.UserStyleRepository.AddAsync(userStyle);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // Return updated favorite styles
            return await GetUserFavoriteStylesAsync(request.UserId);
        }

        public async Task<bool> RemoveUserStyleAsync(int userId, int styleId)
        {
            var userStyle = await _unitOfWork.UserStyleRepository.GetAsync(
                filter: us => us.UserId == userId && us.StyleId == styleId
            );

            var styleToRemove = userStyle.FirstOrDefault();
            if (styleToRemove == null)
                return false;

            _unitOfWork.UserStyleRepository.Remove(styleToRemove);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StyleRecommendationResponse>> GetStyleRecommendationsAsync(int userId, int count = 10)
        {
            // Get user's favorite styles
            var userStyles = await _unitOfWork.UserStyleRepository.GetAsync(
                filter: us => us.UserId == userId,
                includeProperties: "Style"
            );

            if (!userStyles.Any())
            {
                // If user has no favorite styles, return popular styles
                var popularStyles = await _unitOfWork.StyleRepository.GetAsync(
                    includeProperties: "PhotographerStyles.Photographer.User"
                );

                return popularStyles
                    .OrderByDescending(s => s.PhotographerStyles.Count)
                    .Take(count)
                    .Select(s => new StyleRecommendationResponse
                    {
                        StyleId = s.StyleId,
                        StyleName = s.Name ?? "",
                        StyleDescription = s.Description,
                        PhotographerCount = s.PhotographerStyles.Count,
                        RecommendedPhotographers = s.PhotographerStyles
                            .Take(5)
                            .Select(ps => new RecommendedPhotographerInfo
                            {
                                PhotographerId = ps.Photographer.PhotographerId,
                                FullName = ps.Photographer.User.FullName ?? "",
                                Specialty = ps.Photographer.Specialty,
                                HourlyRate = ps.Photographer.HourlyRate,
                                Rating = ps.Photographer.Rating,
                                AvailabilityStatus = ps.Photographer.AvailabilityStatus,
                                ProfileImage = ps.Photographer.User.ProfileImage,
                                VerificationStatus = ps.Photographer.VerificationStatus
                            }).ToList()
                    });
            }

            // Get styles similar to user's favorites
            var favoriteStyleIds = userStyles.Select(us => us.StyleId).ToList();
            var allStyles = await _unitOfWork.StyleRepository.GetAsync(
                includeProperties: "PhotographerStyles.Photographer.User"
            );

            var recommendations = allStyles
                .Where(s => !favoriteStyleIds.Contains(s.StyleId)) // Exclude already favorite styles
                .OrderByDescending(s => s.PhotographerStyles.Count)
                .Take(count)
                .Select(s => new StyleRecommendationResponse
                {
                    StyleId = s.StyleId,
                    StyleName = s.Name ?? "",
                    StyleDescription = s.Description,
                    PhotographerCount = s.PhotographerStyles.Count,
                    RecommendedPhotographers = s.PhotographerStyles
                        .Take(5)
                        .Select(ps => new RecommendedPhotographerInfo
                        {
                            PhotographerId = ps.Photographer.PhotographerId,
                            FullName = ps.Photographer.User.FullName ?? "",
                            Specialty = ps.Photographer.Specialty,
                            HourlyRate = ps.Photographer.HourlyRate,
                            Rating = ps.Photographer.Rating,
                            AvailabilityStatus = ps.Photographer.AvailabilityStatus,
                            ProfileImage = ps.Photographer.User.ProfileImage,
                            VerificationStatus = ps.Photographer.VerificationStatus
                        }).ToList()
                });

            return recommendations;
        }

        public async Task<IEnumerable<RecommendedPhotographerInfo>> GetRecommendedPhotographersAsync(int userId, int count = 10)
        {
            // Get user's favorite styles
            var userStyles = await _unitOfWork.UserStyleRepository.GetAsync(
                filter: us => us.UserId == userId,
                includeProperties: "Style"
            );

            if (!userStyles.Any())
            {
                // If user has no favorite styles, return top-rated photographers
                var photographers = await _unitOfWork.PhotographerRepository.GetAsync(
                    filter: p => p.Rating > 0,
                    includeProperties: "User,PhotographerStyles.Style"
                );

                return photographers
                    .OrderByDescending(p => p.Rating)
                    .Take(count)
                    .Select(p => new RecommendedPhotographerInfo
                    {
                        PhotographerId = p.PhotographerId,
                        FullName = p.User.FullName ?? "",
                        Specialty = p.Specialty,
                        HourlyRate = p.HourlyRate,
                        Rating = p.Rating,
                        AvailabilityStatus = p.AvailabilityStatus,
                        ProfileImage = p.User.ProfileImage,
                        VerificationStatus = p.VerificationStatus
                    });
            }

            // Get photographers who specialize in user's favorite styles
            var favoriteStyleIds = userStyles.Select(us => us.StyleId).ToList();
            var photographerStyles = await _unitOfWork.PhotographerStyleRepository.GetAsync(
                filter: ps => favoriteStyleIds.Contains(ps.StyleId),
                includeProperties: "Photographer.User"
            );

            var recommendedPhotographers = photographerStyles
                .GroupBy(ps => ps.PhotographerId)
                .Select(g => new
                {
                    Photographer = g.First().Photographer,
                    MatchingStyles = g.Count()
                })
                .OrderByDescending(x => x.MatchingStyles)
                .ThenByDescending(x => x.Photographer.Rating)
                .Take(count)
                .Select(x => new RecommendedPhotographerInfo
                {
                    PhotographerId = x.Photographer.PhotographerId,
                    FullName = x.Photographer.User.FullName ?? "",
                    Specialty = x.Photographer.Specialty,
                    HourlyRate = x.Photographer.HourlyRate,
                    Rating = x.Photographer.Rating,
                    AvailabilityStatus = x.Photographer.AvailabilityStatus,
                    ProfileImage = x.Photographer.User.ProfileImage,
                    VerificationStatus = x.Photographer.VerificationStatus
                });

            return recommendedPhotographers;
        }

        public async Task<bool> IsUserStyleFavoriteAsync(int userId, int styleId)
        {
            var userStyle = await _unitOfWork.UserStyleRepository.GetAsync(
                filter: us => us.UserId == userId && us.StyleId == styleId
            );

            return userStyle.Any();
        }

        public async Task<IEnumerable<int>> GetUsersByStyleAsync(int styleId)
        {
            var userStyles = await _unitOfWork.UserStyleRepository.GetAsync(
                filter: us => us.StyleId == styleId
            );

            return userStyles.Select(us => us.UserId);
        }
    }
} 