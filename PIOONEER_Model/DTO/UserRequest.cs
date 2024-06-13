using System;
using System.ComponentModel.DataAnnotations;

namespace PIOONEER_Model.DTO
{
    public class UserRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }
<<<<<<< Updated upstream
        public long RoleId { get; set; }
        public string Status { get; set; }
=======

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }
>>>>>>> Stashed changes
    }
}