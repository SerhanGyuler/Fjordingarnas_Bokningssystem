namespace BookingSystem.API.Services
{
    public class BookingService
    {
        private readonly IBookingService _bookingService;

        public BookingService(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
    }
}
