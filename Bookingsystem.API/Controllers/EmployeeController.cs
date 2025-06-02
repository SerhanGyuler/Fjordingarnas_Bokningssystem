using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller")]
    public class EmployeeController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public EmployeeController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetBookingsForEmployee(int employeeId, [FromQuery] string? period)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (!string.IsNullOrEmpty(period))
            {
                if(period.Equals("day", StringComparison.OrdinalIgnoreCase))
                {
                    // Todays bookings
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1);
                }
                else if (period.Equals("week", StringComparison.OrdinalIgnoreCase))
                {
                    // From today to 7 days ahead
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(7);
                }
            }

            var bookings = await _bookingRepository.GetBookingsForEmployeeAsync(employeeId, startDate, endDate);

            if (bookings == null || bookings.Count == 0)
            {
                return NotFound("Inga bokningar hittades för denna frisören.");
            }

            var bookingDtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                IsCancelled = b.IsCancelled,
                CustomerName = b.Customer != null ? $"{b.Customer.FirstName} {b.Customer.LastName}" : string.Empty,
                EmployeeName = b.Employee != null ? $"{b.Employee.FirstName} {b.Employee.LastName}" : string.Empty,
                Services = b.Services.Select(s => s.ServiceName ?? string.Empty).ToList()
            }).ToList();

            return Ok(bookingDtos);
        }
    }
}
