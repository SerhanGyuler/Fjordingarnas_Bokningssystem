using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(
            IBookingRepository bookingRepository,
            IEmployeeRepository employeeRepository,
            IEmployeeService employeeService)
        {
            _bookingRepository = bookingRepository;
            _employeeRepository = employeeRepository;
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetEmployeeDto>>> GetEmployees()
        {
            var employee = await _employeeRepository.GetAllAsync();

            if (employee == null)
            {
                return NotFound("No employees found.");
            }

            var employeeDtos = employee.Select(e => new GetEmployeeDto
            {
                Id = e.Id,
                Name = e != null ? $"{e.FirstName} {e.LastName}" : "Unknown Employee",
                PhoneNumber = e.PhoneNumber,
                Services = e.Services.Select(s => s.ServiceName!).ToList(),
            });

            return Ok(employeeDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetEmployeeDto>> GetEmployeeById(int id)
        {
            var employee = await _employeeRepository.GetByIdWithServicesAsync(id);

            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            var employeeDto = new GetEmployeeDto
            {
                Id = employee.Id,
                Name = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown Employee",
                PhoneNumber = employee.PhoneNumber,
                Services = employee.Services.Select(s => s.ServiceName!).ToList(),
            };

            return Ok(employeeDto);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetBookingsForEmployee(int employeeId, [FromQuery] string? period)
        {
            var (startDate, endDate) = _employeeService.GetPeriodDates(period);

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
