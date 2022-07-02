using System.ComponentModel.DataAnnotations;

namespace ChargeIt.Models
{
	public class CarViewModel
	{
		public int Id { get; set; }
        [Required]
        [MaxLength(12)]
        [Display(Name = "Plate Number")]		
		public string PlateNumber { get; set; }
	}
}
