using Fjordingarnas_Bokningssystem.Models;

namespace BookingSystem.API.Repositories
{
    public interface ICustomerRepository
    {
        public Task<ICollection<Customer>> GetAllCustomersAsync();
        public Task<Customer?> GetCustomerByIdAsync(int id);
        public Task<Customer?> GetCustomerByFirstNameAsync(string firstName);
        public Task<Customer?> GetCustomerByLastNameAsync(string lastName);
        public Task<Customer?> GetCustomerByPhoneNumberAsync(string phoneNumber);
        public Task AddCustomerAsync(Customer customer);
        public Task UpdateCustomerAsync(Customer customer);
        public Task DeleteCustomerAsync(Customer customer);
        public Task<bool> SaveChangesAsync();
    }
}
