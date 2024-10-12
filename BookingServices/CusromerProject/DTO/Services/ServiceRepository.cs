using BookingServices.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CustomerProject.DTO.Services;
using Microsoft.DotNet.Scaffolding.Shared;

namespace CusromerProject.DTO.Services
{
    public class ServiceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(1); // Cache duration of 5 minutes

        public ServiceRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // 1. Get all services with caching
        public async Task<List<AllServicesDetailsDTO>> GetAllServicesAsync()
        {
            const string cacheKey = "all_services_cache";

            // Check if data is in the cache
            if (!_cache.TryGetValue(cacheKey, out List<AllServicesDetailsDTO>? services))
            {
                services = await _context.Services
                    .Where(s => s.IsOnlineOrOffline == true && s.IsBlocked == false &&
                        s.ServicePrices.Any(sp => sp.PriceDate.Date == DateTime.Now.Date && sp.Price > 0))
                    .Include(s => s.Category)
                    .Include(s => s.ServicePrices)
                    .Include(s => s.ServiceImages)
                    .AsNoTracking()  // Disable change tracking for read-only operation
                    .Select(s => new AllServicesDetailsDTO
                    {
                        Id = s.ServiceId,
                        Name = s.Name,
                        Location = s.Location,
                        Category = s.Category.Name,
                        Quantity = s.Quantity,
                        PriceForTheCurrentDay = s.ServicePrices
                            .Where(sp => sp.PriceDate.Date == DateTime.Now.Date)
                            .Select(sp => sp.Price.ToString())
                            .FirstOrDefault(),
                        Image = s.ServiceImages.FirstOrDefault().URL, // Null-conditional operator
                        _AdminContract = BuildAdminContract(s.AdminContract), // Use helper method
                        _ProviderContract = BuildProviderContract(s.ProviderContract),
                        Provider = new ProviderDTO
                        {
                            ProviderId = s.ServiceProvider.ProviderId,
                            Name = s.ServiceProvider.Name,
                            ServiceDetails = s.ServiceProvider.ServiceDetails ?? string.Empty,
                            Rate = s.ServiceProvider.Rate
                        }
                    })
                    .ToListAsync();

                // Store data in the cache
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                };
                _cache.Set(cacheKey, services, cacheOptions);
            }

            return services;
        }

        // 2. Get service details by ID with caching
        public async Task<ServiceDetailsDTO?> GetServiceByIdAsync(int serviceId)
        {
            string cacheKey = $"service_details_{serviceId}";

            // Check if data is in the cache
            if (!_cache.TryGetValue(cacheKey, out ServiceDetailsDTO? service))
            {
                service = await _context.Services
                    .Where(s => s.ServiceId == serviceId && s.IsOnlineOrOffline == true && s.IsBlocked == false &&
                        s.ServicePrices.Any(sp => sp.PriceDate.Date == DateTime.Now.Date && sp.Price > 0))
                    .Include(s => s.Category)
                    .Include(s => s.ServiceProvider)
                    .Include(s => s.ServicePrices)
                    .Include(s => s.ServiceImages)
                    .Include(s => s.Relatedservices)
                    .AsNoTracking()  // Disable change tracking
                    .Select(s => new ServiceDetailsDTO
                    {
                        Id = s.ServiceId,
                        Name = s.Name,
                        Details = s.Details,
                        Location = s.Location,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        Quantity = s.Quantity,
                        InitialPayment = s.InitialPaymentPercentage,
                        Category = s.Category.Name,
                        ProviderName = s.ServiceProvider.Name ?? string.Empty, // Null check for provider
                        PriceForTheCurrentDay = s.ServicePrices
                            .Where(sp => sp.PriceDate.Date == DateTime.Now.Date)
                            .Select(sp => sp.Price)
                            .FirstOrDefault(),
                        Images = s.ServiceImages.Select(i => i.URL).ToList(),
                        RelatedServices = s.Relatedservices
                         .Where(rs => rs.IsOnlineOrOffline == true && rs.IsBlocked == false &&
                                 rs.ServicePrices.Any(sp => sp.PriceDate.Date == DateTime.Now.Date && sp.Price > 0))
                         .Select(rs => new AllServicesDetailsDTO
                        {
                            Id = rs.ServiceId,
                            Name = rs.Name,
                            Location = rs.Location,
                            Category = rs.Category.Name,
                            PriceForTheCurrentDay = rs.ServicePrices
                                .Where(sp => sp.PriceDate.Date == DateTime.Now.Date)
                                .Select(sp => sp.Price.ToString())
                                .FirstOrDefault(),
                            Image = rs.ServiceImages.FirstOrDefault().URL
                        }).ToList(),
                        _AdminContract = BuildAdminContract(s.AdminContract), // Use helper method
                        _ProviderContract = BuildProviderContract(s.ProviderContract),
                        Provider = new ProviderDTO
                        {
                            ProviderId = s.ServiceProvider.ProviderId,
                            Name = s.ServiceProvider.Name,
                            ServiceDetails = s.ServiceProvider.ServiceDetails ?? string.Empty,
                            Rate = s.ServiceProvider.Rate
                        }
                    })
                    .FirstOrDefaultAsync();

                if (service != null)
                {
                    // Store data in the cache
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheDuration
                    };
                    _cache.Set(cacheKey, service, cacheOptions);
                }
            }

            return service;
        }

        // Helper method for building contracts safely
        private static ContractDTO? BuildAdminContract(AdminContract? contract)
        {
            return contract != null
                ? new ContractDTO
                {
                    Id = contract.ContractId,
                    Name = contract.ContractName ?? string.Empty, // Handle null values
                    Details = contract.Details ?? string.Empty
                }
                : null;
        }
        private static ContractDTO? BuildProviderContract(ProviderContract? contract)
        {
            return contract != null
                ? new ContractDTO
                {
                    Id = contract.ContractId,
                    Name = contract.ContractName ?? string.Empty, // Handle null values
                    Details = contract.Details ?? string.Empty
                }
                : null;
        }
    }

}
