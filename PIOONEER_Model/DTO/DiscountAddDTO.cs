using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace PIOONEER_Model.DTO
{
    public class DiscountAddDTO
    {
        [Required]
        [ValidDate("yyyy-MM-dd", ErrorMessage = "The field {0} must be a valid date in the format yyyy-MM-dd.")]
        public DateTime ExpiredDay { get; set; }
        [Required]
        [ValidFloat(ErrorMessage = "TotalPrice description must be a valid float number.")]
        public float Percent {  get; set; }
    }
}
