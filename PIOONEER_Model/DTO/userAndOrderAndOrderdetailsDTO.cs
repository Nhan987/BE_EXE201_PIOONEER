using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class userAndOrderAndOrderdetailsDTO
    {
        public string OrderCode { get; set; }
        public long ProductId { get; set; }
        public long OrderId { get; set; }
        public int OrderQuantity { get; set; }
        public double OrderPrice { get; set; }
    }
}
