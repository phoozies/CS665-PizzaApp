using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS665_PizzaRestaurantApp.Models
{
    class OrderModel
    {
        [Key]
        public int OrderID { get; set; }  // Primary Key

        [Required]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }  // Foreign Key linking to Customer

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual CustomerModel Customer { get; set; }
        public virtual ICollection<OrderDetailModel> OrderDetails { get; set; }

        // Calculated property
        public decimal TotalAmount => OrderDetails?.Sum(od => od.Quantity * od.UnitPrice) ?? 0;
    }
}
