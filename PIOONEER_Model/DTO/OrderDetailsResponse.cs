using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class OrderDetailsResponse
    {
        public long Id { get; set; }
        public string ProductName { get; set; }  // Changed from ProductId
        public long OrderId { get; set; }
        public int OrderQuantity { get; set; }
        public double OrderPrice { get; set; }
    }
}
