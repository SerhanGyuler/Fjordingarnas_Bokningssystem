using BookingSystem.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Services
{
    public interface IBookingService
    {
        public Task<IEnumerable<BookingDto>> GetAllBookingDtosAsync();
        public Task<BookingDto> GetBookingByIdAsync(int id);
        public Task<NewBookingDto?> CreateBookingAsync(BookingInputDto input);
        public Task DeleteAsync(int id);
        public Task<(bool Success, string? Error, BookingDto? Result)> UpdateBookingAsync(int id, BookingInputDto bookingDto);
        public Task<(bool Success, string? Error)> RescheduleBookingAsync(int id, [FromBody] RescheduleBookingDto dto)
    }
}
