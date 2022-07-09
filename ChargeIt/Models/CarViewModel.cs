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
        [Required]
        [Display(Name = "Owner")]		
		public int? CarOwnerId { get; set; }
        [Display(Name = "Owner")]		
		public string CarOwner { get; set; }
		public List<DropDownViewModel> CarOwners { get; set; }
	}
}
