namespace BookingSystem.API.Models.DTOs
{
    public class BookingPriceDto
    {
        public string ServiceName { get; set; } = null!;
        public TimeSpan Duration { get; set; }
        public decimal Price { get; set; }
    }
}
