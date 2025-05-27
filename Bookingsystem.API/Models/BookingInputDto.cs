namespace BookingSystem.API.Models
{
    public class BookingInputDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public List<int> ServiceIds { get; set; } = new();
    }
}
