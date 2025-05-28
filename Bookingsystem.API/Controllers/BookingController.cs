using BookingSystem.API.Data;
using BookingSystem.API.Models;
using BookingSystem.API.Models.DTOs;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // GET all bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings() 
        {
            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Employee)
                .Include(b => b.Services)
                .ToListAsync();

            var bookingDtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                IsCancelled = b.IsCancelled,
                CustomerName = b.Customer != null ? $"{b.Customer.FirstName} {b.Customer.LastName}" : "Unknown Customer",
                EmployeeName = b.Employee != null ? $"{b.Employee.FirstName} {b.Employee.LastName}" : "Unknown Employee",
                Services = [.. b.Services.Select(s => s.ServiceName)]
            });

            return Ok(bookingDtos);
        }

        // GET booking by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Employee)
                .Include(b => b.Services)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                IsCancelled = booking.IsCancelled,
                CustomerName = booking.Customer != null ? $"{booking.Customer.FirstName} {booking.Customer.LastName}" : "Unknown Customer",
                EmployeeName = booking.Employee != null ? $"{booking.Employee.FirstName} {booking.Employee.LastName}" : "Unknown Employee",
                Services = [.. booking.Services.Select(s => s.ServiceName)]
            };

            return Ok(bookingDto);
        }


        // POST (Create Booking)
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking([FromBody] BookingInputDto bookingDto)
        {
            // Hämta kopplingar från databasen
            var customer = await _context.Customers.FindAsync(bookingDto.CustomerId);
            var employee = await _context.Employees.FindAsync(bookingDto.EmployeeId);
            var services = await _context.Services.Where(s => bookingDto.ServiceIds.Contains(s.Id)).ToListAsync();

            if (customer == null || employee == null || services.Count != bookingDto.ServiceIds.Count)
                return BadRequest("Unknown CustomerId, EmployeeId or ServiceIds.");

            var newBooking = new Booking
            {
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime,
                IsCancelled = false,
                CustomerId = bookingDto.CustomerId,
                EmployeeId = bookingDto.EmployeeId,
                Services = services
            };

            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookingById), new { id = newBooking.Id }, newBooking);
        }

        // DELETE booking by Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound($"Booking with ID {id} not found.");

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok($"Booking with ID {id} has been deleted.");
        }

        // PUT update booking


        // PATCH cancel booking by Id


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

        //GET Overview of bookings by week or month
        [HttpGet("bookings/overview")]
        public async Task<IActionResult> GetBookingsOverview([FromQuery] string range = "week")
        {
            var now = DateTime.Now;
            var startDateWeek = now.AddDays(-(int)now.DayOfWeek);
            var endDateWeek = startDateWeek.AddDays(7);
            var startDateMonth = new DateTime(now.Year, now.Month, 1);
            var endDateMonth = startDateMonth.AddMonths(1);

            DateTime startDate, endDate;

            if (range.ToLower() == "month")
            {
                startDate = startDateMonth;
                endDate = endDateMonth;
            }
            else
            {
                startDate = startDateWeek;
                endDate = endDateWeek;
            }

            return Ok();
        }
    }
    }