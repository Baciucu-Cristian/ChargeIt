namespace ChargeIt.Models
{
    public class StationDetailsViewModel
    {
        public ChargeMachineViewModel ChargeMachine { get; set; }
        public List<BookingViewModel> Bookings { get; set; }
    }
}
