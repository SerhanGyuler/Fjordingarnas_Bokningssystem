using BookingSystem.API.Data;
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
            var customer = await _context.Customers
                .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.PhoneNumber,
                })
                .ToListAsync();

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }


        // GET customers via id
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Customer>> GetCustomerViaId(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound($"Customer with ID '{id}' does not exist.");
            }

            return Ok(customer);
        }

        // GET customers via FirstName
        [HttpGet("firstname/{FirstName}")]
        public async Task<ActionResult<Customer>> GetCustomerViaFirstName(string FirstName)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.FirstName == FirstName);

            if (customer == null)
            {
                return NotFound($"Customer with FirstName '{FirstName}' does not exist.");
            }

            return Ok(customer);
        }

        // GET customers via LastName
        [HttpGet("lastname/{LastName}")]
        public async Task<ActionResult<Customer>> GetCustomerViaLastName(string LastName)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.LastName == LastName);

            if (customer == null)
            {
                return NotFound($"Customer with LastName '{LastName}' does not exist.");
            }

            return Ok(customer);
        }

        // GET customers via PhoneNumber
        [HttpGet("phonenumber/{PhoneNumber}")]
        public async Task<ActionResult<Customer>> GetCustomerViaPhoneNumber(string PhoneNumber)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == PhoneNumber);

            if (customer == null)
            {
                return NotFound($"Customer with PhoneNumber '{PhoneNumber}' does not exist.");
            }

            return Ok(customer);
        }

        // POST create a new customer
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(int id, string firstName, string lastName, string phoneNumber)
        {
            var customer =_context.Customers.Add(new Customer
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        // PUT update an existing customer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            if (id != updatedCustomer.Id)
            {
                return BadRequest("Customer ID mismatch.");
            }

            var existingCustomer = await _context.Customers.FindAsync(id);

            if (existingCustomer == null)
            {
                return NotFound($"Customer with ID {id} not found.");
            }

            // Update fields
            existingCustomer.FirstName = updatedCustomer.FirstName;
            existingCustomer.LastName = updatedCustomer.LastName;
            existingCustomer.PhoneNumber = updatedCustomer.PhoneNumber;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            return Ok(existingCustomer);
        }

        // DELETE customer by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id, string FirstName, string LastName)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound($"Customer with ID {id} not found.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok($"Customer with ID {id} has been deleted.");
        }

    }
} 
