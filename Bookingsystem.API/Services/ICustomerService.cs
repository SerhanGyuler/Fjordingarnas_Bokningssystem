using BookingSystem.API.Models.DTOs;
using Fjordingarnas_Bokningssystem.Models;

namespace BookingSystem.API.Services
{
    public interface ICustomerService
    {
        public Task<IEnumerable<CustomerDto>> GetAllCustomerDtosAsync();
        public Task<Customer?> GetCustomerDtoByIdAsync(int id);
        public Task<Customer?> GetCustomerByFirstNameAsync(string firstName);
        public Task<Customer?> GetCustomerByLastNameAsync(string lastName);
        public Task<string> CreateCustomerAsync(int id, string firstName, string lastName, string phoneNumber);
        public Task<CustomerDto?> UpdateCustomerAsync(int id, CustomerDto customerDto);
        public Task<string?> DeleteCustomerAsync(int id);
    }
}
