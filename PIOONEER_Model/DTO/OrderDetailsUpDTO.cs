using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class OrderDetailsUpDTO
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "= must be a number not float.")]
        public int OrderQuantity { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = " must be a number.")]
        public double OrderPrice { get; set; }
    }
}
