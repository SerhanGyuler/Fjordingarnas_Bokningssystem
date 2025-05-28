namespace BookingSystem.API.Models.DTOs
{
    public class BookingOverviewDto
    {
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; } 
        public string Service { get; set; }
        public string Employee { get; set; }
        public string Customer { get; set; }
    }
}
