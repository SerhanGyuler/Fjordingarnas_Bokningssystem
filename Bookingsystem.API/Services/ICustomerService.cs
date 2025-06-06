using BookingSystem.API.Models.DTOs;

namespace BookingSystem.API.Services
{
    public interface ICustomerService
    {
        public Task<IEnumerable<CustomerDto>> GetAllCustomerDtosAsync();
    }
}
