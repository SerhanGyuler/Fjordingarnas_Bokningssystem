using BookingSystem.API.Data;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.EntityFrameworkCore;
using Models;

namespace BookingSystem.API.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<Employee?> GetByIdWithServicesAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Services)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Services)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdWithBookingsServicesAndUnavailabilityAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Bookings)
                .Include(e => e.Services)
                .Include(e => e.UnavailablePeriods)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddUnavailabilityAsync(int employeeId, UnavailablePeriod period)
        {
            period.EmployeeId = employeeId;
            await _context.Set<UnavailablePeriod>().AddAsync(period);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UnavailablePeriod>> GetUnavailabilityAsync(int employeeId)
        {
            return await _context.Set<UnavailablePeriod>()
                .Where(p => p.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<bool> IsAvailableAsync(int employeeId, DateTime start, DateTime end)
        {
            return !await _context.Set<UnavailablePeriod>()
                .AnyAsync(p => p.EmployeeId == employeeId &&
                               start < p.EndDate &&
                               end > p.StartDate);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
