namespace BookingSystem.API.Models.DTOs
{
    public class BookingPriceDto
    {
        public string ServiceName { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
