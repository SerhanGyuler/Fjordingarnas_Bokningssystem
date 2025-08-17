using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
using Microsoft.AspNetCore.Mvc;
using Models;

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
            try
            {
                var bookingDtos = await _employeeService.GetBookingsForEmployeeAsync(employeeId, period);

                if (bookingDtos == null || bookingDtos.Count == 0)
                {
                    return NotFound("Inga bokningar hittades för denna frisören.");
                }

                return Ok(bookingDtos);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel i GetBookingsForEmployee: {ex.Message}");
                return StatusCode(500, "Ett oväntat fel inträffade.");
            }
        }

        [HttpPost("{employeeId}/unavailability")]
        public async Task<IActionResult> AddUnavailability(int employeeId, [FromBody] UnavailabilityDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return NotFound("Kunde inte hitta frisör.");

            var period = new UnavailablePeriod
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason
            };

            await _employeeRepository.AddUnavailabilityAsync(employeeId, period);

            return Ok("Otillgänglighet registrerat.");
        }

        [HttpGet("{employeeId}/unavailability")]
        public async Task<IActionResult> GetUnavailability(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return NotFound("Kunde inte hitta frisör.");

            var periods = await _employeeRepository.GetUnavailabilityAsync(employeeId);
            return Ok(periods);
        }
    }
}
