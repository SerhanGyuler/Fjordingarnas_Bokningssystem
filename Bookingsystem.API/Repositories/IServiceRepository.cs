using Fjordingarnas_Bokningssystem.Models;

namespace BookingSystem.API.Repositories
{
    public interface IServiceRepository
    {
        public Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<int> ids);
        public Task<List<Service>> GetServicesByBookingIdAsync(int bookingId);
    }
}
