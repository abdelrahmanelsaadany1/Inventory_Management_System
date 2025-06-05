using System.ComponentModel.DataAnnotations;
using System;

namespace Inventory_Management_System.Models
{
    public class Manager
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Manager Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.,&]+$", ErrorMessage = "Name can only contain letters, numbers, spaces, hyphens, periods, commas, and ampersands.")]
        [Display(Name = "Manager Name")]
        public string Name { get; set; }

        // Updated Regular Expression for ContactInfo
        [RegularExpression(@"^(010|011|012)\d{8}$", ErrorMessage = "Please enter a valid 11-digit Egyptian phone number starting with 010, 011, or 012.")]
        [Display(Name = "Contact Info")]
        public string ContactInfo { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}