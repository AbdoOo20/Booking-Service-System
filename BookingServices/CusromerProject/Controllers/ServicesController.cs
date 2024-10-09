using BookingServices.Data;
using CusromerProject.DTO.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CusromerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly ServiceRepository _serviceRepository;

        public ServicesController(ServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        // 1. Get all services
        [HttpGet("all")]
        public async Task<IActionResult> GetAllServices()
        {
            try
            {
                var services = await _serviceRepository.GetAllServicesAsync();
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // 2. Get service by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            try
            {
                var service = await _serviceRepository.GetServiceByIdAsync(id);
                if (service == null)
                    return NotFound(new { message = "Service not found" });

                return Ok(service);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
