using CusromerProject.DTO.Review;
using CustomerProject.DTO.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CusromerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewRepository _reviewRepository;

        public ReviewsController(ReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // GET: api/Reviews/{customerId}/{bookingId}
        [HttpGet("{bookingId}")]
       [Authorize]
        public async Task<ActionResult<ReviewDTO>> GetReview(int bookingId)
        {
            var result = await _reviewRepository.GetReviewByIdAsync(bookingId);

            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        // POST: api/Reviews
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<bool>> PostReview(PostedReviewDTO review)
        {
            var result = await _reviewRepository.AddReviewAsync(review);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(new { message = "Created Success" });
        }
    }
}
