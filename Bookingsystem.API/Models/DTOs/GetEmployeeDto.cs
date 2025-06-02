namespace BookingSystem.API.Models.DTOs
{
    public class GetEmployeeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public List<string> Services { get; set; } = new();
    }
}
