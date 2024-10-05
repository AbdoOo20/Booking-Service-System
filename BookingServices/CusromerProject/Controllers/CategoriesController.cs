using Microsoft.AspNetCore.Mvc;
using CusromerProject.DTO.Categories;
using BookingServices.Data;

namespace CusromerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly ILogger<CategoriesController> _logger; // Add logger

        public CategoriesController(CategoryRepository categoryRepository, ILogger<CategoriesController> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var result = await _categoryRepository.GetAll();

            if (!result.IsSuccess)
            {
                _logger.LogWarning(result.Error);
                return NoContent(); // Return 204 No Content if no categories are found
            }

            return Ok(result.Value); // Return 200 OK with the list of categories
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var result = await _categoryRepository.GetById(id);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(result.Error);
                return NotFound(result.Error); // Return 404 if category is not found
            }

            return Ok(result.Value); // Return 200 OK with the CategoryDTO
        }
    }
}
