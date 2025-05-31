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

        public async Task<Service> AddAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
                return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task<bool> UpdateServiceAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
