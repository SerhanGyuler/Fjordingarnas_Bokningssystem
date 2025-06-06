using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;

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
    }
}
