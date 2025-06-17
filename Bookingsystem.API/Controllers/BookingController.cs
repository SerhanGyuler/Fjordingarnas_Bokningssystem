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


        [HttpGet("GetAvailableTimes")]
        public async Task<IActionResult> GetAvailableTimes([FromQuery] int serviceId, [FromQuery] int employeeId)
        {
            var result = await _bookingService.GetAvailableBookingSpotsAsync(serviceId, employeeId);

            if (result == null)
                return NotFound("Frisören eller tjänsten hittades inte.");

            if (!result.Any())
                return Ok(new List<AvailableTimeSpotDto>());

            return Ok(result);
        }


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