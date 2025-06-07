namespace BookingSystem.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDateTimeService _dateTimeProvider;

        public EmployeeService(IDateTimeService dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public (DateTime? StartDate, DateTime? EndDate) GetPeriodDates(string? period)
        {
            if (string.IsNullOrEmpty(period))
                return (null, null);
            
            if (period.Equals("day", StringComparison.OrdinalIgnoreCase))
            {
                return (_dateTimeProvider.Today, _dateTimeProvider.Today.AddDays(1));
            }
            else if (period.Equals("week", StringComparison.OrdinalIgnoreCase))
            {
                return (_dateTimeProvider.Today, _dateTimeProvider.Today.AddDays(7));
            }

            return (null, null);
        }
    }
}
