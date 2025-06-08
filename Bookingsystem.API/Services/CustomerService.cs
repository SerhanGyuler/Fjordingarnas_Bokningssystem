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

        public async Task<CustomerDto?> GetCustomerDtoByIdAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber
            };
        }
        public async Task<CustomerDto?> GetCustomerByFirstNameAsync(string firstName)
        {
            var customer = await _customerRepository.GetCustomerByFirstNameAsync(firstName);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber
            };
        }

        public async Task<CustomerDto?> GetCustomerByLastNameAsync(string lastName)
        {
            var customer = await _customerRepository.GetCustomerByLastNameAsync(lastName);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber
            };
        }

        public async Task<CustomerDto?> GetCustomerByPhoneNumberAsync(string phoneNumber)
        {
            var customer = await _customerRepository.GetCustomerByPhoneNumberAsync(phoneNumber);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber
            };
        }
    }

}
