using BookingSystem.API.Data;
using BookingSystem.API.Models;
using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingService _bookingService;

        public BookingController(
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            IEmployeeRepository employeeRepository,
            IServiceRepository serviceRepository,
            IBookingService bookingService)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _serviceRepository = serviceRepository;
            _bookingService = bookingService;
        }

        // GET all bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var bookingDtos = await _bookingService.GetAllBookingDtosAsync();
            
            if (!bookingDtos.Any())
                return NotFound("No bookings found");
            
            return Ok(bookingDtos);
        }

        // GET booking by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            return Ok(booking);
        }


        // POST (Create Booking)
        [HttpPost]
        public async Task<ActionResult<NewBookingDto>> CreateBooking([FromBody] BookingInputDto bookingInput)
        {
            (NewBookingDto booking, string error) = await _bookingService.CreateBookingAsync(bookingInput);

            if (booking is null)
                return BadRequest(error ?? "Unknown error.");

            return Ok(booking);
        }


        // DELETE booking by Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                await _bookingService.DeleteAsync(id);
                return Ok($"Booking with ID {id} has been deleted.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT update booking      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingInputDto bookingDto)
        {
            var (success, error, result) = await _bookingService.UpdateBookingAsync(id, bookingDto);

            if (!success)
            {
                if (error?.Contains("Not found") == true)
                {
                    return NotFound(error);
                }

                return BadRequest("Unknown error");
            }

            return Ok(result);
        }


        // PATCH cancel booking by Id
        [HttpPatch("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");

            }

            booking.IsCancelled = true;
            await _bookingRepository.SaveChangesAsync();

            return Ok($"Booking with ID {id} has been cancelled.");
        }

        // PATCH update booking status by Id
        [HttpPatch("reschedule/{id}")]
        public async Task<IActionResult> RescheduleBooking(int id, [FromBody] RescheduleBookingDto dto)
        {
            var (success, error) = await _bookingService.RescheduleBookingAsync(id, dto);

            if (!success) 
            {
                if (error?.Contains("Not found") == true)
                {
                    return NotFound(error);
                }
                else if (error?.Contains("reschedule") == true)
                {
                    return BadRequest("Cannot reschedule a cancelled booking.");
                }
            }

            return Ok($"Booking with ID {id} has been rescheduled.");
        }


        //[HttpGet("AvailableBookingSpots")]
        //public async Task<IActionResult> GetAvailableTimes([FromQuery] int serviceId, [FromQuery] int employeeId)
        //{
        //    // get the chosen service
        //    var service = await _context.Services.FindAsync(serviceId);
        //    if (service == null)
        //    {
        //        return NotFound("Denna tjänsten hittades inte.");
        //    }

        //    // Get the chosen employee & bookings & services
        //    var employee = await _context.Employees
        //        .Include(e => e.Bookings)
        //        .Include(e => e.Services)
        //        .FirstOrDefaultAsync(e => e.Id == employeeId);

        //    if (employee == null)
        //    {
        //        return NotFound("Frisören hittades inte.");
        //    }
        //    // Controll that the chosen barber has chosen service
        //    if (!employee.Services.Any(s => s.Id == serviceId))
        //    {
        //        return BadRequest("Vald frisör erbjuder inte denna tjänsten.");
        //    }

        //    var availableSpots = new List<AvailableTimeSpotDto>();
        //    var today = DateTime.Today;

        //    // Check timespots 30 days ahead
        //    for (int daysAhead = 1; daysAhead <= 30; daysAhead++)
        //    {
        //        var date = today.AddDays(daysAhead);

        //        // Check for available spots every 30 minutes from 09:00 - 16:00
        //        for (var time = new TimeSpan(9, 0, 0); time + service.Duration <= new TimeSpan(16, 0, 0); time += TimeSpan.FromMinutes(30))
        //        {
        //            var startDateTime = date.Add(time);
        //            var endDateTime = startDateTime.Add(service.Duration);

        //            // Check if time already is occupied
        //            var isOccupied = employee.Bookings
        //                .Any(b => !b.IsCancelled &&
        //                b.StartTime < endDateTime &&
        //                b.EndTime > startDateTime);

        //            // add as available time
        //            if (!isOccupied)
        //            {

        //                availableSpots.Add(new AvailableTimeSpotDto
        //                {
        //                    EmployeeId = employee.Id,
        //                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
        //                    StartTime = startDateTime,
        //                    EndTime = endDateTime
        //                });
        //            }
        //        }
        //    }
        //    return Ok(availableSpots);
        //}


        // WIP POSTMAN
        //GET Overview of bookings by week or month
        [HttpGet("bookings/overview")]
        public async Task<IActionResult> GetBookingsOverview([FromQuery] string range = "week")
        {
            var overview = await _bookingService.GetBookingsOverviewAsync(range);
            return Ok(overview);
        }

        [HttpGet("price")]
        public async Task<IActionResult> GetPriceOfBooking(int id)
        {
            var priceOverview = await _bookingService.GetPriceOfBookingAsync(id);
            if (priceOverview == null)
                return NotFound($"Booking with ID {id} not found.");

            return Ok(priceOverview);
        }
    }
}