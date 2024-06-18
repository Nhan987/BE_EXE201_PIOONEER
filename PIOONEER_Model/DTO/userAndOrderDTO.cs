using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class userAndOrderDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public string OrderCode { get; set; }
        public string? OrderRequirement { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "OrderCode can only contain alphabetic characters.")]
        public string? shippingMethod { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "TotalPrice description must be a valid float number.")]
        public double TotalPrice { get; set; }

        public long ProductId { get; set; }
        public int OrderQuantity { get; set; }
        public double OrderPrice { get; set; }

    }
}
