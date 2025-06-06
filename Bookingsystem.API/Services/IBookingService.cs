using BookingSystem.API.Models.DTOs;

namespace BookingSystem.API.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllBookingDtosAsync();
    }
}
