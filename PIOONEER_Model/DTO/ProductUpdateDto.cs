﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class ProductUpdateDto
    {


        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "ProductName can only contain alphabetic characters.")]
        public string ProductName { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "ProductDescription can only contain alphabetic characters.")]
        public string ProductDescription { get; set; }
        [Required]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = " must be a  number.")]
        public double ProductPrice { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "OrderId must be a number.")]
        public int ProductQuantity { get; set; }

        [Required]
        public IFormFile ProductImg { get; set; }


    }
}
