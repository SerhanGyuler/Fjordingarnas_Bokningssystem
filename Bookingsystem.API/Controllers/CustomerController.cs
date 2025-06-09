using BookingSystem.API.Data;
using BookingSystem.API.Models.DTOs;
using BookingSystem.API.Repositories;
using BookingSystem.API.Services;
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
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET all customers
        [HttpGet]
        public async Task<ActionResult<ICollection<CustomerDto>>> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomerDtosAsync();
            return Ok(customers);
        }

        // GET customers via ID
        [HttpGet("id/{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerViaId(int id)
        {
            var customer = await _customerService.GetCustomerDtoByIdAsync(id);

            if (customer == null)
            {
                return NotFound($"Customer with ID '{id}' not found.");
            }

            return Ok(customer);
        }

        // GET customers via FirstName
        [HttpGet("firstname/{firstName}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerViaFirstName(string firstName)
        {
            var customer = await _customerService.GetCustomerByFirstNameAsync(firstName);

            if (customer == null)
            {
                return NotFound($"Customer with FirstName '{firstName}' not found.");
            }

            return Ok(customer);
        }

        // GET customers via LastName
        [HttpGet("lastname/{lastName}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerViaLastName(string lastName)
        {
            var customer = await _customerService.GetCustomerByLastNameAsync(lastName);

            if (customer == null)
            {
                return NotFound($"Customer with LastName '{lastName}' not found.");
            }

            return Ok(customer);
        }

        // GET customers via PhoneNumber
        [HttpGet("phonenumber/{PhoneNumber}")]
        public async Task<ActionResult<Customer>> GetCustomerViaPhoneNumber(string phoneNumber)
        {
            var customer = await _customerService.GetCustomerByPhoneNumberAsync(phoneNumber);

            if (customer == null)
            {
                return NotFound($"Customer with PhoneNumber '{phoneNumber}' not found.");
            }

            return Ok(customer);
        }

        // POST create a new customer
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(int id, string firstName, string lastName, string phoneNumber)
        {
            var result = await _customerService.CreateCustomerAsync(id, firstName, lastName, phoneNumber);
            return Ok(result);
        }

        // PUT update an existing customer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerDto customerDto)
        {
            var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerDto);

            if (updatedCustomer == null)
            {
                return NotFound();
            }

            return Ok(updatedCustomer);
        }

        // DELETE customer by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);

            if (result == null)
            {
                return NotFound($"Customer with ID {id} was not found.");
            }

            return Ok(result);
        }
    }
} 
