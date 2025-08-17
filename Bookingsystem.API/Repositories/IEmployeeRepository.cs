using Fjordingarnas_Bokningssystem.Models;
using Models;

namespace BookingSystem.API.Repositories
{
    public interface IEmployeeRepository
    {
        public Task<Employee?> GetByIdAsync(int id);
        public Task<IEnumerable<Employee>> GetAllAsync();
        public Task<Employee> GetByIdWithServicesAsync(int id);
        public Task<Employee?> GetByIdWithBookingsServicesAndUnavailabilityAsync(int id);
        public Task AddUnavailabilityAsync(int employeeId, UnavailablePeriod period);
        public Task<IEnumerable<UnavailablePeriod>> GetUnavailabilityAsync(int employeeId);
        public Task<bool> IsAvailableAsync(int employeeId, DateTime start, DateTime end);
        public Task SaveChangesAsync();

    }
}
