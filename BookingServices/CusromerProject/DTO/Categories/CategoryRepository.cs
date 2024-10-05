using BookingServices.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CusromerProject.DTO.Categories
{
    public class CategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public CategoryRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Result<List<Category>>> GetAll()
        {
            const string cacheKey = "categories_list";

            if (!_cache.TryGetValue(cacheKey, out List<Category> categories))
            {
                categories = await _context.Categories
                                           .AsNoTracking()
                                           .ToListAsync()
                                           .ConfigureAwait(false);

                if (categories == null || categories.Count == 0)
                {
                    return Result<List<Category>>.Failure("No categories found.");
                }

                _cache.Set(cacheKey, categories, _cacheDuration);
            }

            return Result<List<Category>>.Success(categories);
        }

        public async Task<Result<CategoryDTO>> GetById(int id)
        {
            string cacheKey = $"category_{id}";

            if (!_cache.TryGetValue(cacheKey, out CategoryDTO targetCategory))
            {
                var category = await _context.Categories
                                             .Include(c => c.Services)
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(c => c.CategoryId == id)
                                             .ConfigureAwait(false);

                if (category == null)
                {
                    return Result<CategoryDTO>.Failure("Category not found.");
                }

                targetCategory = MapToDTO(category);

                _cache.Set(cacheKey, targetCategory, _cacheDuration);
            }

            return Result<CategoryDTO>.Success(targetCategory);
        }

        private CategoryDTO MapToDTO(Category category)
        {
            var targetCategory = new CategoryDTO()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.Name,
                Services = category.Services.Select(item => new ServiceForCategory
                {
                    Id = item.ServiceId,
                    Name = item.Name,
                    Location = item.Location,
                    Image = GetServiceImageAsync(item.ServiceId).Result, // Caution: Consider making this async
                    Price = GetServicePriceAsync(item.ServiceId).Result  // Caution: Consider making this async
                }).ToList()
            };

            return targetCategory;
        }

        private async Task<string> GetServiceImageAsync(int serviceId)
        {
            return await _context.ServiceImages
                                 .Where(s => s.ServiceId == serviceId)
                                 .Select(s => s.URL)
                                 .FirstOrDefaultAsync()
                                 .ConfigureAwait(false);
        }

        private async Task<decimal?> GetServicePriceAsync(int serviceId)
        {
            return await _context.ServicePrices
                                 .Where(sp => sp.ServiceId == serviceId && sp.PriceDate.Date == DateTime.Now.Date)
                                 .Select(sp => sp.Price)
                                 .FirstOrDefaultAsync()
                                 .ConfigureAwait(false);
        }
    }

    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public string Error { get; private set; }
        public T Value { get; private set; }

        private Result(bool isSuccess, T value, string error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, null);
        public static Result<T> Failure(string error) => new Result<T>(false, default, error);
    }
}
