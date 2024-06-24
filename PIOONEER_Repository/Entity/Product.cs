using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIOONEER_Repository.Entity
{
    [Table("Product")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        public long DiscountId { get; set; } 
        public long CategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ProductUrlImg { get; set; }
        public bool Status { get; set; }
        public long ProductByUserId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("DiscountId")]
        public virtual Discount Discount { get; set; }

        [ForeignKey("ProductByUserId")]
        public virtual User ProductByUser { get; set; }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
