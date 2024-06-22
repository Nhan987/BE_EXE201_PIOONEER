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

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        public string OrderRequirement { get; set; }

        [Required(ErrorMessage = "ShippingMethod is required")]
        public string shippingMethod { get; set; }

        [Required(ErrorMessage = "PaymentMethod is required")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "TotalPrice is required")]
        public double TotalPrice { get; set; }

        [Required(ErrorMessage = "OrderDetail is required")]
        public List<OrderDetailsAddDTO> OrderDetail { get; set; }

    }

}
