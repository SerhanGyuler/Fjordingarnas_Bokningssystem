using BookingSystem.API.Models.DTOs;

namespace BookingSystem.API.Services
{
    public interface IBookingService
    {
        public Task<IEnumerable<BookingDto>> GetAllBookingDtosAsync();
        public Task<BookingDto> GetBookingByIdAsync(int id);
        public Task<NewBookingDto?> CreateBookingAsync(BookingInputDto input);
        public Task DeleteAsync(int id);
    }
}
