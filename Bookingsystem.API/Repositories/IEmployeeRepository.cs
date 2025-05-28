using Fjordingarnas_Bokningssystem.Models;

namespace BookingSystem.API.Repositories
{
    public interface IEmployeeRepository
    {
        public Task<Employee?> GetByIdAsync(int id);
    }
}
