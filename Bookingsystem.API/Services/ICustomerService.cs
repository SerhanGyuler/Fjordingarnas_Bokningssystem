using BookingSystem.API.Models.DTOs;
using Fjordingarnas_Bokningssystem.Models;

namespace BookingSystem.API.Services
{
    public interface ICustomerService
    {
        public Task<List<CustomerDto>> GetAllCustomerDtosAsync();
        public Task<CustomerDto?> GetCustomerDtoByIdAsync(int id);
        public Task<CustomerDto?> GetCustomerByFirstNameAsync(string firstName);
        public Task<CustomerDto?> GetCustomerByLastNameAsync(string lastName);
        public Task<string> CreateCustomerAsync(int id, string firstName, string lastName, string phoneNumber);
        public Task<CustomerDto?> GetCustomerByPhoneNumberAsync(string phoneNumber);
        public Task<CustomerDto?> UpdateCustomerAsync(int id, CustomerDto customerDto);
        public Task<string?> DeleteCustomerAsync(int id);
    }
}
