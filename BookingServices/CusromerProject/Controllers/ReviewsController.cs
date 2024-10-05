using CusromerProject.DTO.Review;
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
        [HttpGet("{customerId}/{bookingId}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(string customerId, int bookingId)
        {
            var result = await _reviewRepository.GetReviewByIdAsync(customerId, bookingId);

            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        // POST: api/Reviews
        [HttpPost]
        public async Task<ActionResult<bool>> PostReview(ReviewDTO reviewDTO)
        {
            var result = await _reviewRepository.AddReviewAsync(reviewDTO);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Created();
        }
    }
}
