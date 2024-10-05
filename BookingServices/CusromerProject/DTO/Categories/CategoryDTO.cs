using CusromerProject.DTO.Services;

namespace CusromerProject.DTO.Categories
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<ServiceForCategory> Services { get; set; } = new List<ServiceForCategory>();
    }
}
