using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;

namespace BookingSystem.API.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<CustomerDto>> GetAllCustomerDtosAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();

            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PhoneNumber = c.PhoneNumber
            }).ToList();
        }
    }

}
