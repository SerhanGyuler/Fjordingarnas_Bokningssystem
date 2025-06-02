using Fjordingarnas_Bokningssystem.Models;

namespace BookingSystem.API.Repositories
{
    public interface IEmployeeRepository
    {
        public Task<Employee?> GetByIdAsync(int id);
        public Task<IEnumerable<Employee>> GetAllAsync();
        public Task<Employee> GetByIdWithServicesAsync(int id);
    }
}
