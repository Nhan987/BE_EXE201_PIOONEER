using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class OrderUpDTO
    {
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = " OrderRequirement can only contain alphabetic characters.")]
        public string OrderRequirement { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = " OrderCode can only contain alphabetic characters.")]
        public string OrderCode { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = " PaymentMethod can only contain alphabetic characters.")]
        public string PaymentMethod { get; set; }
        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "TotalPrice description must be a valid float number.")]
        public double TotalPrice { get; set; }

    }
}
