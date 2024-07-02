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
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }


        public string Address { get; set; }

        public string OrderRequirement { get; set; }


        public string shippingMethod { get; set; }
      
        public string PaymentMethod { get; set; }

        public string Status { get; set; }

    }
}
