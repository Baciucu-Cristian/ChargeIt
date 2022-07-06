namespace ChargeIt.Models
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public Guid Code { get; set; }
        public DateTime StartTime { get; set; }
        public CarViewModel Car { get; set; }
        public ChargeMachineViewModel ChargeMachine { get; set; }
    }
}
