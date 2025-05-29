namespace BookingSystem.API.Models.DTOs
{
    public class NewBookingDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCancelled { get; set; }

        public CustomerDto? Customer { get; set; }
        public EmployeeDto? Employee { get; set; }
        public List<ServiceDto>? Services { get; set; }
    }

    public class CustomerDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class EmployeeDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class ServiceDto
    {
        public string? ServiceName { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal Price { get; set; }
    }
}
