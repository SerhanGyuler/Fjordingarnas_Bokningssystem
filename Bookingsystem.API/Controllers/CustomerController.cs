using BookingSystem.API.Data;
using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // GET all customers
        [HttpGet]
        public async Task<ActionResult<ICollection<Customer>>> GetAllCustomers()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            var result = customers.Select(c => new { c.Id, c.FirstName, c.LastName, c.PhoneNumber }).ToList();
            return Ok(result);
        }


        // GET customers via id
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Customer>> GetCustomerViaId(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound($"Customer with ID '{id}' not found.");
            }

            return Ok(customer);
        }

        // GET customers via FirstName
        [HttpGet("firstname/{FirstName}")]
        public async Task<ActionResult<Customer>> GetCustomerViaFirstName(string FirstName)
        {
            var customer = await _customerRepository.GetCustomerByFirstNameAsync(FirstName);

            if (customer == null)
            {
                return NotFound($"Customer with FirstName '{FirstName}' not found.");
            }

            return Ok(customer);
        }

        // GET customers via LastName
        [HttpGet("lastname/{LastName}")]
        public async Task<ActionResult<Customer>> GetCustomerViaLastName(string LastName)
        {
            var customer = await _customerRepository.GetCustomerByLastNameAsync(LastName);

            if (customer == null)
            {
                return NotFound($"Customer with LastName '{LastName}' not found.");
            }

            return Ok(customer);
        }

        // GET customers via PhoneNumber
        [HttpGet("phonenumber/{PhoneNumber}")]
        public async Task<ActionResult<Customer>> GetCustomerViaPhoneNumber(string PhoneNumber)
        {
            var customer = await _customerRepository.GetCustomerByPhoneNumberAsync(PhoneNumber);

            if (customer == null)
            {
                return NotFound($"Customer with PhoneNumber '{PhoneNumber}' not found.");
            }

            return Ok(customer);
        }

        // POST create a new customer
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(int id, string firstName, string lastName, string phoneNumber)
        {
            var customer = new Customer { Id = id, FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber };
            await _customerRepository.AddCustomerAsync(customer);
            await _customerRepository.SaveChangesAsync();
            return Ok($"Customer {customer.FirstName} {customer.LastName} was created created.");
        }

        // PUT update an existing customer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerDto customerDto)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.PhoneNumber = customerDto.PhoneNumber;

            await _customerRepository.UpdateCustomerAsync(customer);
            await _customerRepository.SaveChangesAsync();

            var updatedDto = new CustomerDto
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber
            };

            return Ok(updatedDto);
        }

        // DELETE customer by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound($"Customer with ID {id} not found.");
            }

            await _customerRepository.DeleteCustomerAsync(customer);
            await _customerRepository.SaveChangesAsync();
            return Ok($"Customer with ID {id} has been deleted.");
        }

    }
} 
