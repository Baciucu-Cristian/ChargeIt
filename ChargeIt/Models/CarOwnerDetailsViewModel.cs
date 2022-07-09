using System.ComponentModel.DataAnnotations;

namespace ChargeIt.Models
{
    public class CarOwnerDetailsViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public List<CarViewModel> Cars { get; set; }
    }
}
