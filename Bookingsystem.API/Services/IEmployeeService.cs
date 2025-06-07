using BookingSystem.API.Models.DTOs;

namespace BookingSystem.API.Services
{
    public interface IEmployeeService
    {
        (DateTime? StartDate, DateTime? EndDate) GetPeriodDates(string? period);
        Task<List<BookingDto>> GetBookingsForEmployeeAsync(int employeeId, string? period);
    }
}
