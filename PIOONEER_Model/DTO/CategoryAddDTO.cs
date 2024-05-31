using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class CategoryAddDTO
    {
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "CategoryName can only contain alphabetic characters.")]
        public string CategoryName { get; set; }
    }
}
