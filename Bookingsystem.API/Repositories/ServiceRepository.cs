using BookingSystem.API.Data;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;

        public ServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Services
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

        public async Task<List<Service>> GetServicesByBookingIdAsync(int bookingId)
        {
            return await _context.Services
                .Where(s => s.Bookings.Any(b => b.Id == bookingId))
                .ToListAsync();
        }
    }
}
