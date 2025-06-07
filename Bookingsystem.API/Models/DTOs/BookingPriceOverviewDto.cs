namespace BookingSystem.API.Models.DTOs
{
    public class BookingPriceOverviewDto
    {
        public List<BookingPriceDto> Prices { get; set; } = new();
        public decimal Total { get; set; }
    }
}
