using BookingServices.Data;
using CusromerProject.DTO.Categories;
using CustomerProject.DTO.Review;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        public async Task<Result<List<ReviewDTO>>> GetReviewByIdAsync(string customerId)
        {
            List<ReviewDTO> reviewDTOs;
            // If not found in cache, query the database
            var reviews = await _context.Reviews
                                        .AsNoTracking()
                                        .Where(r => r.CustomerId == customerId)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

            if (!reviews.Any())
            {
                return Result<List<ReviewDTO>>.Failure("No reviews found for the specified customer.");
            }

            // Map the list of Review entities to a list of ReviewDTOs
            reviewDTOs = reviews.Select(review => new ReviewDTO()
            {
                CustomerId = review.CustomerId,
                BookingId = review.BookingId,
                Rating = review.Rating,
                CustomerComment = review.CustomerComment,
                CustomerCommentDate = review.CustomerCommentDate,
                ProviderReplayComment = review.ProviderReplayComment,
                ProviderReplayCommentDate = review.ProviderReplayCommentDate
            }).ToList();

            // Return the list of ReviewDTOs (whether from cache or DB)
            return Result<List<ReviewDTO>>.Success(reviewDTOs);
        }

        // Method to add a review with validation and caching
        public async Task<Result<bool>> AddReviewAsync(PostedReviewDTO reviewDTO)
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
                CustomerCommentDate = DateTime.Now
            };

            try
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            } catch (Exception ex) {
                
                return Result<bool>.Failure(ex.ToString());
            }

            return Result<bool>.Success(true);
        }
    }
}
