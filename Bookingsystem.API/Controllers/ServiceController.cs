using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IServiceRepository _serviceRepository;

        public ServiceController(
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            IEmployeeRepository employeeRepository,
            IServiceRepository serviceRepository)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _serviceRepository = serviceRepository;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceDto>> CreateService([FromBody] ServiceInputDto serviceInput)
        {
            var newService = new Service
            {
                ServiceName = serviceInput.ServiceName,
                Duration = serviceInput.Duration,
                Price = serviceInput.Price
            };

            var createdService = await _serviceRepository.AddAsync(newService);

            if (createdService == null)
            {
                return BadRequest("Could not create service.");
            }

            var serviceDto = new CreatedServiceDto
            {
                Id = createdService.Id,
                ServiceName = createdService.ServiceName!,
                Duration = createdService.Duration,
                Price = createdService.Price
            };

            return Ok(serviceDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var deleted = await _serviceRepository.DeleteServiceAsync(id);

            if (!deleted)
            {
                return NotFound($"No service found with id {id}.");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceInputDto updateDto)
        {
            var existingService = await _serviceRepository.GetServiceByIdAsync(id);

            if (existingService == null)
            {
                return NotFound($"No service found with id {id}.");
            }

            existingService.ServiceName = updateDto.ServiceName;
            existingService.Duration = updateDto.Duration;
            existingService.Price = updateDto.Price;

            var updated = await _serviceRepository.UpdateServiceAsync(existingService);

            if (!updated)
            {
                return StatusCode(500, "Failed to update service.");
            }

            return NoContent();
        }
    }
}
