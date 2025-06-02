using BookingSystem.API.Data;
using BookingSystem.API.Models;
using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
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

        public BookingController(
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

        // GET all bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            var booking = await _bookingRepository.GetAllAsync();

            if (booking == null)
            {
                return NotFound("No bookings found.");
            }

            var bookingDtos = booking.Select(b => new BookingDto
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
            var booking = await _bookingRepository.GetByIdAsync(id);

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
        public async Task<ActionResult<NewBookingDto>> CreateBooking([FromBody] BookingInputDto NewBookingDto)
        {
            var newBooking = await _bookingRepository.CreateBookingAsync(NewBookingDto);

            if (newBooking == null)
            {
                return BadRequest("Unknown CustomerId, EmployeeId or ServiceIds.");
            }

            var bookingDtoOut = new NewBookingDto
            {
                Id = newBooking.Id,
                StartTime = newBooking.StartTime,
                EndTime = newBooking.EndTime,
                IsCancelled = newBooking.IsCancelled,
                Customer = new CustomerDto
                {
                    FirstName = newBooking.Customer.FirstName,
                    LastName = newBooking.Customer.LastName,
                    PhoneNumber = newBooking.Customer.PhoneNumber
                },
                Employee = new EmployeeDto
                {
                    FirstName = newBooking.Employee.FirstName,
                    LastName = newBooking.Employee.LastName,
                    PhoneNumber = newBooking.Employee.PhoneNumber
                },
                Services = newBooking.Services.Select(s => new ServiceDto
                {
                    ServiceName = s.ServiceName,
                    Duration = s.Duration,
                    Price = s.Price
                }).ToList()
            };

            return Ok(bookingDtoOut);
        }


        // DELETE booking by Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            await _bookingRepository.DeleteAsync(booking);
            await _bookingRepository.SaveChangesAsync();
            return Ok($"Booking with ID {id} has been deleted.");
        }

        // PUT update booking      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingInputDto bookingDto)
        {
            var existingBooking = await _bookingRepository.GetByIdWithServicesAsync(id);

            if (existingBooking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            var customer = await _customerRepository.GetCustomerByIdAsync(bookingDto.CustomerId);
            var employee = await _employeeRepository.GetByIdAsync(bookingDto.EmployeeId);
            var services = await _serviceRepository.GetByIdsAsync(bookingDto.ServiceIds);

            if (customer == null || employee == null || services.Count() != bookingDto.ServiceIds.Count)
            {
                return BadRequest("Unknown CustomerId, EmployeeId or ServiceIds.");
            }

            // Update fields
            existingBooking.StartTime = bookingDto.StartTime;
            existingBooking.EndTime = bookingDto.EndTime;
            existingBooking.CustomerId = bookingDto.CustomerId;
            existingBooking.EmployeeId = bookingDto.EmployeeId;
            existingBooking.Services = services.ToList();

            await _bookingRepository.UpdateAsync(existingBooking);
            var result = await _bookingRepository.SaveChangesAsync();

            if (result == false)
            {
                return StatusCode(500, "Failed to update booking");
            }

            var responseDto = new BookingDto
            {
                Id = existingBooking.Id,
                StartTime = existingBooking.StartTime,
                EndTime = existingBooking.EndTime,
                IsCancelled = existingBooking.IsCancelled,
                CustomerName = customer != null ? $"{customer.FirstName} {customer.LastName}" : "Unknown Customer",
                EmployeeName = employee != null ? $"{employee.FirstName} {employee.LastName}" : "Unknown Employee",
                Services = [.. services.Select(s => s.ServiceName)]
            };

            return Ok(responseDto);
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
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }

            if (booking.IsCancelled)
            {
                return BadRequest("Cannot reschedule a cancelled booking.");
            }

            booking.StartTime = dto.NewStartTime;
            booking.EndTime = dto.NewEndTime;

            await _bookingRepository.SaveChangesAsync();

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
            var now = DateTime.UtcNow;

            var startDateWeek = now.Date.AddDays(-(int)now.DayOfWeek);
            var endDateWeek = startDateWeek.AddDays(7);

            var startDateMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
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

            var bookings = await _bookingRepository.GetBookingsInDateRangeAsync(startDate, endDate);


            var bookingsGrouped = bookings
                .Select(b => new BookingOverviewDto
                {
                    Date = b.StartTime.Date,
                    StartTime = b.StartTime.ToString("HH:mm"),
                    EndTime = b.EndTime.ToString("HH:mm"),
                    Services = b.Services.Select(s => s.ServiceName!).ToArray(),
                    Employee = b.Employee!.FirstName + " " + b.Employee.LastName,
                    Customer = b.Customer!.FirstName + " " + b.Customer.LastName,
                })
                .GroupBy(b => b.Date)
                .ToDictionary(
                    g => g.Key.ToString("yyyy-MM-dd"),
                    g => g.ToList()
                );

            return Ok(new
            {
                Range = range.ToLower(),
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
                Bookings = bookingsGrouped
            });
        }

        [HttpGet("calculate/price")]
        public async Task<IActionResult> GetPriceOfBooking(int id)
        {
            var services = await _serviceRepository.GetServicesByBookingIdAsync(id);

            var servicePrices = services
                .Select(s => new BookingPriceDto
                {
                    ServiceName = s.ServiceName,
                    Price = s.Price
                });

            return Ok(new
            {
                Prices = servicePrices,
                Total = servicePrices.Sum(s => s.Price)
            });
        }
    }
}