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
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotographerService _photographerService;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper, IPhotographerService photographerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photographerService = photographerService;
        }

        public async Task<IEnumerable<ReviewResponse>> GetAllReviewsAsync()
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAsync(
                includeProperties: "Booking,Booking.Photographer,Booking.User"
            );
            return _mapper.Map<IEnumerable<ReviewResponse>>(reviews);
        }

        public async Task<ReviewResponse> GetReviewByIdAsync(int id)
        {
            var review = await _unitOfWork.ReviewRepository.GetAsync(
                filter: r => r.ReviewId == id,
                includeProperties: "Booking,Booking.Photographer,Booking.User"
            );
            
            var reviewEntity = review.FirstOrDefault();
            if (reviewEntity == null)
                throw new ArgumentException($"Review with ID {id} not found");
                
            return _mapper.Map<ReviewResponse>(reviewEntity);
        }

        public async Task<IEnumerable<ReviewResponse>> GetReviewsByPhotographerAsync(int photographerId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAsync(
                filter: r => r.RevieweeId == photographerId && r.RevieweeType == "Photographer",
                includeProperties: "Booking,Booking.Photographer,Booking.User"
            );
            return _mapper.Map<IEnumerable<ReviewResponse>>(reviews);
        }

        public async Task<IEnumerable<ReviewResponse>> GetReviewsByBookingAsync(int bookingId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetAsync(
                filter: r => r.BookingId == bookingId,
                includeProperties: "Booking,Booking.Photographer,Booking.User"
            );
            return _mapper.Map<IEnumerable<ReviewResponse>>(reviews);
        }

        public async Task<ReviewResponse> CreateReviewAsync(CreateReviewRequest request)
        {
            // Check if booking exists
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(request.BookingId);
            if (booking == null)
                throw new ArgumentException($"Booking with ID {request.BookingId} not found");

            // Check if review already exists for this booking
            var existingReview = await _unitOfWork.ReviewRepository.GetAsync(
                filter: r => r.BookingId == request.BookingId
            );
            if (existingReview.Any())
                throw new InvalidOperationException($"Review already exists for booking {request.BookingId}");

            var review = _mapper.Map<Review>(request);
            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            // Update photographer rating if the review is for a photographer
            if (request.RevieweeType == "Photographer")
            {
                await _photographerService.UpdateRatingFromReviewAsync(request.RevieweeId, request.Rating);
            }

            // Get the created review with related data
            var createdReview = await _unitOfWork.ReviewRepository.GetAsync(
                filter: r => r.ReviewId == review.ReviewId,
                includeProperties: "Booking,Booking.Photographer,Booking.User"
            );

            return _mapper.Map<ReviewResponse>(createdReview.FirstOrDefault());
        }

        public async Task<ReviewResponse> UpdateReviewAsync(int id, UpdateReviewRequest request)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new ArgumentException($"Review with ID {id} not found");

            // Store old rating for photographer update
            var oldRating = review.Rating;
            var revieweeId = review.RevieweeId;
            var revieweeType = review.RevieweeType;

            _mapper.Map(request, review);
            _unitOfWork.ReviewRepository.Update(review);
            await _unitOfWork.SaveChangesAsync();

            // Update photographer rating if the review is for a photographer and rating changed
            if (revieweeType == "Photographer" && request.Rating.HasValue && request.Rating != oldRating)
            {
                // Recalculate photographer rating by getting all reviews
                var photographerReviews = await _unitOfWork.ReviewRepository.GetAsync(
                    filter: r => r.RevieweeId == revieweeId && r.RevieweeType == "Photographer"
                );

                var totalRating = photographerReviews.Sum(r => r.Rating);
                var reviewCount = photographerReviews.Count();

                var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(revieweeId);
                if (photographer != null)
                {
                    photographer.RatingSum = totalRating;
                    photographer.RatingCount = reviewCount;
                    photographer.Rating = reviewCount > 0 ? totalRating / reviewCount : null;

                    _unitOfWork.PhotographerRepository.Update(photographer);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            // Get the updated review with related data
            var updatedReview = await _unitOfWork.ReviewRepository.GetAsync(
                filter: r => r.ReviewId == id,
                includeProperties: "Booking,Booking.Photographer,Booking.User"
            );

            return _mapper.Map<ReviewResponse>(updatedReview.FirstOrDefault());
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(id);
            if (review == null)
                return false;

            var revieweeId = review.RevieweeId;
            var revieweeType = review.RevieweeType;

            _unitOfWork.ReviewRepository.Remove(review);
            await _unitOfWork.SaveChangesAsync();

            // Update photographer rating if the review was for a photographer
            if (revieweeType == "Photographer")
            {
                var photographerReviews = await _unitOfWork.ReviewRepository.GetAsync(
                    filter: r => r.RevieweeId == revieweeId && r.RevieweeType == "Photographer"
                );

                var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(revieweeId);
                if (photographer != null)
                {
                    if (photographerReviews.Any())
                    {
                        var totalRating = photographerReviews.Sum(r => r.Rating);
                        var reviewCount = photographerReviews.Count();

                        photographer.RatingSum = totalRating;
                        photographer.RatingCount = reviewCount;
                        photographer.Rating = totalRating / reviewCount;
                    }
                    else
                    {
                        photographer.RatingSum = null;
                        photographer.RatingCount = null;
                        photographer.Rating = null;
                    }

                    _unitOfWork.PhotographerRepository.Update(photographer);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<decimal?> GetAverageRatingForPhotographerAsync(int photographerId)
        {
            var photographer = await _unitOfWork.PhotographerRepository.GetByIdAsync(photographerId);
            return photographer?.Rating;
        }
    }
} 