using BookingServices.Data;
using CusromerProject.DTO.Categories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.ComponentModel.DataAnnotations;

namespace CusromerProject.DTO.Review
{
    public class ReviewRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public ReviewRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMemoryCache cache)
        {
            _context = context;
            _userManager = userManager;
            _cache = cache;
        }

        // Method to get a review by customerId and bookingId
        public async Task<Result<ReviewDTO>> GetReviewByIdAsync(string customerId, int bookingId)
        {
            string cacheKey = $"review_{customerId}_{bookingId}";

            if (!_cache.TryGetValue(cacheKey, out ReviewDTO reviewDTO))
            {
                var review = await _context.Reviews
                                           .AsNoTracking()
                                           .FirstOrDefaultAsync(r => r.CustomerId == customerId && r.BookingId == bookingId)
                                           .ConfigureAwait(false);

                if (review == null)
                {
                    return Result<ReviewDTO>.Failure("Review not found for the specified customer and booking.");
                }

                // Map the Review entity to a DTO
                reviewDTO = new ReviewDTO()
                {
                    CustomerId = review.CustomerId,
                    BookingId = review.BookingId,
                    Rating = review.Rating,
                    CustomerComment = review.CustomerComment,
                    CustomerCommentDate = review.CustomerCommentDate,
                    ProviderReplayComment = review.ProviderReplayComment,
                    ProviderReplayCommentDate = review.ProviderReplayCommentDate
                };

                // Cache the review
                _cache.Set(cacheKey, reviewDTO, _cacheDuration);
            }

            return Result<ReviewDTO>.Success(reviewDTO);
        }

        // Method to add a review with validation and caching
        public async Task<Result<bool>> AddReviewAsync(ReviewDTO reviewDTO)
        {
            if (reviewDTO == null)
            {
                return Result<bool>.Failure("Review cannot be null.");
            }

            // Validate the review model (assuming there are Data Annotations on the Review class)
            var validationContext = new ValidationContext(reviewDTO);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(reviewDTO, validationContext, validationResults, true);

            if (!isValid)
                return Result<bool>.Failure("Review model is invalid.");

            var review = new BookingServices.Data.Review()
            {
                CustomerId = reviewDTO.CustomerId,
                BookingId = reviewDTO.BookingId,
                Rating = reviewDTO.Rating,
                CustomerComment = reviewDTO.CustomerComment,
                CustomerCommentDate = reviewDTO.CustomerCommentDate,
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Invalidate cache for any related reviews if necessary
            // e.g., _cache.Remove($"review_{review.CustomerId}_{review.BookingId}");

            return Result<bool>.Success(true);
        }
    }
}
