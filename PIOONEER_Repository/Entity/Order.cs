using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Repository.Entity
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long UserId { get; set; }

        public string OrderRequirement { get; set; }
        public string OrderCode { get; set; }
        public string shippingMethod { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime CreateDate { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }

    }
}
