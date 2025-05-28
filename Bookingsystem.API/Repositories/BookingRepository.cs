using BookingSystem.API.Data;
using BookingSystem.API.Models.DTOs;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Employee)
                .Include(b => b.Services)
                .ToListAsync();
        }
         
        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Employee)
                .Include(b => b.Services)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking?> GetByIdWithServicesAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Services)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Booking?> CreateBookingAsync(BookingInputDto bookingDto)
        {
            var customer = await _context.Customers.FindAsync(bookingDto.CustomerId);
            var employee = await _context.Employees.FindAsync(bookingDto.EmployeeId);
            var services = await _context.Services
                .Where(s => bookingDto.ServiceIds.Contains(s.Id))
                .ToListAsync();

            if (customer == null || employee == null || services.Count != bookingDto.ServiceIds.Count)
                return null;

            var newBooking = new Booking
            {
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime,
                IsCancelled = false,
                CustomerId = bookingDto.CustomerId,
                EmployeeId = bookingDto.EmployeeId,
                Services = services
            };

            await _context.Bookings.AddAsync(newBooking);
            await _context.SaveChangesAsync();

            return newBooking;
        }
    }
}
