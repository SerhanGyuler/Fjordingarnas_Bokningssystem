namespace BookingSystem.API.Models.DTOs
{
    public class CreatedServiceDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = null!;
        public TimeSpan Duration { get; set; }
        public decimal Price { get; set; }
    }
}
