using BookingSystem.API.Models.DTOs;
using Fjordingarnas_Bokningssystem.Models;

namespace BookingSystem.API.Repositories
{
    public interface IBookingRepository
    {
        public Task<IEnumerable<Booking>> GetAllAsync();
        public Task<Booking?> GetByIdAsync(int id);
        public Task AddAsync(Booking booking);
        public Task DeleteAsync(Booking booking);
        public Task<bool> SaveChangesAsync();
        public Task<Booking?> CreateBookingAsync(BookingInputDto bookingDto);
        public Task<Booking?> GetByIdWithServicesAsync(int id);
        public Task UpdateAsync(Booking booking);
        public Task<List<Booking>> GetBookingsInDateRangeAsync(DateTime startDate, DateTime endDate);
        public Task<List<Booking>> GetBookingsForEmployeeAsync(int employeeId, DateTime? startDate, DateTime? endDate);
    }
}
