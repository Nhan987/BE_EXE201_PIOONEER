using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class OrderDetailsAddDTO
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "ProductId must be a number.")]
        public long ProductId { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "OrderId must be a number.")]
        public long OrderId { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "OrderQuantity must be a number.")]
        public int OrderQuantity { get; set; }
        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$", ErrorMessage = "TotalPrice description must be a valid float number.")]
        public double OrderPrice { get; set; }
    }
}
