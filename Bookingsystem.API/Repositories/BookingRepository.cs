﻿using BookingSystem.API.Data;
using BookingSystem.API.Models.DTOs;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
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

        // CRUD
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

        public async Task<(Booking? booking, string? error)> CreateBookingAsync(BookingInputDto bookingDto)
        {
            var startTimeUtc = DateTime.SpecifyKind(bookingDto.StartTime, DateTimeKind.Utc);
            var endTimeUtc = DateTime.SpecifyKind(bookingDto.EndTime, DateTimeKind.Utc);

            if (await _context.Bookings.AnyAsync(b => 
            b.StartTime < bookingDto.EndTime && 
            b.EndTime > bookingDto.StartTime &&
            b.Employee.Id == bookingDto.EmployeeId &&
            !b.IsCancelled))
            {
                return (null, "Time slot not available for this employee");
            }
            
            var customer = await _context.Customers.FindAsync(bookingDto.CustomerId);
            var employee = await _context.Employees.FindAsync(bookingDto.EmployeeId);
            var services = await _context.Services
                .Where(s => bookingDto.ServiceIds.Contains(s.Id))
                .ToListAsync();

            if (customer == null || employee == null || services.Count != bookingDto.ServiceIds.Count)
            {
                return (null, "Unknown CustomerId, EmployeeId or ServiceIds.");
            }

            var newBooking = new Booking
            {
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime,
                IsCancelled = false,
                Customer = customer,
                Employee = employee,
                Services = services
            };

            await _context.Bookings.AddAsync(newBooking);
            await _context.SaveChangesAsync();

            return (newBooking, null);
        }

        public async Task<List<Booking>> GetBookingsInDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Include(b => b.Services)
                .Include(b => b.Customer)
                .Include(b => b.Employee)
                .Where(b => b.StartTime >= startDate && b.StartTime < endDate && !b.IsCancelled)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsForEmployeeAsync(int employeeId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Bookings
                .Include(b => b.Services)
                .Include(b => b.Customer)
                .Include(b => b.Employee)
                .Where(b => b.EmployeeId == employeeId && !b.IsCancelled);

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(b => b.StartTime >= startDate.Value && b.StartTime < endDate.Value);
            }

            return await query.OrderBy(b => b.StartTime).ToListAsync();
        }

        public async Task<List<Booking>> GetEmployeeBookingsAsync(int employeeId, DateTime from, DateTime to)
        {
            return await _context.Bookings
        .Where(b => b.EmployeeId == employeeId
        && !b.IsCancelled
            && b.StartTime < to
            && b.EndTime > from)
        .ToListAsync();
        }

    }
}
