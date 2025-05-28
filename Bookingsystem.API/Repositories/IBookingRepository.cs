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
    }
}
