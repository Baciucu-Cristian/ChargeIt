namespace ChargeIt.Models
{
    public class CarDetailsViewModel
    {
        public CarViewModel Car { get; set; }
        public List<BookingViewModel> Bookings { get; set; }
    }
}
