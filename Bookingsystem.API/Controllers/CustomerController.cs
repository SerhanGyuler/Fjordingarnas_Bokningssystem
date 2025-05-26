using BookingSystem.API.Data;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.API.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]

    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // GET all customers
        [HttpGet]
        public async Task<ActionResult<ICollection<Customer>>> GetCustomers()
        {
            var customer = await _context.Customers
                .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.PhoneNumber
                })
                .ToListAsync();

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }


        // GET customers via id
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
    }
}
