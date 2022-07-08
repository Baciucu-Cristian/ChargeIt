using System.ComponentModel.DataAnnotations;

namespace ChargeIt.Models
{
    public class BookingsViewModel
    {
        public List<DropDownViewModel> ChargeMachines { get; set; }
        public List<DropDownViewModel> Cars { get; set; }
        [Display(Name = "Charge Machine")]
        [Required(ErrorMessage = "Please select a valid charge machine")]
        public int? ChargeMachineId { get; set; }
        [Display(Name = "Car")]
        [Required(ErrorMessage = "Please select a valid car")]
        public int? CarId { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        [Display(Name = "Available intervals")]
        [Required(ErrorMessage = "Please select a valid interval")]
        public int? IntervalHour { get; set; }
    }
}
