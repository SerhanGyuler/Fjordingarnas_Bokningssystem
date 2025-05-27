using BookingSystem.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class BookingController : ControllerBase
        {
            private readonly AppDbContext _context;

            public BookingController(AppDbContext context)
            {
                _context = context;
            }
            
            [HttpGet("AvailableBookingSpots")]
            public async Task<IActionResult> GetAvailableTimes([FromQuery] int serviceId, [FromQuery] int employeeId)
            {
                // get the chosen service
                var service = await _context.Services.FindAsync(serviceId);
                if (service == null)
                {
                    return NotFound("Denna tjänsten hittades inte.");
                }

                // Get the chosen employee & bookings & services
                var employee = await _context.Employees
                    .Include(e => e.Bookings)
                    .Include(e => e.Services)
                    .FirstOrDefaultAsync(e => e.Id == employeeId);

                if (employee == null)
                {
                    return NotFound("Frisören hittades inte.");
                }
                // Controll that the chosen barber has chosen service
                if (!employee.Services.Any(s => s.Id == serviceId))
                {
                    return BadRequest("Vald frisör erbjuder inte denna tjänsten.");
                }

                var availableSpots = new List<AvailableTimeSpotDto>();
                var today = DateTime.Today;

                // Check timespots 30 days ahead
                for (int daysAhead = 1; daysAhead <= 30; daysAhead++)
                {
                   var date = today.AddDays(daysAhead);

                   // Check for available spots every 30 minutes from 09:00 - 16:00
                   for (var time = new TimeSpan(9, 0, 0); time + service.Duration <= new TimeSpan(16, 0, 0); time += TimeSpan.FromMinutes(30))
                   {
                        var startDateTime = date.Add(time);
                        var endDateTime = startDateTime.Add(service.Duration);

                        // Check if time already is occupied
                        var isOccupied = employee.Bookings
                            .Any(b => !b.IsCancelled &&
                            b.StartTime < endDateTime &&
                            b.EndTime > startDateTime);

                            // add as available time
                        if (!isOccupied)
                        {

                            availableSpots.Add(new AvailableTimeSpotDto
                            {
                                EmployeeId = employee.Id,
                                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                                StartTime = startDateTime,
                                EndTime = endDateTime
                            });
                        }
                   }
                }
                return Ok(availableSpots);
            }
        }
    }