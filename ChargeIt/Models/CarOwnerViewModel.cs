﻿using System.ComponentModel.DataAnnotations;

namespace ChargeIt.Models
{
    public class CarOwnerViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
