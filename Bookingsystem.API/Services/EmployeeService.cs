using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using Fjordingarnas_Bokningssystem.Models;

namespace BookingSystem.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDateTimeService _dateTimeProvider;
        private readonly IBookingRepository _bookingRepository;

        public EmployeeService(IDateTimeService dateTimeProvider, IBookingRepository bookingRepository)
        {
            _dateTimeProvider = dateTimeProvider;
            _bookingRepository = bookingRepository;
        }

        public (DateTime? StartDate, DateTime? EndDate) GetPeriodDates(string? period)
        {
            if (string.IsNullOrEmpty(period))
                return (null, null);

            if (period.Equals("day", StringComparison.OrdinalIgnoreCase))
            {
                return (_dateTimeProvider.Today, _dateTimeProvider.Today.AddDays(1));
            }
            else if (period.Equals("week", StringComparison.OrdinalIgnoreCase))
            {
                return (_dateTimeProvider.Today, _dateTimeProvider.Today.AddDays(7));
            }
            else
            {
                throw new ArgumentException($"Ogiltig periodparameter: '{period}'. Tillåtna värden är 'day' eller 'week'.");
            }
        }

        public async Task<List<BookingDto>> GetBookingsForEmployeeAsync(int employeeId, string? period)
        {
            var (startDate, endDate) = GetPeriodDates(period);

            var bookings = await _bookingRepository.GetBookingsForEmployeeAsync(employeeId, startDate, endDate);

            if (bookings == null || bookings.Count == 0)
            {
                return new List<BookingDto>(); // return empty list so controller can  handle NotFound
            }

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                IsCancelled = b.IsCancelled,
                CustomerName = b.Customer != null ? $"{b.Customer.FirstName} {b.Customer.LastName}" : string.Empty,
                EmployeeName = b.Employee != null ? $"{b.Employee.FirstName} {b.Employee.LastName}" : string.Empty,
                Services = b.Services.Select(s => s.ServiceName ?? string.Empty).ToList()
            }).ToList();
        }
    }
}
