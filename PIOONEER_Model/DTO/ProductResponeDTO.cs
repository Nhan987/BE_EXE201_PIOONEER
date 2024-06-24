using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class ProductResponeDTO
    {
        public long Id { get; set; }
//        public long DiscountId { get; set; }
//        public long CategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ProductUrlImg { get; set; }
        public bool Status { get; set; }
        public long ProductByUserId { get; set; }

    }
}
