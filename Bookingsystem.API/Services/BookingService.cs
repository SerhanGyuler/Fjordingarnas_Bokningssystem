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
    }
}
