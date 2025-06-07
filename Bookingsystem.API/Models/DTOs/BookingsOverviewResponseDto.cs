namespace BookingSystem.API.Models.DTOs
{
    public class BookingsOverviewResponseDto
    {
        public string Range { get; set; } = null!;
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public Dictionary<string, List<BookingOverviewDto>> Bookings { get; set; } = new();
    }
}
