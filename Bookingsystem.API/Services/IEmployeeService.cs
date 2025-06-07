namespace BookingSystem.API.Services
{
    public interface IEmployeeService
    {
        (DateTime? StartDate, DateTime? EndDate) GetPeriodDates(string? period);
    }
}
