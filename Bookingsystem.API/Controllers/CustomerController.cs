using BookingSystem.API.Data;
using Fjordingarnas_Bokningssystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(int id, string firstName, string lastName, string phoneNumber)
        {
            _context.Customers.Add(new Customer
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber
            });

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
