namespace BookingSystem.API.Models.DTOs
{
    public class AvailableTimeSpotDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
