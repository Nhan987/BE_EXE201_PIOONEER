using PIOONEER_Repository.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class OrderAddDTO
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "ProductId must be a number.")]
        public long UserId { get; set; }
  
        public string OrderRequirement { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "OrderCode can only contain alphabetic characters.")]

        public string PaymentMethod { get; set; }
        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "TotalPrice description must be a valid float number.")]
        public double TotalPrice { get; set; }


    }
}
