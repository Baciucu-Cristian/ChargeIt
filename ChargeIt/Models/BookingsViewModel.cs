using System.ComponentModel.DataAnnotations;

namespace ChargeIt.Models
{
    public class BookingsViewModel
    {
        public List<ChargeMachineViewModel> ChargeMachines { get; set; }
        public List<CarViewModel> Cars { get; set; }
        [Display(Name = "Charge Machine")]
        public int ChargeMachineId { get; set; }
        [Display(Name = "Car")]
        public int CarId { get; set; }
    }
}
