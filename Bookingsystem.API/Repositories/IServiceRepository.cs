using BookingSystem.API.Models.DTOs;
using Fjordingarnas_Bokningssystem.Models;

namespace BookingSystem.API.Repositories
{
    public interface IServiceRepository
    {
        public Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<int> ids);
        public Task<List<Service>> GetServicesByBookingIdAsync(int bookingId);
        public Task<Service> CreateService(ServiceInputDto service);
        Task<bool> DeleteServiceAsync(int id);
        Task<bool> UpdateServiceAsync(Service service, ServiceInputDto updateDto);
        Task<Service?> GetServiceByIdAsync(int id);
    }
}
