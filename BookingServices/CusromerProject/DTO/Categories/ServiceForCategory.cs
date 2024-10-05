namespace CusromerProject.DTO.Categories
{
    public class ServiceForCategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        public decimal? Price { get; set; }
        public string? Image { get; set; }
    }
}
