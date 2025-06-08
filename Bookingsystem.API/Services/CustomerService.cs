using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using Fjordingarnas_Bokningssystem.Models;

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

        public async Task<string> CreateCustomerAsync(int id, string firstName, string lastName, string phoneNumber)
        {
            var customer = new Customer
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            };

            await _customerRepository.AddCustomerAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return $"Customer {customer.FirstName} {customer.LastName} was created.";
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(int id, CustomerDto customerDto)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return null;
            }

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.PhoneNumber = customerDto.PhoneNumber;

            await _customerRepository.UpdateCustomerAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber
            };
        }

        public async Task<string?> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return null;
            }

            await _customerRepository.DeleteCustomerAsync(customer);
            await _customerRepository.SaveChangesAsync();

            return $"Customer with ID: {id} has been deleted.";
        }
    }
}
