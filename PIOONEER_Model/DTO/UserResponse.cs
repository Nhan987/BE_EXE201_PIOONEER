using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class UserResponse
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public long RoleId { get; set; }
        public string? PhoneNumber { get; set; }
        public string Status { get; set; }
    }
}
