using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository; ;
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

    }
}
