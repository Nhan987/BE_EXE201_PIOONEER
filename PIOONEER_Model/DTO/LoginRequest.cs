﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class LoginRequest
    {
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
