namespace BookingSystem.API.Models.DTOs
{
    public class UnavailabilityDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
}
