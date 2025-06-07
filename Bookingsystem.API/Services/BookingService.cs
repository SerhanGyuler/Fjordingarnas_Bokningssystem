using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            IEmployeeRepository employeeRepository,
            IServiceRepository serviceRepository
            )
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingDtosAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();

            if (bookings == null || !bookings.Any())
                return Enumerable.Empty<BookingDto>();

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                IsCancelled = b.IsCancelled,
                CustomerName = b.Customer != null ? $"{b.Customer.FirstName} {b.Customer.LastName}" : "Unknown Customer",
                EmployeeName = b.Employee != null ? $"{b.Employee.FirstName} {b.Employee.LastName}" : "Unknown Employee",
                Services = b.Services.Select(s => s.ServiceName).ToList()
            });
        }

        public async Task<BookingDto> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
                return null;

            return new BookingDto
            {
                Id = booking.Id,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                IsCancelled = booking.IsCancelled,
                CustomerName = booking.Customer != null ? $"{booking.Customer.FirstName} {booking.Customer.LastName}" : "Unknown Customer",
                EmployeeName = booking.Employee != null ? $"{booking.Employee.FirstName} {booking.Employee.LastName}" : "Unknown Employee",
                Services = [.. booking.Services.Select(s => s.ServiceName)]
            };
        }

        public async Task<NewBookingDto?> CreateBookingAsync(BookingInputDto input)
        {
            var newBooking = await _bookingRepository.CreateBookingAsync(input);
            if (newBooking is null) return null;

            // Manual mapping kept for now (zero external dependencies, minimal change).
            return new NewBookingDto
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
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking is null)
                throw new KeyNotFoundException($"Booking with ID {id} not found.");

            await _bookingRepository.DeleteAsync(booking);
            await _bookingRepository.SaveChangesAsync();
        }

        public async Task<(bool Success, string? Error, BookingDto? Result)> 
            UpdateBookingAsync(int id, BookingInputDto bookingDto)
        {
            var existingBooking = await _bookingRepository.GetByIdWithServicesAsync(id);
            if (existingBooking == null)
            {
                return (false, $"Booking with ID {id} not found.", null);
            }

            var customer = await _customerRepository.GetCustomerByIdAsync(bookingDto.CustomerId);
            var employee = await _employeeRepository.GetByIdAsync(bookingDto.EmployeeId);
            var services = await _serviceRepository.GetByIdsAsync(bookingDto.ServiceIds);

            if (customer == null || employee == null || services.Count() != bookingDto.ServiceIds.Count)
            {
                return (false, "Unknown CustomerId, EmployeeId or ServiceIds.", null);
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
                return (false, "Failed to update booking", null);
            }

            var responseDto = new BookingDto
            {
                Id = existingBooking.Id,
                StartTime = existingBooking.StartTime,
                EndTime = existingBooking.EndTime,
                IsCancelled = existingBooking.IsCancelled,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                Services = services.Select(s => s.ServiceName).ToList()
            };

            return (true, null, responseDto);
        }

        public async Task<(bool Success, string? Error)> RescheduleBookingAsync(int id, [FromBody] RescheduleBookingDto dto)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) 
            {
                return (false, $"Booking with {id} not found");
            }

            if (booking.IsCancelled)
            {
                return (false, $"Cannot reschedule a cancelled booking.");
            }

            booking.StartTime = dto.NewStartTime;
            booking.EndTime = dto.NewEndTime;

            await _bookingRepository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<BookingsOverviewResponseDto> GetBookingsOverviewAsync(string range)
        {
            var (startDate, endDate) = GetDateRange(range);

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

            return new BookingsOverviewResponseDto
            {
                Range = range.ToLower(),
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
                Bookings = bookingsGrouped
            };
        }

        //used in GetBookingsOverviewAsync
        private (DateTime startDate, DateTime endDate) GetDateRange(string range)
        {
            var now = DateTime.UtcNow;
            if (range.ToLower() == "month")
            {
                var startDateMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endDateMonth = startDateMonth.AddMonths(1);
                return (startDateMonth, endDateMonth);
            }
            else
            {
                var startDateWeek = now.Date.AddDays(-(int)now.DayOfWeek);
                var endDateWeek = startDateWeek.AddDays(7);
                return (startDateWeek, endDateWeek);
            }
        }

        public async Task<BookingPriceOverviewDto?> GetPriceOfBookingAsync(int bookingId)
        {
            var services = await _serviceRepository.GetServicesByBookingIdAsync(bookingId);

            if (services == null || !services.Any())
                return null;

            var servicePrices = services
                .Select(s => new BookingPriceDto
                {
                    ServiceName = s.ServiceName,
                    Price = s.Price
                })
                .ToList();

            return new BookingPriceOverviewDto
            {
                Prices = servicePrices,
                Total = servicePrices.Sum(s => s.Price)
            };
        }
    }
}
