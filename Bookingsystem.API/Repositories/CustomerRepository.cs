using BookingSystem.API.Data;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer?> GetCustomerByFirstNameAsync(string firstName)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.FirstName == firstName);
        }

        public async Task<Customer?> GetCustomerByLastNameAsync(string lastName)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.LastName == lastName);
        }

        public async Task<Customer?> GetCustomerByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
        }

        public async Task DeleteCustomerAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
