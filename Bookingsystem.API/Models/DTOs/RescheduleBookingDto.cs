namespace BookingSystem.API.Models.DTOs
{
    public class RescheduleBookingDto
    {
        public DateTime NewStartTime { get; set; }
        public DateTime NewEndTime { get; set; }
    }
}
