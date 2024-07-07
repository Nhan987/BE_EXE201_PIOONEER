using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Model.DTO
{
    public class OrderResponse
    {
        public long Id { get; set; }
        public string name { get; set; } // Thay đổi từ UserId thành Username
        public string OrderRequirement { get; set; }
        public string? ShippingMethod { get; set; } // Sửa chính tả 'shippingMethod' thành 'ShippingMethod'
        public string OrderCode { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreateDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }
        public ICollection<OrderDetailsResponse> OrderDetails { get; set; }
    }
}
