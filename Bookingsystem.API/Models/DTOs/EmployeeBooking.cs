namespace BookingSystem.API.Models.DTOs
{
    public class EmployeeBooking
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCancelled { get; set; }

        public int CustomerId { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }

        public int EmployeeId { get; set; }
        public string? EmployeeFirstName { get; set; }
        public string? EmployeeLastName { get; set; }

        public List<ServiceDto> Services { get; set; } = new();
    }
}
